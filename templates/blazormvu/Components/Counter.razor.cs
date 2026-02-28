namespace BlazorMVU.App.Components;

public partial class Counter
{
    public enum Msg
    {
        Increment,
        Decrement
    }

    protected override int Init()
        => 0;

    protected override int Update(Msg msg, int model)
        => msg switch
        {
            Msg.Increment => model + 1,
            Msg.Decrement => model - 1,
            _ => model
        };
}
