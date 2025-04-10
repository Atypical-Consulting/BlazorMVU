using BlazorMVU.Demo.Components;
using BlazorMVU.Demo.Shared;

namespace BlazorMVU.Tests.Components;

public class MvuFetchRandomErrorTests : TestContext
{
    [Fact]
    public async Task MvuFetchRandomError_Success()
    {
        // Arrange
        var cut = RenderComponent<MvuFetchRandomError>();

        // Wait a bit for the async method to finish
        await Task.Delay(10, Xunit.TestContext.Current.CancellationToken);

        // Assert
        cut.Instance.State
            .ButtonLabel.ShouldBe("Fetch Weather");
    }

    [Fact]
    public Task MvuFetchRandomError_Loading()
    {
        // Arrange
        var cut = RenderComponent<MvuFetchRandomError>();

        // Assert
        cut.Instance.State
            .ButtonLabel.ShouldBe("Fetch Weather");

        return Task.CompletedTask;
    }
}