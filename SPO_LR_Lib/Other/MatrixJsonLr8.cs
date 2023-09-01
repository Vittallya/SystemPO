using SPO_LR_Lib.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SPO_LR_Lib.Other
{
    [Serializable]
    internal class MatrixJsonLr8 : IData
    {
        private Dictionary<string, string[]> rules;
        private IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)>? rulesList;

        [JsonPropertyName("rules")]
        public Dictionary<string, string[]> Rules
        {
            get => rules;
            set { rules = value; OnRulesUpdated(); }
        }

        [JsonPropertyName("matrix")]
        public Dictionary<string, Dictionary<string, string>> Matrix { get; set; }

        private void OnRulesUpdated()
        {
            //rulesList = rules.Select(x => ("E", x.ToList()));
        }


        public IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)> GetRules()
        {
            throw new NotImplementedException();
        }

        public bool HasRelationBetween(string stackTerminal, string b, out string? relation)
        {
            return Matrix[stackTerminal].TryGetValue(b, out relation);
        }
    }
}
