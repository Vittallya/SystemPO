using SPO_LR_Lib.Analyzers;
using SPO_LR_Lib.Extensions;
using System.Text.Json.Serialization;

namespace WinFormsApp1.Models
{
    [Serializable]
    internal class MatrixJsonLr8 : IData
    {
        private Dictionary<string, string[]> rules;
        private IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)> rulesList;

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
            int i = 0;
            rulesList = rules.SelectMany(r => r.Value.
                                    GroupOnChange((x, y) => y == "|").
                                    Select(x => (r.Key, x.First() == "|" ? x.Skip(1).ToList() : x.ToList(), i++))).ToList();
        }


        public IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)> GetRules()
        {
            return rulesList;
        }

        public bool HasRelationBetween(string stackTerminal, string b, out string? relation)
        {
            return Matrix[stackTerminal].TryGetValue(b, out relation);
        }
    }
}
