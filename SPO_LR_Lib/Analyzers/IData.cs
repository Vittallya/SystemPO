using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib.Analyzers
{
    public interface IData
    {
        bool HasRelationBetween(string stackTerminal, string b, out string? relation);

        IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)> GetRules();
    }
}
