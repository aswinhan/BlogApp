using BlogApp.Shared.Domain.Errors;

namespace BlogApp.Shared.Domain.Results;

// 1. Non-Generic Result (Void)
public readonly record struct Result
{
    private readonly Error? _error;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error => _error ?? Error.None;

    private Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        _error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static implicit operator Result(Error error) => Failure(error);
}

// 2. Generic Result<T> (Value)
public readonly record struct Result<TValue>
{
    private readonly TValue? _value;
    private readonly Error? _error;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error => _error ?? Error.None;

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    // Internal constructor to be used by the non-generic Result helper
    internal Result(TValue? value, bool isSuccess, Error error)
    {
        _value = value;
        IsSuccess = isSuccess;
        _error = error;
    }

    public static implicit operator Result<TValue>(TValue? value) => new(value, true, Error.None);
    public static implicit operator Result<TValue>(Error error) => new(default, false, error);
}