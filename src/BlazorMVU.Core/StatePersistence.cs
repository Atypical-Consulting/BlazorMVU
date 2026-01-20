using System.Text.Json;
using Microsoft.JSInterop;

namespace BlazorMVU;

/// <summary>
/// Options for state persistence.
/// </summary>
public record PersistenceOptions
{
    /// <summary>
    /// The storage key prefix.
    /// </summary>
    public string KeyPrefix { get; init; } = "blazormvu_";

    /// <summary>
    /// Whether to use sessionStorage instead of localStorage.
    /// </summary>
    public bool UseSessionStorage { get; init; } = false;

    /// <summary>
    /// Whether to automatically save state on every update.
    /// </summary>
    public bool AutoSave { get; init; } = false;

    /// <summary>
    /// Debounce delay for auto-save (to prevent excessive writes).
    /// </summary>
    public TimeSpan AutoSaveDebounce { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// JSON serializer options for state serialization.
    /// </summary>
    public JsonSerializerOptions? JsonOptions { get; init; }

    /// <summary>
    /// Creates default persistence options.
    /// </summary>
    public static PersistenceOptions Default => new();
}

/// <summary>
/// Provides state persistence capabilities using browser storage.
/// </summary>
/// <typeparam name="TModel">The model type to persist.</typeparam>
public class StatePersistence<TModel> : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly PersistenceOptions _options;
    private readonly string _storageKey;
    private CancellationTokenSource? _autoSaveCts;
    private DateTime _lastSaveTime = DateTime.MinValue;
    private TModel? _pendingState;
    private bool _disposed;

    /// <summary>
    /// Event raised when state is loaded from storage.
    /// </summary>
    public event Func<TModel, Task>? OnStateLoaded;

    /// <summary>
    /// Event raised when state is saved to storage.
    /// </summary>
    public event Action<TModel>? OnStateSaved;

    /// <summary>
    /// Event raised when an error occurs during persistence operations.
    /// </summary>
    public event Action<Exception>? OnError;

    /// <summary>
    /// Creates a new state persistence instance.
    /// </summary>
    /// <param name="jsRuntime">The JS runtime for browser storage access.</param>
    /// <param name="componentKey">Unique key for this component's state.</param>
    /// <param name="options">Persistence options.</param>
    public StatePersistence(
        IJSRuntime jsRuntime,
        string componentKey,
        PersistenceOptions? options = null)
    {
        _jsRuntime = jsRuntime;
        _options = options ?? PersistenceOptions.Default;
        _storageKey = $"{_options.KeyPrefix}{componentKey}";
    }

    /// <summary>
    /// Loads state from browser storage.
    /// </summary>
    /// <returns>The loaded state, or null if not found.</returns>
    public async Task<TModel?> LoadAsync(CancellationToken ct = default)
    {
        try
        {
            var storageName = _options.UseSessionStorage ? "sessionStorage" : "localStorage";
            var json = await _jsRuntime.InvokeAsync<string?>(
                $"{storageName}.getItem",
                ct,
                _storageKey);

            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            var state = JsonSerializer.Deserialize<TModel>(json, _options.JsonOptions);

            if (state != null && OnStateLoaded != null)
            {
                await OnStateLoaded(state);
            }

            return state;
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
            return default;
        }
    }

    /// <summary>
    /// Saves state to browser storage.
    /// </summary>
    public async Task SaveAsync(TModel state, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(state, _options.JsonOptions);
            var storageName = _options.UseSessionStorage ? "sessionStorage" : "localStorage";

            await _jsRuntime.InvokeVoidAsync(
                $"{storageName}.setItem",
                ct,
                _storageKey,
                json);

            _lastSaveTime = DateTime.UtcNow;
            OnStateSaved?.Invoke(state);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    /// <summary>
    /// Queues a state save with debouncing (for auto-save scenarios).
    /// </summary>
    public void QueueSave(TModel state)
    {
        _pendingState = state;

        // Cancel any pending save
        _autoSaveCts?.Cancel();
        _autoSaveCts?.Dispose();
        _autoSaveCts = new CancellationTokenSource();

        var ct = _autoSaveCts.Token;

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(_options.AutoSaveDebounce, ct);

                if (!ct.IsCancellationRequested && _pendingState != null)
                {
                    await SaveAsync(_pendingState, ct);
                    _pendingState = default;
                }
            }
            catch (TaskCanceledException)
            {
                // Expected when debouncing
            }
        }, ct);
    }

    /// <summary>
    /// Removes state from browser storage.
    /// </summary>
    public async Task ClearAsync(CancellationToken ct = default)
    {
        try
        {
            var storageName = _options.UseSessionStorage ? "sessionStorage" : "localStorage";
            await _jsRuntime.InvokeVoidAsync(
                $"{storageName}.removeItem",
                ct,
                _storageKey);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(ex);
        }
    }

    /// <summary>
    /// Checks if state exists in browser storage.
    /// </summary>
    public async Task<bool> ExistsAsync(CancellationToken ct = default)
    {
        try
        {
            var storageName = _options.UseSessionStorage ? "sessionStorage" : "localStorage";
            var value = await _jsRuntime.InvokeAsync<string?>(
                $"{storageName}.getItem",
                ct,
                _storageKey);

            return !string.IsNullOrEmpty(value);
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;

        _disposed = true;
        _autoSaveCts?.Cancel();
        _autoSaveCts?.Dispose();

        // Save any pending state before disposing
        if (_pendingState != null)
        {
            await SaveAsync(_pendingState);
        }

        GC.SuppressFinalize(this);
    }
}
