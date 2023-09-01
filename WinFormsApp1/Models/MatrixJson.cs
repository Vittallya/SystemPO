using SPO_LR_Lib.Analyzers;
using System.Text.Json.Serialization;

namespace WinFormsApp1.Models
{
    public class MatrixJson: IData
    {
        private string[][] rules;
        private IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)>? rulesList;

        [JsonPropertyName("rules")]
        public string[][] Rules 
        { 
            get => rules; 
            set { rules = value; OnRulesUpdated(); }
        }

        private void OnRulesUpdated()
        {
            int i = 0;
            rulesList = rules.Select(x => ("E", x.ToList(), i++));
        }

        [JsonPropertyName("matrix")]
        public Dictionary<string, Dictionary<string, string>> Matrix { get; set; }

        public IEnumerable<(string NonTerminalToReplace, List<string> rules, int ruleNum)>? GetRules()
        {
            return rulesList;
        }

        public bool HasRelationBetween(string a, string b, out string? relation)
        {
            return Matrix[a].TryGetValue(b, out relation);
        }
    }
}
