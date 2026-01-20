using System.Collections.Immutable;
using BlazorMVU;

namespace BlazorMVU.Demo.Components;

/// <summary>
/// A stopwatch component demonstrating MVU subscriptions.
/// Uses timer subscriptions for accurate time tracking.
/// </summary>
public partial class MvuStopwatch
{
    // Domain types
    public record LapRecord(TimeSpan LapTime, TimeSpan SplitTime);

    // Model
    public record Model(
        TimeSpan Elapsed,
        DateTime? StartedAt,
        bool IsRunning,
        ImmutableList<LapRecord> Laps,
        TimeSpan LastLapTime);

    // Messages
    public abstract record Msg
    {
        public record Start : Msg;
        public record Stop : Msg;
        public record Reset : Msg;
        public record Lap : Msg;
        public record Tick(DateTime Now) : Msg;
    }

    protected override Model Init() => new(
        Elapsed: TimeSpan.Zero,
        StartedAt: null,
        IsRunning: false,
        Laps: [],
        LastLapTime: TimeSpan.Zero);

    // Subscriptions - only tick when running
    protected override Sub<Msg> Subscriptions(Model model)
    {
        if (model.IsRunning)
        {
            // Tick every 10ms for smooth display
            return Sub.Timer<Msg>(
                TimeSpan.FromMilliseconds(10),
                now => new Msg.Tick(now),
                "stopwatch-tick");
        }
        return Sub.None<Msg>();
    }

    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.Start => model with
            {
                IsRunning = true,
                StartedAt = DateTime.UtcNow
            },

            Msg.Stop when model.StartedAt.HasValue => model with
            {
                IsRunning = false,
                Elapsed = model.Elapsed + (DateTime.UtcNow - model.StartedAt.Value),
                StartedAt = null
            },

            Msg.Reset => model with
            {
                IsRunning = false,
                Elapsed = TimeSpan.Zero,
                StartedAt = null,
                Laps = [],
                LastLapTime = TimeSpan.Zero
            },

            Msg.Lap when model.IsRunning && model.StartedAt.HasValue =>
                RecordLap(model),

            Msg.Tick tick when model.StartedAt.HasValue => model with
            {
                Elapsed = model.Elapsed + (tick.Now - model.StartedAt.Value),
                StartedAt = tick.Now
            },

            _ => model
        };

    private static Model RecordLap(Model model)
    {
        var currentElapsed = model.Elapsed + (DateTime.UtcNow - model.StartedAt!.Value);
        var splitTime = currentElapsed - model.LastLapTime;

        return model with
        {
            Laps = model.Laps.Add(new LapRecord(currentElapsed, splitTime)),
            LastLapTime = currentElapsed
        };
    }
}
