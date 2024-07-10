using MediatR;

namespace Application.Common.Domain.Bases;

public abstract class BaseEntity
{
    public readonly List<INotification> StagedEvents = [];
}