using Microsoft.AspNetCore.Components;

namespace BlazorMVU;

/// <summary>
/// Base class for MVU (Model-View-Update) components in Blazor.
/// Provides the core MVU pattern with support for commands, subscriptions,
/// middleware, and time-travel debugging.
/// </summary>
/// <typeparam name="TModel">The type of the component's state model.</typeparam>
/// <typeparam name="TMsg">The type of messages that can be dispatched.</typeparam>
public abstract class MvuComponent<TModel, TMsg> : ComponentBase, IAsyncDisposable
    where TModel : notnull
{
    private readonly List<IDisposable> _subscriptionDisposables = [];
    private readonly List<MvuMiddleware<TModel, TMsg>> _middlewares = [];
    private CancellationTokenSource _cts = new();
    private TimeTravelDebugger<TModel, TMsg>? _debugger;
    private bool _disposed;

    /// <summary>
    /// Gets or sets the current state of the component.
    /// </summary>
    public required TModel State { get; set; }

    /// <summary>
    /// Gets the time-travel debugger for this component.
    /// Only available when time-travel debugging is enabled.
    /// </summary>
    public TimeTravelDebugger<TModel, TMsg>? Debugger => _debugger;

    /// <summary>
    /// Gets or sets whether time-travel debugging is enabled.
    /// </summary>
    [Parameter]
    public bool EnableTimeTravel { get; set; }

    /// <summary>
    /// Gets or sets the maximum history size for time-travel debugging.
    /// </summary>
    [Parameter]
    public int TimeTravelMaxHistory { get; set; } = 100;

    /// <summary>
    /// Initializes the model with its initial state.
    /// </summary>
    /// <returns>The initial model state.</returns>
    protected abstract TModel Init();

    /// <summary>
    /// Initializes the model and returns an optional command to execute.
    /// Override this instead of Init() when you need to execute commands on initialization.
    /// </summary>
    /// <returns>A tuple of the initial model and command to execute.</returns>
    protected virtual (TModel Model, Cmd<TMsg> Cmd) InitWithCmd()
        => (Init(), Cmd.None<TMsg>());

    /// <summary>
    /// Updates the model based on a message.
    /// </summary>
    /// <param name="msg">The message to process.</param>
    /// <param name="model">The current model state.</param>
    /// <returns>The new model state.</returns>
    protected abstract TModel Update(TMsg msg, TModel model);

    /// <summary>
    /// Updates the model based on a message and returns an optional command.
    /// Override this instead of Update() when you need to execute commands.
    /// </summary>
    /// <param name="msg">The message to process.</param>
    /// <param name="model">The current model state.</param>
    /// <returns>A tuple of the new model state and command to execute.</returns>
    protected virtual (TModel Model, Cmd<TMsg> Cmd) UpdateWithCmd(TMsg msg, TModel model)
        => (Update(msg, model), Cmd.None<TMsg>());

    /// <summary>
    /// Returns the subscriptions for this component based on the current model.
    /// Override this to set up subscriptions (timers, events, etc.).
    /// </summary>
    /// <param name="model">The current model state.</param>
    /// <returns>The subscriptions to activate.</returns>
    protected virtual Sub<TMsg> Subscriptions(TModel model) => Sub.None<TMsg>();

    /// <summary>
    /// Dispatches a message to update the component state.
    /// </summary>
    /// <param name="msg">The message to dispatch.</param>
    protected virtual void Dispatch(TMsg msg)
    {
        if (_disposed) return;

        var previousModel = State;
        var context = new DispatchContext<TModel, TMsg>(msg, State, previousModel);

        // Run middleware pipeline
        _ = RunMiddlewarePipelineAsync(context, () =>
        {
            // Core update logic
            var (newModel, cmd) = UpdateWithCmd(msg, State);
            State = newModel;

            // Record to debugger if enabled
            _debugger?.RecordState(State, msg);

            // Re-render
            StateHasChanged();

            // Execute commands
            _ = ExecuteCommandAsync(cmd);

            // Update subscriptions if model changed
            if (!EqualityComparer<TModel>.Default.Equals(previousModel, State))
            {
                UpdateSubscriptions();
            }

            return Task.CompletedTask;
        });
    }

    /// <summary>
    /// Dispatches a message asynchronously.
    /// </summary>
    /// <param name="msg">The message to dispatch.</param>
    protected Task DispatchAsync(TMsg msg)
    {
        Dispatch(msg);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Adds middleware to the dispatch pipeline.
    /// Call this in OnInitialized before base.OnInitialized().
    /// </summary>
    protected void UseMiddleware(MvuMiddleware<TModel, TMsg> middleware)
    {
        _middlewares.Add(middleware);
    }

    /// <summary>
    /// Adds multiple middleware to the dispatch pipeline.
    /// </summary>
    protected void UseMiddleware(params MvuMiddleware<TModel, TMsg>[] middlewares)
    {
        _middlewares.AddRange(middlewares);
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        // Initialize debugger if enabled
        if (EnableTimeTravel)
        {
            _debugger = new TimeTravelDebugger<TModel, TMsg>(TimeTravelMaxHistory);
        }

        // Initialize model
        var (model, cmd) = InitWithCmd();
        State = model;

        // Record initial state
        _debugger?.RecordInitialState(State);

        // Execute initial command
        _ = ExecuteCommandAsync(cmd);

        // Set up subscriptions
        UpdateSubscriptions();
    }

    /// <summary>
    /// Restores the state from the time-travel debugger.
    /// Call this after navigating through history.
    /// </summary>
    protected void RestoreFromDebugger()
    {
        if (_debugger?.CurrentState is { } state)
        {
            State = state;
            StateHasChanged();
        }
    }

    private async Task RunMiddlewarePipelineAsync(
        DispatchContext<TModel, TMsg> context,
        Func<Task> core)
    {
        if (_middlewares.Count == 0)
        {
            await core();
            return;
        }

        async Task RunPipeline(int index)
        {
            if (index >= _middlewares.Count)
            {
                await core();
            }
            else
            {
                await _middlewares[index](context, () => RunPipeline(index + 1));
            }
        }

        await RunPipeline(0);
    }

    private async Task ExecuteCommandAsync(Cmd<TMsg> cmd)
    {
        switch (cmd)
        {
            case Cmd<TMsg>.None:
                break;

            case Cmd<TMsg>.Batch batch:
                foreach (var c in batch.Commands)
                {
                    await ExecuteCommandAsync(c);
                }
                break;

            case Cmd<TMsg>.OfMsg ofMsg:
                Dispatch(ofMsg.Message);
                break;

            case Cmd<TMsg>.OfFunc ofFunc:
                Dispatch(ofFunc.Func());
                break;

            case Cmd<TMsg>.OfTask ofTask:
                try
                {
                    var result = await ofTask.Task(_cts.Token);
                    Dispatch(result);
                }
                catch (OperationCanceledException)
                {
                    // Cancelled, don't dispatch
                }
                break;

            case Cmd<TMsg>.OfTaskUnit ofTaskUnit:
                try
                {
                    await ofTaskUnit.Task(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Cancelled
                }
                break;

            case Cmd<TMsg>.OfEffect ofEffect:
                ofEffect.Effect();
                break;

            case Cmd<TMsg>.Delay delay:
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(delay.Delay, _cts.Token);
                        await ExecuteCommandAsync(delay.Command);
                    }
                    catch (OperationCanceledException)
                    {
                        // Cancelled
                    }
                }, _cts.Token);
                break;
        }
    }

    private void UpdateSubscriptions()
    {
        // Dispose existing subscriptions
        foreach (var disposable in _subscriptionDisposables)
        {
            disposable.Dispose();
        }
        _subscriptionDisposables.Clear();

        // Set up new subscriptions
        var subs = Subscriptions(State);
        SetupSubscription(subs);
    }

    private void SetupSubscription(Sub<TMsg> sub)
    {
        switch (sub)
        {
            case Sub<TMsg>.None:
                break;

            case Sub<TMsg>.Batch batch:
                foreach (var s in batch.Subscriptions)
                {
                    SetupSubscription(s);
                }
                break;

            case Sub<TMsg>.Timer timer:
                var timerDisposable = new System.Timers.Timer(timer.Interval.TotalMilliseconds);
                timerDisposable.Elapsed += (_, _) =>
                {
                    var msg = timer.ToMsg(DateTime.UtcNow);
                    InvokeAsync(() => Dispatch(msg));
                };
                timerDisposable.AutoReset = true;
                timerDisposable.Start();
                _subscriptionDisposables.Add(timerDisposable);
                break;

            case Sub<TMsg>.Timeout timeout:
                var timeoutCts = new CancellationTokenSource();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await Task.Delay(timeout.Delay, timeoutCts.Token);
                        await InvokeAsync(() => Dispatch(timeout.Message));
                    }
                    catch (OperationCanceledException)
                    {
                        // Cancelled
                    }
                }, timeoutCts.Token);
                _subscriptionDisposables.Add(new CancellationDisposable(timeoutCts));
                break;

            case Sub<TMsg>.Custom custom:
                var customDisposable = custom.Subscribe(
                    msg => InvokeAsync(() => Dispatch(msg)),
                    _cts.Token);
                _subscriptionDisposables.Add(customDisposable);
                break;

            // Browser event subscriptions would require JS interop
            // These are documented but implementation depends on the specific use case
            case Sub<TMsg>.OnWindowResize:
            case Sub<TMsg>.OnKeyDown:
            case Sub<TMsg>.OnKeyUp:
            case Sub<TMsg>.OnMouseMove:
            case Sub<TMsg>.OnVisibilityChange:
            case Sub<TMsg>.OnOnlineStatusChange:
                // These require JS interop setup - see documentation
                break;
        }
    }

    /// <inheritdoc/>
    public virtual async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        // Cancel all operations
        await _cts.CancelAsync();
        _cts.Dispose();

        // Dispose all subscriptions
        foreach (var disposable in _subscriptionDisposables)
        {
            disposable.Dispose();
        }
        _subscriptionDisposables.Clear();

        GC.SuppressFinalize(this);
    }

    private sealed class CancellationDisposable(CancellationTokenSource cts) : IDisposable
    {
        public void Dispose() => cts.Cancel();
    }
}

/// <summary>
/// A simpler MVU component base class without command support.
/// Use this for components that don't need side effects management.
/// </summary>
/// <typeparam name="TModel">The type of the component's state model.</typeparam>
/// <typeparam name="TMsg">The type of messages that can be dispatched.</typeparam>
public abstract class SimpleMvuComponent<TModel, TMsg> : ComponentBase
    where TModel : notnull
{
    /// <summary>
    /// Gets or sets the current state of the component.
    /// </summary>
    public required TModel State { get; set; }

    /// <summary>
    /// Initializes the model with its initial state.
    /// </summary>
    protected abstract TModel Init();

    /// <summary>
    /// Updates the model based on a message.
    /// </summary>
    protected abstract TModel Update(TMsg msg, TModel model);

    /// <summary>
    /// Dispatches a message to update the component state.
    /// </summary>
    protected virtual void Dispatch(TMsg msg)
    {
        State = Update(msg, State);
        StateHasChanged();
    }

    /// <inheritdoc/>
    protected override void OnInitialized()
    {
        State = Init();
    }
}
