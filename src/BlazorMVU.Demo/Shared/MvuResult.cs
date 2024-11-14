namespace BlazorMVU.Demo.Shared;

public struct MvuResult<T>
{
    public T? Value { get; }
    public Exception? Error { get; }
    public bool IsSuccess { get; }

    private MvuResult(T? value, Exception? error, bool isSuccess)
    {
        Value = value;
        Error = error;
        IsSuccess = isSuccess;
    }

    public static MvuResult<T> Success(T value)
        => new(value, null, true);

    public static MvuResult<T> Failure(Exception? error)
        => new(default, error, false);
}