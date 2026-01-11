using BlogApp.Shared.Domain.Results;

namespace BlogApp.Shared.Domain.Errors;

public readonly record struct Error
{
    private Error(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
        NumericType = (int)type;
    }

    public string Code { get; }
    public string Description { get; }
    public ErrorType Type { get; }
    public int NumericType { get; }

    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
    public static Error Unexpected(string code, string description) => new(code, description, ErrorType.Unexpected);
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);
    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);

    //Cast (ErrorType)type
    public static Error Custom(int type, string code, string description) => new(code, description, (ErrorType)type);

    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    // Implicit conversion to Result
    public static implicit operator Result(Error error) => Result.Failure(error);
}