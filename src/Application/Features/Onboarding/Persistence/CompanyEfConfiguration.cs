using Application.Features.Onboarding.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application.Features.Onboarding.Persistence;

public class CompanyEfConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.CorporateName)
            .IsRequired();
    }
}