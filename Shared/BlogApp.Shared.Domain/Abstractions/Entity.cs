namespace BlogApp.Shared.Domain.Abstractions;

public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected Entity(Guid id)
    {
        Id = id;
    }

    // Default constructor for EF Core
    protected Entity() { }

    public Guid Id { get; init; }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
    {
        // Optimization: returns a wrapper instead of copying the whole list
        return _domainEvents.AsReadOnly();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}