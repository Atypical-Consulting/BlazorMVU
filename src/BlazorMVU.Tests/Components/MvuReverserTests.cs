using BlazorMVU.Demo.Components;

namespace BlazorMVU.Tests.Components;

public class MvuReverserTests : TestContext
{
    [Fact]
    public void MvuReverser_InitialValueIsEmpty()
    {
        // Arrange
        var cut = RenderComponent<MvuReverser>();

        // Assert
        Assert.Equal("", cut.Find("input").GetAttribute("value"));
        Assert.Equal("", cut.Find("#reverser-result").TextContent);
    }

    [Fact]
    public void MvuReverser_InputChangesUpdateState()
    {
        // Arrange
        var cut = RenderComponent<MvuReverser>();

        // Act
        cut.Find("#reverser-input").Input("test");

        // Assert
        Assert.Equal("test", cut.Find("#reverser-input").GetAttribute("value"));
        Assert.Equal("tset", cut.Find("#reverser-result").GetAttribute("value"));
    }

    [Fact]
    public void MvuReverser_MultipleInputChangesUpdateStateCorrectly()
    {
        // Arrange
        var cut = RenderComponent<MvuReverser>();

        // Act
        cut.Find("#reverser-input").Input("test");
        cut.Find("#reverser-input").Input("more");

        // Assert
        Assert.Equal("more", cut.Find("#reverser-input").GetAttribute("value"));
        Assert.Equal("erom", cut.Find("#reverser-result").GetAttribute("value"));
    }
}