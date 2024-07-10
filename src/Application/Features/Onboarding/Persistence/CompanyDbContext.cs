using Application.Features.Onboarding.Domain;

namespace Application.Common.Persistence;

public partial class AppDbContext
{
    public DbSet<Company> Onboarding { get; set; } = null!;
}