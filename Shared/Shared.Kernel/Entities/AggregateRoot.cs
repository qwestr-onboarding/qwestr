using MediatR;

namespace Shared.Kernel.Entities;

public abstract class AggregateRoot<TId> : Entity<TId>
{
    private readonly List<INotification> _domainEvents = new();
    
    public IReadOnlyList<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void RaiseDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}