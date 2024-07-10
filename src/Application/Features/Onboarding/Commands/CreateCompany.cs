using Application.Features.Onboarding.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Features.Onboarding.Commands;

[Handler]
public sealed partial class CreateCompany : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/onboarding",
                async (Command command, Handler handler, CancellationToken cancellationToken) =>
                {
                    var id = await handler.HandleAsync(command, cancellationToken);
                    return Results.Created($"/onboarding/{id}", id);
                })
            .Produces<Company>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(Company));
    }

    public sealed record Command(string TradeName, string CorporateName, TypeCorporateSizing CorporateSizing);

    private static async ValueTask<Guid> HandleAsync(Command request, AppDbContext dbContext, IMongoDbCache cache, CancellationToken cancellationToken)
    {
        var company = new Company
        {
            CorporateName = request.CorporateName,
            TradeName = request.TradeName,
            CorporateSizing = request.CorporateSizing
        };

        await dbContext.Onboarding.AddAsync(company, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var key = $"Company:{company.Id}";

        var companyFromDb = await dbContext.Onboarding.FindAsync(new object[] { company.Id }, cancellationToken);

        if (companyFromDb == null) throw new NotFoundException(nameof(Company), company.Id);

        var companyString = JsonSerializer.Serialize(companyFromDb);
        var companyBytes = System.Text.Encoding.UTF8.GetBytes(companyString);

        await cache.SetAsync(key, companyBytes, new DistributedCacheEntryOptions(), cancellationToken);

        await cache.RemoveAsync("Company", cancellationToken);

        return company.Id;
    }
}