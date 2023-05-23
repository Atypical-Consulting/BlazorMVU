using BlazorMVU.Demo.Components;

namespace BlazorMVU.Tests.Components;

public class MvuCounterTests : TestContext
{
    [Fact]
    public void MvuCounter_InitialValueIsZero()
    {
        // Arrange
        var cut = RenderComponent<MvuCounter>();

        // Assert
        Assert.Equal("0", cut.Find("p").TextContent);
    }

    [Fact]
    public void MvuCounter_IncrementButtonIncreasesValue()
    {
        // Arrange
        var cut = RenderComponent<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(2)").Click();

        // Assert
        Assert.Equal("1", cut.Find("p").TextContent);
    }

    [Fact]
    public void MvuCounter_DecrementButtonDecreasesValue()
    {
        // Arrange
        var cut = RenderComponent<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(1)").Click();

        // Assert
        Assert.Equal("-1", cut.Find("p").TextContent);
    }

    [Fact]
    public void MvuCounter_MultipleClicksUpdateValueCorrectly()
    {
        // Arrange
        var cut = RenderComponent<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(2)").Click();
        cut.Find("button.btn-primary:nth-of-type(2)").Click();
        cut.Find("button.btn-primary:nth-of-type(1)").Click();

        // Assert
        Assert.Equal("1", cut.Find("p").TextContent);
    }
}
