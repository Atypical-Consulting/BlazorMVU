using System.Collections.Immutable;

namespace BlazorMVU.Demo.Components;

/// <summary>
/// A parent component demonstrating MVU parent-child communication patterns.
/// Shows how to:
/// - Receive messages from child components via EventCallback
/// - Broadcast state to children via Parameters
/// - Maintain aggregated state from multiple children
/// </summary>
public partial class MvuParentChild
{
    // Model
    public record Model(
        int TotalFromChildren,
        string? LastChildSource,
        ImmutableList<string> History,
        int? BroadcastValue);

    // Messages
    public abstract record Msg
    {
        /// <summary>
        /// Received when a child counter sends its value to the parent.
        /// </summary>
        public record ChildCountChanged(string ChildId, int Value) : Msg;

        /// <summary>
        /// Broadcasts a value to all child counters.
        /// </summary>
        public record BroadcastToChildren(int Value) : Msg;

        /// <summary>
        /// Clears the broadcast value (reset after sending).
        /// </summary>
        public record ClearBroadcast : Msg;

        /// <summary>
        /// Clears the message history.
        /// </summary>
        public record ClearHistory : Msg;
    }

    protected override Model Init() => new(
        TotalFromChildren: 0,
        LastChildSource: null,
        History: [],
        BroadcastValue: null);

    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.ChildCountChanged changed => model with
            {
                TotalFromChildren = model.TotalFromChildren + changed.Value,
                LastChildSource = changed.ChildId,
                History = model.History.Add($"[{DateTime.Now:HH:mm:ss}] Child {changed.ChildId} sent: {changed.Value}")
            },

            Msg.BroadcastToChildren broadcast => model with
            {
                BroadcastValue = broadcast.Value,
                History = model.History.Add($"[{DateTime.Now:HH:mm:ss}] Parent broadcast: {broadcast.Value}")
            },

            Msg.ClearBroadcast => model with { BroadcastValue = null },

            Msg.ClearHistory => model with { History = [] },

            _ => model
        };
}
