using Application.Features.Onboarding.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Features.Onboarding.Queries;

[Handler]
public sealed partial class GetCompany : IEndpoint
{
    // Assuming there's a way to access the MongoDBCache instance, e.g., through a static property
    private static readonly IDistributedCache _mongoDBCache; // This needs to be set from outside

    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/onboarding/{id:guid}",
                (Guid id, Handler handler, CancellationToken cancellationToken)
                    => handler.HandleAsync(new Query(id), cancellationToken))
            .Produces<Company>()
            .Produces(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(Company));
    }

    public sealed record Query(Guid Id);

    private static async ValueTask<Company> HandleAsync(Query request, AppDbContext dbContext, IMongoDbCache cache, CancellationToken cancellationToken)
    {
        var key = $"Company:{request.Id}";

        byte[] cachedCompanyBytes = await cache.GetAsync(key, cancellationToken);

        if (cachedCompanyBytes != null)
        {
            var cachedCompanyString = System.Text.Encoding.UTF8.GetString(cachedCompanyBytes);
            var company = JsonSerializer.Deserialize<Company>(cachedCompanyString);
            if (company != null)
            {
                return company;
            }
        }

        var companyFromDb = await dbContext.Onboarding.FindAsync(new object[] { request.Id }, cancellationToken);

        if (companyFromDb == null) throw new NotFoundException(nameof(Company), request.Id);

        var companyString = JsonSerializer.Serialize(companyFromDb);
        var companyBytes = System.Text.Encoding.UTF8.GetBytes(companyString);

        await cache.SetAsync(key, companyBytes, new DistributedCacheEntryOptions(), cancellationToken);

        return companyFromDb;
    }
}

