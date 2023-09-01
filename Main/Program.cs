
using System.Collections.Immutable;

new Solution().LengthOfLongestSubstring("abba");

public class Solution
{
    public int LengthOfLongestSubstring(string s)
    {
        if (s.Length == 0)
            return 0;

        var arr = s.Select((c, i) => (i, c)).ToList();
        

        var groups = arr.GroupBy(x => x.c);

        if (groups.Count() == 1)
            return 1;

        groups = groups.Where(x => x.Count() > 1);

        if (!groups.Any())
            return s.Length;

        var groups1 = groups.
            
            Select((a, b) =>
            {
                var c = a.Select(x => x.i).OrderBy(x => x);

                if (c.Count() == 1)
                    return 0;

                var elem1 = c.First();
                var max = c.Skip(1).Max(y =>
                {
                    var dist = Math.Max(2, Math.Abs(elem1 - y));
                    elem1 = y;
                    return dist;
                });
                return max;
            });

        int max = groups1.Max();


        return max;
    }
}