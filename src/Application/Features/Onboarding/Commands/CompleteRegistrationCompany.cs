using Application.Features.Onboarding.Domain;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Application.Features.Onboarding.Commands;


[Handler]
public sealed partial class CompleteRegistrationCompany : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/onboarding/{id:guid}/complete",
                async ([FromRoute] Guid id, Handler handler, CancellationToken cancellationToken) =>
                {
                    await handler.HandleAsync(new Command(id), cancellationToken);
                    return Results.NoContent();
                })
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .WithTags(nameof(Company));
    }
    public sealed record Command(Guid Id);
    private static async ValueTask HandleAsync(Command request, AppDbContext dbContext, IMongoDbCache cache, CancellationToken cancellationToken)
    {
        var key = $"Company:{request.Id}";

        var company = await dbContext.Onboarding.FindAsync([request.Id], cancellationToken);

        if (company == null) throw new NotFoundException(nameof(Company), request.Id);

        company.Complete();

        var companyString = JsonSerializer.Serialize(company);
        var companyBytes = System.Text.Encoding.UTF8.GetBytes(companyString);

        await cache.SetAsync(key, companyBytes, new DistributedCacheEntryOptions(), cancellationToken);
        await cache.RemoveAsync("Company", cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
