public enum StateType
{
    Usual, Start, Final
}

public class AutomatState
{
    private List<AutomatConnection> automatConnections = new();
    private readonly string name;

    public AutomatState(string name, StateType type)
    {
        this.name = name;
        Type = type;
    }

    public string Name => name;

    public StateType Type { get; }

    public AutomatState? GetNext(char? sym = null)
    {
        return automatConnections.FirstOrDefault(x => x.IsAccepts(sym))?.Reciever;
    }

    public void ConnectTo(AutomatState state, params string[] conditions)
    {
        var connection = new AutomatConnection(this, state, conditions);
        automatConnections.Add(connection);
    }


    public override bool Equals(object? obj)
    {
        if (obj is AutomatState at)
            return name.Equals(at.Name);

        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }
}
