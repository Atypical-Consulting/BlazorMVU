namespace BlazorMVU;

/// <summary>
/// Represents a subscription that listens for external events and produces messages.
/// Subscriptions are active listeners that run for the lifetime of the component.
/// </summary>
/// <typeparam name="TMsg">The type of message this subscription can produce.</typeparam>
public abstract record Sub<TMsg>
{
    /// <summary>
    /// No subscription.
    /// </summary>
    public sealed record None : Sub<TMsg>;

    /// <summary>
    /// A batch of subscriptions that all run concurrently.
    /// </summary>
    /// <param name="Subscriptions">The subscriptions to run.</param>
    public sealed record Batch(IReadOnlyList<Sub<TMsg>> Subscriptions) : Sub<TMsg>;

    /// <summary>
    /// A timer subscription that fires at regular intervals.
    /// </summary>
    /// <param name="Interval">The interval between ticks.</param>
    /// <param name="ToMsg">Function to convert the tick time to a message.</param>
    /// <param name="Id">Optional unique identifier for this subscription.</param>
    public sealed record Timer(TimeSpan Interval, Func<DateTime, TMsg> ToMsg, string? Id = null) : Sub<TMsg>;

    /// <summary>
    /// A one-time timer that fires after a delay.
    /// </summary>
    /// <param name="Delay">The delay before firing.</param>
    /// <param name="Message">The message to dispatch.</param>
    /// <param name="Id">Optional unique identifier for this subscription.</param>
    public sealed record Timeout(TimeSpan Delay, TMsg Message, string? Id = null) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser window resize events.
    /// </summary>
    /// <param name="ToMsg">Function to convert window dimensions to a message.</param>
    public sealed record OnWindowResize(Func<int, int, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser keyboard events.
    /// </summary>
    /// <param name="ToMsg">Function to convert key info to a message.</param>
    public sealed record OnKeyDown(Func<string, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser keyboard up events.
    /// </summary>
    /// <param name="ToMsg">Function to convert key info to a message.</param>
    public sealed record OnKeyUp(Func<string, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser mouse move events.
    /// </summary>
    /// <param name="ToMsg">Function to convert mouse position to a message.</param>
    public sealed record OnMouseMove(Func<double, double, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser visibility change events.
    /// </summary>
    /// <param name="ToMsg">Function to convert visibility state to a message.</param>
    public sealed record OnVisibilityChange(Func<bool, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A subscription to browser online/offline events.
    /// </summary>
    /// <param name="ToMsg">Function to convert online state to a message.</param>
    public sealed record OnOnlineStatusChange(Func<bool, TMsg> ToMsg) : Sub<TMsg>;

    /// <summary>
    /// A custom subscription with user-provided setup and teardown logic.
    /// </summary>
    /// <param name="Subscribe">Function that sets up the subscription and returns a dispose action.</param>
    /// <param name="Id">Unique identifier for this subscription.</param>
    public sealed record Custom(
        Func<Action<TMsg>, CancellationToken, IDisposable> Subscribe,
        string Id) : Sub<TMsg>;

    private Sub() { }
}

/// <summary>
/// Helper methods for creating subscriptions.
/// </summary>
public static class Sub
{
    /// <summary>
    /// Creates an empty subscription.
    /// </summary>
    public static Sub<TMsg> None<TMsg>() => new Sub<TMsg>.None();

    /// <summary>
    /// Creates a batch of subscriptions.
    /// </summary>
    public static Sub<TMsg> Batch<TMsg>(params Sub<TMsg>[] subscriptions) =>
        new Sub<TMsg>.Batch(subscriptions);

    /// <summary>
    /// Creates a batch of subscriptions.
    /// </summary>
    public static Sub<TMsg> Batch<TMsg>(IEnumerable<Sub<TMsg>> subscriptions) =>
        new Sub<TMsg>.Batch(subscriptions.ToList());

    /// <summary>
    /// Creates a timer subscription.
    /// </summary>
    public static Sub<TMsg> Timer<TMsg>(TimeSpan interval, Func<DateTime, TMsg> toMsg, string? id = null) =>
        new Sub<TMsg>.Timer(interval, toMsg, id);

    /// <summary>
    /// Creates a timer subscription that dispatches a fixed message.
    /// </summary>
    public static Sub<TMsg> Every<TMsg>(TimeSpan interval, TMsg msg, string? id = null) =>
        new Sub<TMsg>.Timer(interval, _ => msg, id);

    /// <summary>
    /// Creates a one-time timeout subscription.
    /// </summary>
    public static Sub<TMsg> Timeout<TMsg>(TimeSpan delay, TMsg msg, string? id = null) =>
        new Sub<TMsg>.Timeout(delay, msg, id);

    /// <summary>
    /// Creates a window resize subscription.
    /// </summary>
    public static Sub<TMsg> OnWindowResize<TMsg>(Func<int, int, TMsg> toMsg) =>
        new Sub<TMsg>.OnWindowResize(toMsg);

    /// <summary>
    /// Creates a key down subscription.
    /// </summary>
    public static Sub<TMsg> OnKeyDown<TMsg>(Func<string, TMsg> toMsg) =>
        new Sub<TMsg>.OnKeyDown(toMsg);

    /// <summary>
    /// Creates a key up subscription.
    /// </summary>
    public static Sub<TMsg> OnKeyUp<TMsg>(Func<string, TMsg> toMsg) =>
        new Sub<TMsg>.OnKeyUp(toMsg);

    /// <summary>
    /// Creates a mouse move subscription.
    /// </summary>
    public static Sub<TMsg> OnMouseMove<TMsg>(Func<double, double, TMsg> toMsg) =>
        new Sub<TMsg>.OnMouseMove(toMsg);

    /// <summary>
    /// Creates a visibility change subscription.
    /// </summary>
    public static Sub<TMsg> OnVisibilityChange<TMsg>(Func<bool, TMsg> toMsg) =>
        new Sub<TMsg>.OnVisibilityChange(toMsg);

    /// <summary>
    /// Creates an online status change subscription.
    /// </summary>
    public static Sub<TMsg> OnOnlineStatusChange<TMsg>(Func<bool, TMsg> toMsg) =>
        new Sub<TMsg>.OnOnlineStatusChange(toMsg);

    /// <summary>
    /// Creates a custom subscription.
    /// </summary>
    public static Sub<TMsg> Custom<TMsg>(
        Func<Action<TMsg>, CancellationToken, IDisposable> subscribe,
        string id) =>
        new Sub<TMsg>.Custom(subscribe, id);
}
