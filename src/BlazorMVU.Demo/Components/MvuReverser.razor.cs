using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Components;

public partial class MvuReverser
{
    // Model
    public record Model(string Content)
    {
        public string Reverse
            => new(Content.Reverse().ToArray());
    }

    // Messages
    public abstract record Msg
    {
        public record Change(string Content) : Msg;
    }

    // Initialize the model
    protected override Model Init()
        => new("");

    // Update the model based on the message
    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.Change change => new Model(change.Content),
            _ => model
        };

    // Handle the text change and dispatch a message
    private void HandleTextChange(ChangeEventArgs obj)
    {
        var content = obj.Value?.ToString() ?? "";
        var msg = new Msg.Change(content);
        Dispatch(msg);
    }
}