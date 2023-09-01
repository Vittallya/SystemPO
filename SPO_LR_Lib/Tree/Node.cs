namespace SPO_LR_Lib
{
    /// <summary>
    /// Класс, который запоминает результаты каждой свертки в ходе синтаксического анализа.
    /// </summary>
    class Node
    {
        public Node(IEnumerable<(string, SymbolType, string, string)> symbols, int ruleNum)
        {
            ReplacedSymbols = symbols;
            RuleNum = ruleNum;
        }

        /// <summary>
        /// Номер правила, которое было применено для свертки
        /// </summary>
        public int RuleNum { get; }

        /// <summary>
        /// Список замененных в результате свертки символов.
        /// </summary>
        public IEnumerable<(string symbol, SymbolType symbolType, string origin, string lexType)> ReplacedSymbols { get; }
    }
}
