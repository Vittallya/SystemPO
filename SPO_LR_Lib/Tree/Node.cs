namespace SPO_LR_Lib
{
    class Node
    {
        public Node(IEnumerable<(string, SymbolType, string, string)> symbols, int ruleNum)
        {
            ReplacedSymbols = symbols;
            RuleNum = ruleNum;
        }

        public int RuleNum { get; }

        public IEnumerable<(string symbol, SymbolType symbolType, string origin, string lexType)> ReplacedSymbols { get; }
    }
}
