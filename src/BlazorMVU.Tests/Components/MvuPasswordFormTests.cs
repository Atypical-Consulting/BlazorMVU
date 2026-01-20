using BlazorMVU.Demo.Components;

namespace BlazorMVU.Tests.Components;

public class MvuPasswordFormTests : BunitContext
{
    [Fact]
    public void MvuPasswordForm_InitialValuesAreEmpty()
    {
        // Arrange
        var cut = Render<MvuPasswordForm>();

        // Assert
        cut.Find("input[type='text']").GetAttribute("value").ShouldBe("");
        cut.Find("input[type='password']").GetAttribute("value").ShouldBe("");
        cut.FindAll("input[type='password']").Last().GetAttribute("value").ShouldBe("");
    }

    [Fact]
    public void MvuPasswordForm_PasswordMismatchShowsError()
    {
        // Arrange
        var cut = Render<MvuPasswordForm>();

        // Act
        cut.Find("input[type='password']").Input("password");
        cut.FindAll("input[type='password']").Last().Input("different");

        // Assert
        cut.Find("input[type='password']").GetAttribute("aria-invalid").ShouldBe("true");
        cut.FindAll("input[type='password']").Last().GetAttribute("aria-invalid").ShouldBe("true");
    }

    [Fact]
    public void MvuPasswordForm_PasswordMatchClearsError()
    {
        // Arrange
        var cut = Render<MvuPasswordForm>();

        // Act
        cut.Find("input[type='password']").Input("password");
        cut.FindAll("input[type='password']").Last().Input("password");

        // Assert
        cut.Find("input[type='password']").GetAttribute("aria-invalid").ShouldBe("false");
        cut.FindAll("input[type='password']").Last().GetAttribute("aria-invalid").ShouldBe("false");
    }
}