using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Shared;

public abstract class MvuComponent<TModel, TMsg> : ComponentBase
{
    protected TModel State { get; set; }

    protected abstract TModel Init();

    protected abstract (TModel, Cmd<TMsg>) Update(TMsg msg, TModel model);

    protected override void OnInitialized()
    {
        State = Init();
    }

    protected virtual void Dispatch(TMsg msg)
    {
        Console.WriteLine($"Dispatching {msg}");
        
        (State, Cmd<TMsg> cmd) = Update(msg, State);
        ExecuteCmd(cmd);
        StateHasChanged();
    }

    private void ExecuteCmd(Cmd<TMsg> cmd)
    {
        foreach (var effect in cmd.Effects)
        {
            effect(Dispatch);
        }
    }
}
