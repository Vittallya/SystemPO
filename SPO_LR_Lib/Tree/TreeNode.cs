namespace SPO_LR_Lib
{
    public enum TreeNodeType
    {
        Identifier,     //узел дерева - идентификатор
        BinaryOp,       //узел дерева - бинарная операция
        UnaryOp,        //узел дерева - унарная операция
    }


    /// <summary>
    /// Класс узла дерева
    /// </summary>
    public class TreeNode
    {

        private readonly List<TreeNode> children;

        /// <summary>
        /// Список потомков узла (потомки могут быть только у терминалов)
        /// </summary>
        public IEnumerable<TreeNode> Childs => children;

        /// <summary>
        /// Тип узла
        /// </summary>
        public TreeNodeType NodeType { get; private set; } = TreeNodeType.Identifier;

        /// <summary>
        /// Конструктор узла дерево, которое принимает на вход символ тип символа. Исходную лексему, тип лексемы и номер правила.
        /// </summary>
        /// <param name="symbol">символ</param>
        /// <param name="symbolType">тип символа</param>
        /// <param name="originLexem">исходная лексема</param>
        /// <param name="lexType">тип лескемы</param>
        /// <param name="ruleNum">номер правила</param>
        public TreeNode(string symbol, SymbolType symbolType, string originLexem, string lexType, int ruleNum)
        {
            children = new List<TreeNode>();
            Symbol = symbol;
            SymbolType = symbolType;
            OriginLexem = originLexem;
            LexType = lexType;
            RuleNum = ruleNum;
        }

        /// <summary>
        /// Добавление дочерних узлов к текущему узлу
        /// В этом методе также определяется тип узла: либо это идентификатор, либо это бинарная или унарная операция.
        /// </summary>
        /// <param name="nodes"></param>
        public void AddChilds(IEnumerable< TreeNode> nodes)
        {
            children.AddRange(nodes);

            if (children.Count == 1 && children[0].Symbol == "a")
            {
                NodeType = TreeNodeType.Identifier;
            }
            else
            {
                int operandsCount = children.Count(x => x.Symbol == "a" || x.SymbolType == SymbolType.NonTerminal);
                NodeType = operandsCount == 2 ? TreeNodeType.BinaryOp : TreeNodeType.UnaryOp;
            }
        }


        public IEnumerable<TreeNode> GetAllNonTerminals(string letter)
        {
            List<TreeNode> nodes = new List<TreeNode>();

            if(SymbolType == SymbolType.NonTerminal)
            {
                if (Symbol == letter)
                {
                    nodes.Add(this);
                }

                var nodesChildren = children.SelectMany(x => x.GetAllNonTerminals(letter));
                nodes.AddRange(nodesChildren);

                return nodes;
            }

            return nodes;
        }

        /// <summary>
        /// Поиск узла, который еще не имеет потомков
        /// </summary>
        /// <returns>узел, который еще не имеет потомков, если такой есть или null</returns>
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
