using BlazorMVU.Demo.Components;
using BlazorMVU.Demo.Shared;

namespace BlazorMVU.Tests.Components;

// ReSharper disable once ClassNeverInstantiated.Global
public class TestMvuFetchText : MvuFetchText
{
    public new void Dispatch(Msg msg)
        => base.Dispatch(msg);
}

public class MvuFetchTextTests : TestContext
{
    [Fact]
    public async Task MvuFetchText_Success()
    {
        // Arrange
        var cut = RenderComponent<MvuFetchText>(parameters => parameters.Add(p => p.FetchDelay, 1));

        // Wait a bit for the async method to finish
        await Task.Delay(10);

        // Assert
        cut.Instance.State
            .Message.ShouldBe("This is the fetched text.");
    }

    [Fact]
    public Task MvuFetchText_Loading()
    {
        // Arrange
        var cut = RenderComponent<MvuFetchText>(parameters => parameters.Add(p => p.FetchDelay, int.MaxValue));

        // Assert
        cut.Instance.State
            .Message.ShouldBe("Loading...");

        return Task.CompletedTask;
    }
    
    [Fact]
    public async Task MvuFetchText_Failure()
    {
        // Arrange
        var cut = RenderComponent<TestMvuFetchText>(parameters => parameters.Add(p => p.FetchDelay, -1));

        // Act
        await cut.InvokeAsync(() =>
        {
            var failure = MvuResult<string>.Failure(new Exception());
            var msg = new MvuFetchText.Msg.GotText(failure);
            cut.Instance.Dispatch(msg);
        });
        
        // Assert
        cut.Instance.State
            .Message.ShouldBe("I was unable to load your book.");
    }
}