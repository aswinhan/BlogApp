namespace BlogApp.Shared.Domain.Errors;

// 1. Add 'Error[]? Errors = null' to the constructor
public sealed record Error(string Code, string Description, ErrorType Type, Error[]? Errors = null)
{
    // 2. Update static fields to use the new constructor (null is default, so simple 'new' works)
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided", ErrorType.Failure);

    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    // 3. Update Validation to actually SAVE the errors array
    public static Error Validation(string code, string description, Error[] errors) =>
        new(code, description, ErrorType.Validation, errors);
}