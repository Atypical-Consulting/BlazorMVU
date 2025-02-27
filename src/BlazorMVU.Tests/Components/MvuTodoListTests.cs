using BlazorMVU.Demo.Components;

namespace BlazorMVU.Tests.Components;

public class MvuTodoListTests : TestContext
{
    [Fact]
    public void MvuTodoList_InitialValueIsEmpty()
    {
        // Arrange
        var cut = RenderComponent<MvuTodoList>();

        // Assert
        cut.FindAll("li").Count.ShouldBe(0);
    }

    [Fact]
    public void MvuTodoList_AddButtonAddsItem()
    {
        // Arrange
        var cut = RenderComponent<MvuTodoList>();

        // Act
        cut.Find("input").Input("Buy milk");
        cut.Find("button").Click();
        
        // Assert
        cut.FindAll(".todo-item").Count.ShouldBe(1);
    }
}