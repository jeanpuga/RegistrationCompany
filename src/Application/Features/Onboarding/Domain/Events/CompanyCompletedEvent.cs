using MediatR;

namespace Application.Features.Onboarding.Domain.Events;

public record CompanyCompletedEvent(Guid CompanyId) : INotification;