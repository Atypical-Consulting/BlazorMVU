namespace BlazorMVU;

/// <summary>
/// Represents the context passed through the middleware pipeline.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TMsg">The message type.</typeparam>
public record DispatchContext<TModel, TMsg>(
    TMsg Message,
    TModel CurrentModel,
    TModel? PreviousModel = default);

/// <summary>
/// Delegate for the next middleware in the pipeline.
/// </summary>
public delegate Task MiddlewareNext();

/// <summary>
/// Middleware delegate that can intercept and modify dispatch behavior.
/// </summary>
/// <typeparam name="TModel">The model type.</typeparam>
/// <typeparam name="TMsg">The message type.</typeparam>
public delegate Task MvuMiddleware<TModel, TMsg>(
    DispatchContext<TModel, TMsg> context,
    MiddlewareNext next);

/// <summary>
/// Built-in middleware implementations.
/// </summary>
public static class Middleware
{
    /// <summary>
    /// Creates a logging middleware that logs all dispatched messages.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Logger<TModel, TMsg>(
        Action<string> log,
        Func<TMsg, string>? messageFormatter = null,
        Func<TModel, string>? modelFormatter = null)
    {
        return async (context, next) =>
        {
            var msgStr = messageFormatter?.Invoke(context.Message) ?? context.Message?.ToString() ?? "null";
            var modelStr = modelFormatter?.Invoke(context.CurrentModel) ?? context.CurrentModel?.ToString() ?? "null";

            log($"[MVU] Dispatching: {msgStr}");
            log($"[MVU] Current state: {modelStr}");

            await next();

            log($"[MVU] Dispatch complete");
        };
    }

    /// <summary>
    /// Creates a middleware that logs messages to the console.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> ConsoleLogger<TModel, TMsg>() =>
        Logger<TModel, TMsg>(Console.WriteLine);

    /// <summary>
    /// Creates a middleware that measures dispatch time.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Timing<TModel, TMsg>(
        Action<TMsg, TimeSpan> onTimed)
    {
        return async (context, next) =>
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            await next();
            sw.Stop();
            onTimed(context.Message, sw.Elapsed);
        };
    }

    /// <summary>
    /// Creates a middleware that filters messages based on a predicate.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Filter<TModel, TMsg>(
        Func<TMsg, TModel, bool> predicate)
    {
        return async (context, next) =>
        {
            if (predicate(context.Message, context.CurrentModel))
            {
                await next();
            }
        };
    }

    /// <summary>
    /// Creates a middleware that catches and handles exceptions.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> ErrorHandler<TModel, TMsg>(
        Action<Exception, TMsg> onError)
    {
        return async (context, next) =>
        {
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                onError(ex, context.Message);
            }
        };
    }

    /// <summary>
    /// Creates a middleware that debounces rapid message dispatches.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Debounce<TModel, TMsg>(
        TimeSpan delay,
        Func<TMsg, bool>? shouldDebounce = null)
    {
        DateTime lastDispatch = DateTime.MinValue;

        return async (context, next) =>
        {
            if (shouldDebounce != null && !shouldDebounce(context.Message))
            {
                await next();
                return;
            }

            var now = DateTime.UtcNow;
            if (now - lastDispatch >= delay)
            {
                lastDispatch = now;
                await next();
            }
        };
    }

    /// <summary>
    /// Creates a middleware that throttles message dispatches.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Throttle<TModel, TMsg>(
        TimeSpan interval,
        Func<TMsg, bool>? shouldThrottle = null)
    {
        DateTime lastDispatch = DateTime.MinValue;
        readonly object lockObj = new();

        return async (context, next) =>
        {
            if (shouldThrottle != null && !shouldThrottle(context.Message))
            {
                await next();
                return;
            }

            var now = DateTime.UtcNow;
            bool shouldProceed;

            lock (lockObj)
            {
                shouldProceed = now - lastDispatch >= interval;
                if (shouldProceed)
                {
                    lastDispatch = now;
                }
            }

            if (shouldProceed)
            {
                await next();
            }
        };
    }

    /// <summary>
    /// Combines multiple middleware into a single middleware.
    /// </summary>
    public static MvuMiddleware<TModel, TMsg> Combine<TModel, TMsg>(
        params MvuMiddleware<TModel, TMsg>[] middlewares)
    {
        return async (context, next) =>
        {
            async Task RunPipeline(int index)
            {
                if (index >= middlewares.Length)
                {
                    await next();
                }
                else
                {
                    await middlewares[index](context, () => RunPipeline(index + 1));
                }
            }

            await RunPipeline(0);
        };
    }
}
