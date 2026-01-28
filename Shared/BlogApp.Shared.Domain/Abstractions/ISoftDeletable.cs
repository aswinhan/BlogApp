namespace BlogApp.Shared.Domain.Abstractions;

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedOnUtc { get; }
    void SoftDelete();
    void Recover();
}