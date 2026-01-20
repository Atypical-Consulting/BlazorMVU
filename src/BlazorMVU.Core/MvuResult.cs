namespace BlazorMVU;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// Used for handling async operation results in a type-safe way.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public readonly struct MvuResult<T>
{
    /// <summary>
    /// Gets the success value, if any.
    /// </summary>
    public T? Value { get; }

    /// <summary>
    /// Gets the error, if any.
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// Gets whether this result represents a success.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets whether this result represents a failure.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    private MvuResult(T? value, Exception? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="value">The success value.</param>
    public static MvuResult<T> Success(T value) => new(value, null, true);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    public static MvuResult<T> Failure(Exception? error) => new(default, error, false);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public static MvuResult<T> Failure(string message) => new(default, new Exception(message), false);

    /// <summary>
    /// Maps the success value using the provided function.
    /// </summary>
    public MvuResult<TNew> Map<TNew>(Func<T, TNew> mapper) =>
        IsSuccess && Value is not null
            ? MvuResult<TNew>.Success(mapper(Value))
            : MvuResult<TNew>.Failure(Error);

    /// <summary>
    /// Flat maps the success value using the provided function.
    /// </summary>
    public MvuResult<TNew> Bind<TNew>(Func<T, MvuResult<TNew>> binder) =>
        IsSuccess && Value is not null
            ? binder(Value)
            : MvuResult<TNew>.Failure(Error);

    /// <summary>
    /// Gets the value or a default.
    /// </summary>
    public T? GetValueOrDefault(T? defaultValue = default) =>
        IsSuccess ? Value : defaultValue;

    /// <summary>
    /// Pattern matches on the result.
    /// </summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Exception?, TResult> onFailure) =>
        IsSuccess && Value is not null
            ? onSuccess(Value)
            : onFailure(Error);

    /// <summary>
    /// Executes an action based on the result.
    /// </summary>
    public void Switch(
        Action<T> onSuccess,
        Action<Exception?> onFailure)
    {
        if (IsSuccess && Value is not null)
            onSuccess(Value);
        else
            onFailure(Error);
    }

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator MvuResult<T>(T value) => Success(value);

    /// <summary>
    /// Implicitly converts an exception to a failed result.
    /// </summary>
    public static implicit operator MvuResult<T>(Exception error) => Failure(error);
}

/// <summary>
/// Helper methods for creating MvuResult instances.
/// </summary>
public static class MvuResult
{
    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static MvuResult<T> Success<T>(T value) => MvuResult<T>.Success(value);

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static MvuResult<T> Failure<T>(Exception? error) => MvuResult<T>.Failure(error);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    public static MvuResult<T> Failure<T>(string message) => MvuResult<T>.Failure(message);

    /// <summary>
    /// Executes an async operation and wraps the result.
    /// </summary>
    public static async Task<MvuResult<T>> TryAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            var result = await operation();
            return MvuResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return MvuResult<T>.Failure(ex);
        }
    }

    /// <summary>
    /// Executes a synchronous operation and wraps the result.
    /// </summary>
    public static MvuResult<T> Try<T>(Func<T> operation)
    {
        try
        {
            var result = operation();
            return MvuResult<T>.Success(result);
        }
        catch (Exception ex)
        {
            return MvuResult<T>.Failure(ex);
        }
    }
}
