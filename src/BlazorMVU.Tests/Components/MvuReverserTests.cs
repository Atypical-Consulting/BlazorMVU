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
        cut.Find("input").GetAttribute("value").ShouldBe("");
        cut.Find("#reverser-result").TextContent.ShouldBe("");
    }

    [Fact]
    public void MvuReverser_InputChangesUpdateState()
    {
        // Arrange
        var cut = RenderComponent<MvuReverser>();

        // Act
        cut.Find("#reverser-input").Input("test");

        // Assert
        cut.Find("#reverser-input").GetAttribute("value").ShouldBe("test");
        cut.Find("#reverser-result").GetAttribute("value").ShouldBe("tset");
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
        cut.Find("#reverser-input").GetAttribute("value").ShouldBe("more");
        cut.Find("#reverser-result").GetAttribute("value").ShouldBe("erom");
    }
}