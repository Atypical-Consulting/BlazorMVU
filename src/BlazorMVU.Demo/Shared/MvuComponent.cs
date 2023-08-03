using Microsoft.AspNetCore.Components;

namespace BlazorMVU.Demo.Shared;

public abstract class MvuComponent<TModel, TMsg> : ComponentBase
{
    public required TModel State { get; set; }

    protected abstract TModel Init();

    protected abstract TModel Update(TMsg msg, TModel model);

    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual void Dispatch(TMsg msg)
    {
        State = Update(msg, State);
        StateHasChanged();
    }

    protected override void OnInitialized()
    {
        State = Init();
    }
}
