using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Components;

/// <summary>
/// A child counter component that demonstrates parent-child communication in MVU.
/// It has its own internal state but can notify the parent of important changes.
/// </summary>
public partial class MvuCounterChild
{
    // Model
    public record Model(int Count);

    // Messages
    public abstract record Msg
    {
        public record Increment : Msg;
        public record Decrement : Msg;
        public record SetValue(int Value) : Msg;
    }

    /// <summary>
    /// Label to display for this counter.
    /// </summary>
    [Parameter]
    public string Label { get; set; } = "Counter";

    /// <summary>
    /// Initial count value.
    /// </summary>
    [Parameter]
    public int InitialCount { get; set; }

    /// <summary>
    /// Event callback when the count changes and is sent to parent.
    /// </summary>
    [Parameter]
    public EventCallback<int> OnCountChanged { get; set; }

    /// <summary>
    /// Optional external value from parent. When set, updates the internal count.
    /// </summary>
    [Parameter]
    public int? ExternalValue { get; set; }

    private int? _previousExternalValue;

    protected override Model Init() => new(InitialCount);

    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.Increment => model with { Count = model.Count + 1 },
            Msg.Decrement => model with { Count = model.Count - 1 },
            Msg.SetValue set => model with { Count = set.Value },
            _ => model
        };

    protected override void OnParametersSet()
    {
        // React to external value changes from parent
        if (ExternalValue.HasValue && ExternalValue != _previousExternalValue)
        {
            _previousExternalValue = ExternalValue;
            Dispatch(new Msg.SetValue(ExternalValue.Value));
        }
    }
}
