using MediatR;

namespace Application.Features.Onboarding.Domain.Events;

public record CompanyCreatedEvent(Guid CompanyId) : INotification;