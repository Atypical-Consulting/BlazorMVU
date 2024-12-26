using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Components;

public partial class MvuTodoList
{
    // Model
    public record Model(
        ImmutableList<Todo> Todos,
        string NewTodo);

    public record Todo(
        string Description,
        bool IsDone);

    // Messages
    public abstract record Msg
    {
        public record AddTodo(string Description) : Msg;

        public record ToggleDone(Todo Todo) : Msg;

        public record DeleteTodo(Todo Todo) : Msg;

        public record NewTodoChange(string NewTodo) : Msg;
    }

    // Parameter for initial state
    [Parameter]
    public ImmutableList<Todo> Todos { get; set; }
        = ImmutableList<Todo>.Empty;

    // Initialize the model
    protected override Model Init()
        => new(Todos, "");

    // Update the model based on the message
    protected override Model Update(Msg msg, Model model)
        => msg switch
        {
            Msg.AddTodo addTodo => new Model(model.Todos.Add(new Todo(addTodo.Description, false)), ""),
            Msg.ToggleDone toggleDone => model with
            {
                Todos = model.Todos.Replace(toggleDone.Todo,
                    toggleDone.Todo with { IsDone = !toggleDone.Todo.IsDone })
            },
            Msg.DeleteTodo deleteTodo => model with { Todos = model.Todos.Remove(deleteTodo.Todo) },
            Msg.NewTodoChange newTodoChange => model with { NewTodo = newTodoChange.NewTodo },
            _ => model
        };

    // Add a new todo
    private void AddTodo()
    {
        if (!string.IsNullOrWhiteSpace(State.NewTodo))
        {
            Dispatch(new Msg.AddTodo(State.NewTodo));
        }
    }

    // Toggle the done status of a todo
    private void ToggleDone(Todo todo)
    {
        Dispatch(new Msg.ToggleDone(todo));
    }

    // Delete a todo
    private void DeleteTodo(Todo todo)
    {
        Dispatch(new Msg.DeleteTodo(todo));
    }

    // Handle the text changes and dispatch messages
    private void HandleNewTodoChange(ChangeEventArgs obj)
    {
        var name = obj.Value?.ToString() ?? "";
        var msg = new Msg.NewTodoChange(name);
        Dispatch(msg);
    }
}