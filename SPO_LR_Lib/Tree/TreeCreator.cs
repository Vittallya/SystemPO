namespace SPO_LR_Lib
{
    /// <summary>
    /// Класс, который на основе результатов свертки синтаксического анализа возвращает 
    /// дерево последовательных выражений.
    /// </summary>
    internal class TreeCreator
    {
        /// <summary>
        /// Метод, который принимает список не связанных между собой объектов node и возвращает корень
        /// получившегося дерева.
        /// </summary>
        /// <param name="list">список не связанных между собой объектов node</param>
        /// <returns>корень получившегося дерева</returns>
        public TreeNode GetTreeView(IEnumerable<Node> list, string startLetter)
        {
            TreeNode root = new(startLetter, SymbolType.NonTerminal, "", "", -1);

            List<TreeNode> treeNodesNonTerminals = new() { root };

            foreach (Node node in list)
            {
                //Из корня дерева получаем такой нетерминальный символ, который не имеет потомков.
                var lastNonTerminal = root.GetLastEmptyNonTerminal();
                //создаем дочерние узлы
                var newTreeNodes = node.ReplacedSymbols.Select(x => new TreeNode(x.symbol, x.symbolType, x.origin, x.lexType, node.RuleNum));

                //добавляем эти узлы в качестве потомков текущего найденного нетерминала
                lastNonTerminal.AddChilds(newTreeNodes);
            }

            return root;
        }
    }
}
