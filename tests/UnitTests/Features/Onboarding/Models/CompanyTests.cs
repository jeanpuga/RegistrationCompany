using Application.Features.Onboarding.Domain;
using Application.Features.Onboarding.Domain.Events;
using FluentAssertions;
using Xunit;

namespace ProjectTeste01.Unit.Tests.Features.Todos.Models
{
    public class CompanyTests
    {
        [Fact]
        public void Todo_Complete_ShouldUpdateCompleted()
        {
            // Arrange
            var item = new Company
            {
                Id = Guid.NewGuid(),
                TradeName="Joinize",
                CorporateName="Joinize Corporation Inc.2023",
                CorporateSizing=TypeCorporateSizing.SmallBusiness,
            };

            // Act
            item.Complete();

            // Assert
            item.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Todo_Complete_ShouldAddEvent()
        {
            // Arrange
            var item = new Company
            {
                Id = Guid.NewGuid(),
                TradeName = "Joinize",
                CorporateName = "Joinize Corporation Inc.2023",
                CorporateSizing = TypeCorporateSizing.SmallBusiness,
            };

            // Act
            item.Complete();

            // Assert
            item.StagedEvents.Should().ContainSingle(x => x is CompanyCompletedEvent, "Porque o cadastro foi completado!");
        }
    }
}