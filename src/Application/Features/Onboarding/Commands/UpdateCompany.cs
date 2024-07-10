using Application.Features.Onboarding.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Features.Onboarding.Commands;

[Handler]
public sealed partial class UpdateCompany : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/onboarding/{id:guid}",
                async (Guid id, Command command, Handler handler, CancellationToken cancellationToken) =>
                {
                    await handler.HandleAsync(command with
                    {
                        Id = id // TODO: Remove this duplication
                    }, cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(Company));
    }

    public sealed record Command(Guid Id, string TradeName, string CorporateName, TypeCorporateSizing CorporateSizing);

    private static async ValueTask HandleAsync(Command request, AppDbContext dbContext, IMongoDbCache cache, CancellationToken cancellationToken)
    {
        var key = $"Company:{request.Id}";

        var company = await dbContext.Onboarding.FindAsync([request.Id], cancellationToken);

        if (company == null) throw new NotFoundException(nameof(Company), request.Id);

        company.TradeName = request.TradeName;
        company.CorporateName = request.CorporateName;
        company.CorporateSizing = request.CorporateSizing;

        var companyString = JsonSerializer.Serialize(company);
        var companyBytes = System.Text.Encoding.UTF8.GetBytes(companyString);

        await cache.SetAsync(key, companyBytes, new DistributedCacheEntryOptions(), cancellationToken);
        await cache.RemoveAsync("Company", cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}