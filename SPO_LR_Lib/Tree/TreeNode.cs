namespace SPO_LR_Lib
{
    public enum TreeNodeType
    {
        Identifier, BinaryOp, UnaryOp, None, Separator
    }

    public class TreeNode
    {

        private readonly List<TreeNode> children;

        public IEnumerable<TreeNode> Childs => children;

        public TreeNodeType NodeType { get; private set; } = TreeNodeType.Identifier;

        public TreeNode(string symbol, SymbolType symbolType, string originLexem, string lexType, int ruleNum)
        {
            children = new List<TreeNode>();
            Symbol = symbol;
            SymbolType = symbolType;
            OriginLexem = originLexem;
            LexType = lexType;
            RuleNum = ruleNum;
        }

        public void AddChilds(IEnumerable< TreeNode> nodes)
        {
            children.AddRange(nodes);

            if (children.Count == 1 && children[0].Symbol == "a")
            {
                NodeType = TreeNodeType.Identifier;
            }
            else if(children.Count == 5)
            {
                NodeType = TreeNodeType.Separator;
            }

            else
            {
                int operandsCount = children.Count(x => x.Symbol == "a" || x.SymbolType == SymbolType.NonTerminal);
                NodeType = operandsCount == 2 ? TreeNodeType.BinaryOp : TreeNodeType.UnaryOp;
            }
        }

        public TreeNode? GetLastEmptyNonTerminal()
        {
            if (!Childs.Any() && SymbolType == SymbolType.NonTerminal)
                return this;

            var childs = children.Select(x => x.GetLastEmptyNonTerminal());

            return childs.LastOrDefault(x => x != null);

        }


        public string Symbol { get; }
        public SymbolType SymbolType { get; }
        public string OriginLexem { get; }
        public string LexType { get; }
        public int RuleNum { get; }
    }
}
