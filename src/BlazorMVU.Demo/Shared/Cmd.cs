namespace BlazorMVU.Demo.Shared;

public delegate void Dispatch<TMsg>(TMsg msg);

public delegate void Effect<TMsg>(Dispatch<TMsg> dispatch);

public class Cmd<TMsg>
{
    public List<Effect<TMsg>> Effects { get; init; }

    public Cmd()
    {
        Effects = new List<Effect<TMsg>>();
    }

    public Cmd<TMsg> None()
    {
        return new Cmd<TMsg>();
    }

    public Cmd<TMsg> OfMsg(TMsg msg)
    {
        return new Cmd<TMsg>
        {
            Effects = new List<Effect<TMsg>>
            {
                dispatch => dispatch(msg)
            }
        };
    }

    public Cmd<TMsg> Batch(IEnumerable<Cmd<TMsg>> cmds)
    {
        return new Cmd<TMsg>
        {
            Effects = cmds
                .SelectMany(c => c.Effects)
                .ToList()
        };
    }

    // Other methods omitted for brevity...
}