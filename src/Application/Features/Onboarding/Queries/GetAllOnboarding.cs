using Application.Features.Onboarding.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Linq.Expressions;
using System.Text.Json;

namespace Application.Features.Onboarding.Queries;

[Handler]
public sealed partial class GetAllOnboarding : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/onboarding",
                ([AsParameters] Query query, Handler handler, CancellationToken cancellationToken)
                    => handler.HandleAsync(query, cancellationToken))
            .Produces<IReadOnlyList<Company>>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(Company));
    }

    public sealed record Query(bool? IsCompleted = null);

    private static async ValueTask<IReadOnlyList<Company>> HandleAsync(Query request, AppDbContext dbContext, IMongoDbCache cache, CancellationToken cancellationToken)
    {
        var key = $"Company";

        Expression<Func<Company, bool>> condition(Query request) => x => request.IsCompleted == null || x.IsCompleted == request.IsCompleted;

        byte[] cachedCompanyBytes = await cache.GetAsync(key, cancellationToken);

        if (cachedCompanyBytes != null)
        {
            var cachedCompanyString = System.Text.Encoding.UTF8.GetString(cachedCompanyBytes);
            var companies = JsonSerializer.Deserialize<List<Company>>(cachedCompanyString);
            if (companies != null)
            {
                return companies
                    .AsQueryable()
                    .Where(condition(request))
                    .ToList();
            }
        }

        var companiesFromDb = await dbContext.Onboarding
            .Where(condition(request))
            .ToListAsync(cancellationToken);

        if (companiesFromDb == null) throw new NotFoundException(nameof(Company));

        var companiesString = JsonSerializer.Serialize(companiesFromDb);
        var companiesBytes = System.Text.Encoding.UTF8.GetBytes(companiesString);

        await cache.SetAsync(key, companiesBytes, new DistributedCacheEntryOptions(), cancellationToken);

        return companiesFromDb.AsReadOnly();
    }
}