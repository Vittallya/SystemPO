class AutomatConnection
{
    private readonly List<Predicate<char?>> predicates;

    public AutomatConnection(AutomatState origin, AutomatState reciever, string[] conditions)
    {
        Origin = origin;
        Reciever = reciever;
        predicates = new List<Predicate<char?>>();

        foreach (var str in conditions)
        {

            if (str.Contains('-'))
            {
                var parts = str.Split('-');

                int low = parts[0][0];
                int high = parts[1][0];

                if (low > high)
                    throw new ArgumentException($"В указанном шаблоне (\"{str}\") нижнее значение больше верхнего");

                predicates.Add(s => s.HasValue && (int)s >= low && (int)s <= high);
            }

            else if (str.Contains(','))
            {
                var parts = str.Split(",");
                predicates.Add(s => parts.Any(p => !s.HasValue && p.Length == 0 || p.Length == 1 && p[0] == s ));
            }
            else
            {
                predicates.Add(s => !s.HasValue && str.Length == 0 || str.Length == 1 && str[0] == s);
            }
        }
    }

    public bool IsAccepts(char? sym)
    {
        return predicates.Any(p => p(sym));
    }

    public AutomatState Origin { get; }
    public AutomatState Reciever { get; }
    public IEnumerable<Predicate<char?>> Predicates => predicates;
}