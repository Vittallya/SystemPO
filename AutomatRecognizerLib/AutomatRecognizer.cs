
public class AutomatRecognizer
{
    private readonly Dictionary<string, AutomatState> statesByName = new();

    private AutomatState? startState;

    public void CreateState(string name, StateType stateType = StateType.Usual)
    {
        if(statesByName.ContainsKey(name))
            throw new ArgumentException($"Состояние с именем \"{name}\" уже есть");

        var state = new AutomatState(name, stateType);

        if(stateType == StateType.Start)
        {

            if (startState == null)
                startState = state;
            else
                throw new ArgumentException("Начальное состояние уже задано");
        }


        statesByName.Add(name, state);
    }

    public void CreateConnection(string sourceStateName, string recieverStateName, params string[] conditions)
    {
        if (!statesByName.TryGetValue(sourceStateName, out AutomatState? sourceState))
            throw new ArgumentException($"Не найдено состояние-источник по указанному имени (\"{sourceStateName}\")");
            
            
        if (!statesByName.TryGetValue(recieverStateName, out AutomatState? recieverState))
            throw new ArgumentException($"Не найдено состояние-источник по указанному имени (\"{recieverStateName}\")");

        sourceState.ConnectTo(recieverState, conditions);
    }

    public bool TryRecognize(string input, out AutomatState? endState)
    {
        if (startState == null)
            throw new ArgumentException("Начальное состояние не задано");

        if(input == null)
            throw new ArgumentNullException(nameof(input));

        AutomatState? current = startState;
        bool res = false;



        for (int i = 0; i < input.Length; i++)
        {
            current = current?.GetNext(input[i]);

            if (current == null)
            {
                break;
            }

        }

        if (current != null && current.Type != StateType.Final)
        {
            current = current.GetNext();
        }

        res = current != null && current.Type == StateType.Final;
        endState = current;

        return res;
    }
}
