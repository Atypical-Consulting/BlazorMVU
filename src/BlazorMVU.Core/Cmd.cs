namespace BlazorMVU;

/// <summary>
/// Represents a command that produces side effects and may dispatch messages.
/// Commands are the functional way to handle side effects in the MVU pattern.
/// </summary>
/// <typeparam name="TMsg">The type of message this command can produce.</typeparam>
public abstract record Cmd<TMsg>
{
    /// <summary>
    /// A command that does nothing.
    /// </summary>
    public sealed record None : Cmd<TMsg>;

    /// <summary>
    /// A command that batches multiple commands together.
    /// All commands in the batch will be executed.
    /// </summary>
    /// <param name="Commands">The commands to execute.</param>
    public sealed record Batch(IReadOnlyList<Cmd<TMsg>> Commands) : Cmd<TMsg>;

    /// <summary>
    /// A command that executes an asynchronous task and dispatches the resulting message.
    /// </summary>
    /// <param name="Task">The async task that produces a message.</param>
    public sealed record OfTask(Func<CancellationToken, Task<TMsg>> Task) : Cmd<TMsg>;

    /// <summary>
    /// A command that executes an asynchronous task that may or may not produce a message.
    /// </summary>
    /// <param name="Task">The async task that optionally produces a message.</param>
    public sealed record OfTaskOption(Func<CancellationToken, Task<TMsg?>> Task) : Cmd<TMsg> where TMsg : class;

    /// <summary>
    /// A command that executes a fire-and-forget async operation.
    /// </summary>
    /// <param name="Task">The async task to execute.</param>
    public sealed record OfTaskUnit(Func<CancellationToken, Task> Task) : Cmd<TMsg>;

    /// <summary>
    /// A command that immediately dispatches a message.
    /// </summary>
    /// <param name="Message">The message to dispatch.</param>
    public sealed record OfMsg(TMsg Message) : Cmd<TMsg>;

    /// <summary>
    /// A command that executes a synchronous function and dispatches the resulting message.
    /// </summary>
    /// <param name="Func">The function that produces a message.</param>
    public sealed record OfFunc(Func<TMsg> Func) : Cmd<TMsg>;

    /// <summary>
    /// A command that executes a side effect without producing a message.
    /// </summary>
    /// <param name="Effect">The side effect to execute.</param>
    public sealed record OfEffect(Action Effect) : Cmd<TMsg>;

    /// <summary>
    /// A command that delays execution of another command.
    /// </summary>
    /// <param name="Delay">The delay duration.</param>
    /// <param name="Command">The command to execute after the delay.</param>
    public sealed record Delay(TimeSpan Delay, Cmd<TMsg> Command) : Cmd<TMsg>;

    private Cmd() { }
}

/// <summary>
/// Helper methods for creating commands.
/// </summary>
public static class Cmd
{
    /// <summary>
    /// Creates a command that does nothing.
    /// </summary>
    public static Cmd<TMsg> None<TMsg>() => new Cmd<TMsg>.None();

    /// <summary>
    /// Creates a command that batches multiple commands together.
    /// </summary>
    public static Cmd<TMsg> Batch<TMsg>(params Cmd<TMsg>[] commands) =>
        new Cmd<TMsg>.Batch(commands);

    /// <summary>
    /// Creates a command that batches multiple commands together.
    /// </summary>
    public static Cmd<TMsg> Batch<TMsg>(IEnumerable<Cmd<TMsg>> commands) =>
        new Cmd<TMsg>.Batch(commands.ToList());

    /// <summary>
    /// Creates a command from an async task.
    /// </summary>
    public static Cmd<TMsg> OfTask<TMsg>(Func<CancellationToken, Task<TMsg>> task) =>
        new Cmd<TMsg>.OfTask(task);

    /// <summary>
    /// Creates a command from an async task (without cancellation token).
    /// </summary>
    public static Cmd<TMsg> OfTask<TMsg>(Func<Task<TMsg>> task) =>
        new Cmd<TMsg>.OfTask(_ => task());

    /// <summary>
    /// Creates a command from a fire-and-forget async task.
    /// </summary>
    public static Cmd<TMsg> OfTaskUnit<TMsg>(Func<CancellationToken, Task> task) =>
        new Cmd<TMsg>.OfTaskUnit(task);

    /// <summary>
    /// Creates a command that immediately dispatches a message.
    /// </summary>
    public static Cmd<TMsg> OfMsg<TMsg>(TMsg msg) =>
        new Cmd<TMsg>.OfMsg(msg);

    /// <summary>
    /// Creates a command from a synchronous function.
    /// </summary>
    public static Cmd<TMsg> OfFunc<TMsg>(Func<TMsg> func) =>
        new Cmd<TMsg>.OfFunc(func);

    /// <summary>
    /// Creates a command that executes a side effect without producing a message.
    /// </summary>
    public static Cmd<TMsg> OfEffect<TMsg>(Action effect) =>
        new Cmd<TMsg>.OfEffect(effect);

    /// <summary>
    /// Creates a command that delays another command.
    /// </summary>
    public static Cmd<TMsg> Delay<TMsg>(TimeSpan delay, Cmd<TMsg> command) =>
        new Cmd<TMsg>.Delay(delay, command);

    /// <summary>
    /// Creates a command that delays a message dispatch.
    /// </summary>
    public static Cmd<TMsg> DelayMsg<TMsg>(TimeSpan delay, TMsg msg) =>
        new Cmd<TMsg>.Delay(delay, OfMsg(msg));
}
