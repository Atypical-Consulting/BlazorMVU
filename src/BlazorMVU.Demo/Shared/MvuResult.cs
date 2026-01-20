// This file is kept for backward compatibility.
// New code should use BlazorMVU.MvuResult directly.

namespace BlazorMVU.Demo.Shared;

/// <summary>
/// Legacy MvuResult type alias for backward compatibility.
/// For new code, use BlazorMVU.MvuResult directly.
/// </summary>
public readonly struct MvuResult<T>
{
    private readonly BlazorMVU.MvuResult<T> _inner;

    public T? Value => _inner.Value;
    public Exception? Error => _inner.Error;
    public bool IsSuccess => _inner.IsSuccess;

    private MvuResult(BlazorMVU.MvuResult<T> inner) => _inner = inner;

    public static MvuResult<T> Success(T value)
        => new(BlazorMVU.MvuResult<T>.Success(value));

    public static MvuResult<T> Failure(Exception? error)
        => new(BlazorMVU.MvuResult<T>.Failure(error));
}