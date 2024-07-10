using Application.Common.Domain.Bases;
using Application.Features.Onboarding.Domain.Events;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.Features.Onboarding.Domain;

public class Company : BaseEntity
{
    public Company()
    {
        StagedEvents.Add(new CompanyCreatedEvent(Id));
    }

    public Guid Id { get; init; }


    [MaxLength(256)]
    public string TradeName { get; set; } = string.Empty;

    [MaxLength(1024)]
    public string CorporateName { get; set; } = string.Empty;
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TypeCorporateSizing CorporateSizing { get; set; }


    public bool IsCompleted { get; private set; }

    /// <exception cref="InvalidOperationException">Throws when trying to complete an already completed item</exception>
    public void Complete()
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Onboarding is already completed");
        }

        IsCompleted = true;

        StagedEvents.Add(new CompanyCompletedEvent(Id));
    }
}