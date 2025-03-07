using BlazorMVU.Demo.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Components;

public partial class MvuFetchText
{
    [Parameter]
    public RenderFragment<string>? ChildContent { get; set; }

    [Parameter]
    public int FetchDelay { get; set; } = 1000;

    // Model
    public abstract record Model
    {
        public record Failure : Model;

        public record Loading : Model;

        public record Success(string FullText) : Model;

        public string Message
            => this switch
            {
                Failure => "I was unable to load your book.",
                Loading => "Loading...",
                Success success => success.FullText,
                _ => ""
            };
    }

    // Messages
    public abstract record Msg
    {
        public record GotText(MvuResult<string> Result) : Msg;
    }

    // Initialize the model
    protected override Model Init()
    {
#pragma warning disable CS4014
        FetchText(); // <-- We don't await this, so it's fire and forget
#pragma warning restore CS4014
        return new Model.Loading();
    }

    // Update the model based on the message
    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.GotText gotText => gotText.Result.IsSuccess
                ? new Model.Success(gotText.Result.Value ?? "")
                : new Model.Failure(),
            _ => model
        };

    // Simulate fetching text and dispatch a message
    private async Task FetchText()
    {
        try
        {
            await Task.Delay(FetchDelay); // Simulate API call delay
            const string fullText = "This is the fetched text."; // Simulate received data
            var msg = new Msg.GotText(MvuResult<string>.Success(fullText));
            Dispatch(msg);
        }
        catch (Exception? ex)
        {
            var msg = new Msg.GotText(MvuResult<string>.Failure(ex));
            Dispatch(msg);
        }
    }
}