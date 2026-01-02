namespace BlogApp.Shared.Domain.Results;

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    protected internal Result(TValue? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);

    public static Result<TValue> Success(TValue value) => new(value, true, Error.None);

    // Hiding the base Failure to return typed Result<T>
    public new static Result<TValue> Failure(Error error) => new(default, false, error);
}