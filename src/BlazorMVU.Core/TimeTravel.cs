namespace BlazorMVU;

/// <summary>
/// Represents a single entry in the state history.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TMsg">The message type.</typeparam>
public record StateHistoryEntry<TModel, TMsg>(
    TModel State,
    TMsg? Message,
    DateTime Timestamp)
{
    /// <summary>
    /// Creates an initial history entry (no message).
    /// </summary>
    public static StateHistoryEntry<TModel, TMsg> Initial(TModel state) =>
        new(state, default, DateTime.UtcNow);

    /// <summary>
    /// Creates a history entry from a message dispatch.
    /// </summary>
    public static StateHistoryEntry<TModel, TMsg> FromDispatch(TModel state, TMsg message) =>
        new(state, message, DateTime.UtcNow);
}

/// <summary>
/// Provides time-travel debugging capabilities for MVU components.
/// Tracks state history and allows navigation through past states.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TMsg">The message type.</typeparam>
public class TimeTravelDebugger<TModel, TMsg>
    where TModel : notnull
{
    private readonly List<StateHistoryEntry<TModel, TMsg>> _history = [];
    private readonly int _maxHistorySize;
    private int _currentIndex = -1;
    private bool _isPaused;

    /// <summary>
    /// Event raised when the state history changes.
    /// </summary>
    public event Action<TimeTravelDebugger<TModel, TMsg>>? OnHistoryChanged;

    /// <summary>
    /// Gets the current history of states.
    /// </summary>
    public IReadOnlyList<StateHistoryEntry<TModel, TMsg>> History => _history.AsReadOnly();

    /// <summary>
    /// Gets the current position in the history.
    /// </summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>
    /// Gets whether time-travel is currently paused.
    /// </summary>
    public bool IsPaused => _isPaused;

    /// <summary>
    /// Gets the current state.
    /// </summary>
    public TModel? CurrentState => _currentIndex >= 0 && _currentIndex < _history.Count
        ? _history[_currentIndex].State
        : default;

    /// <summary>
    /// Gets whether we can go back in history.
    /// </summary>
    public bool CanGoBack => _currentIndex > 0;

    /// <summary>
    /// Gets whether we can go forward in history.
    /// </summary>
    public bool CanGoForward => _currentIndex < _history.Count - 1;

    /// <summary>
    /// Creates a new time-travel debugger.
    /// </summary>
    /// <param name="maxHistorySize">Maximum number of states to keep in history.</param>
    public TimeTravelDebugger(int maxHistorySize = 100)
    {
        _maxHistorySize = maxHistorySize;
    }

    /// <summary>
    /// Records the initial state.
    /// </summary>
    public void RecordInitialState(TModel state)
    {
        _history.Clear();
        _history.Add(StateHistoryEntry<TModel, TMsg>.Initial(state));
        _currentIndex = 0;
        OnHistoryChanged?.Invoke(this);
    }

    /// <summary>
    /// Records a new state after a message dispatch.
    /// Returns true if the state was recorded, false if paused.
    /// </summary>
    public bool RecordState(TModel state, TMsg message)
    {
        if (_isPaused)
        {
            return false;
        }

        // If we've gone back in time and then dispatch a new message,
        // we truncate the future history
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
        }

        _history.Add(StateHistoryEntry<TModel, TMsg>.FromDispatch(state, message));

        // Trim history if it exceeds the maximum size
        while (_history.Count > _maxHistorySize)
        {
            _history.RemoveAt(0);
        }

        _currentIndex = _history.Count - 1;
        OnHistoryChanged?.Invoke(this);
        return true;
    }

    /// <summary>
    /// Goes back one step in history.
    /// </summary>
    /// <returns>The previous state, or null if at the beginning.</returns>
    public TModel? GoBack()
    {
        if (!CanGoBack) return default;

        _currentIndex--;
        _isPaused = true;
        OnHistoryChanged?.Invoke(this);
        return _history[_currentIndex].State;
    }

    /// <summary>
    /// Goes forward one step in history.
    /// </summary>
    /// <returns>The next state, or null if at the end.</returns>
    public TModel? GoForward()
    {
        if (!CanGoForward) return default;

        _currentIndex++;

        // If we've reached the end, unpause
        if (_currentIndex == _history.Count - 1)
        {
            _isPaused = false;
        }

        OnHistoryChanged?.Invoke(this);
        return _history[_currentIndex].State;
    }

    /// <summary>
    /// Jumps to a specific index in the history.
    /// </summary>
    /// <returns>The state at the specified index, or null if invalid.</returns>
    public TModel? GoTo(int index)
    {
        if (index < 0 || index >= _history.Count) return default;

        _currentIndex = index;
        _isPaused = index < _history.Count - 1;
        OnHistoryChanged?.Invoke(this);
        return _history[_currentIndex].State;
    }

    /// <summary>
    /// Resumes recording at the current position, discarding future history.
    /// </summary>
    public void Resume()
    {
        if (!_isPaused) return;

        // Truncate any future history
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
        }

        _isPaused = false;
        OnHistoryChanged?.Invoke(this);
    }

    /// <summary>
    /// Clears all history and resets with the given state.
    /// </summary>
    public void Reset(TModel state)
    {
        _history.Clear();
        _history.Add(StateHistoryEntry<TModel, TMsg>.Initial(state));
        _currentIndex = 0;
        _isPaused = false;
        OnHistoryChanged?.Invoke(this);
    }

    /// <summary>
    /// Exports the history as a serializable list.
    /// </summary>
    public IReadOnlyList<StateHistoryEntry<TModel, TMsg>> Export() => _history.AsReadOnly();

    /// <summary>
    /// Imports history from a previous export.
    /// </summary>
    public void Import(IEnumerable<StateHistoryEntry<TModel, TMsg>> history)
    {
        _history.Clear();
        _history.AddRange(history);
        _currentIndex = _history.Count - 1;
        _isPaused = false;
        OnHistoryChanged?.Invoke(this);
    }
}
