namespace BlogApp.Shared.Domain.Errors;

public sealed record ValidationError(string PropertyName, string ErrorMessage);