namespace BlogApp.Shared.Domain.Abstractions;

public interface IAuditableEntity
{
    DateTime CreatedOnUtc { get; set; }
    DateTime? ModifiedOnUtc { get; set; }
}