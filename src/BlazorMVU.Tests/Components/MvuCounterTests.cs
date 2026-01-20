using BlazorMVU.Demo.Components;

namespace BlazorMVU.Tests.Components;

public class MvuCounterTests : BunitContext
{
    [Fact]
    public void MvuCounter_InitialValueIsZero()
    {
        // Arrange
        var cut = Render<MvuCounter>();

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe("0");
    }

    [Fact]
    public void MvuCounter_IncrementButtonIncreasesValue()
    {
        // Arrange
        var cut = Render<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(2)").Click();

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe("1");
    }

    [Fact]
    public void MvuCounter_DecrementButtonDecreasesValue()
    {
        // Arrange
        var cut = Render<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(1)").Click();

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe("-1");
    }

    [Fact]
    public void MvuCounter_MultipleClicksUpdateValueCorrectly()
    {
        // Arrange
        var cut = Render<MvuCounter>();

        // Act
        cut.Find("button.btn-primary:nth-of-type(2)").Click();
        cut.Find("button.btn-primary:nth-of-type(2)").Click();
        cut.Find("button.btn-primary:nth-of-type(1)").Click();

        // Assert
        cut.Find("input").GetAttribute("value").ShouldBe("1");
    }
}