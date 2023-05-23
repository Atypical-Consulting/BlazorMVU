using BlazorMVU.Demo.Components;
using BlazorMVU.Demo.Shared;

namespace BlazorMVU.Tests.Components;

public class TestMvuFetchText : MvuFetchText
{
    public new void Dispatch(Msg msg)
        => base.Dispatch(msg);
}

public class MvuFetchTextTests : TestContext
{
    [Fact]
    public void FetchText_ShowsLoadingInitially()
    {
        // Arrange
        var cut = RenderComponent<TestMvuFetchText>();

        // Assert
        cut.MarkupMatches("<h3>Fetch Text</h3><p>Loading...</p>");
    }

    [Fact]
    public async Task FetchText_ShowsErrorOnFailure()
    {
        // Arrange
        var cut = RenderComponent<TestMvuFetchText>();

        // Act
        await cut.InvokeAsync(() =>
        {
            var failure = MvuResult<string>.Failure(new Exception());
            var msg = new MvuFetchText.Msg.GotText(failure);
            cut.Instance.Dispatch(msg);
        });

        // Assert
        cut.MarkupMatches("<h3>Fetch Text</h3><p>I was unable to load your book.</p>");
    }

    // [Fact]
    // public async Task FetchText_ShowsSuccessOnSuccess()
    // {
    //     // Arrange
    //     var cut = RenderComponent<TestMvuFetchText>();
    //
    //     // Act
    //     await cut.InvokeAsync(() =>
    //     {
    //         var success = MvuResult<string>.Success("Fetched text");
    //         var msg = new MvuFetchText.Msg.GotText(success);
    //         cut.Instance.Dispatch(msg);
    //     });
    //
    //     // Assert
    //     cut.MarkupMatches("<h3>Fetch Text</h3><pre>Fetched text</pre>");
    // }
}