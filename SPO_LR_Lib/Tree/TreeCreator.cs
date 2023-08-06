using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPO_LR_Lib
{
    internal class TreeCreator
    {
        public TreeNode GetTreeView(IEnumerable<Node> list)
        {
            TreeNode root = new("E", SymbolType.NonTerminal, "", "", -1);

            List<TreeNode> treeNodesNonTerminals = new() { root };

            foreach (Node node in list)
            {
                //var lastNonTerminal = treeNodesNonTerminals.LastOrDefault(x => !x.Childs.Any());
                var lastNonTerminal = root.GetLastEmptyNonTerminal();

                var newTreeNodes = node.ReplacedSymbols.Select(x => new TreeNode(x.symbol, x.symbolType, x.origin, x.lexType, node.RuleNum));
                lastNonTerminal.AddChilds(newTreeNodes);
                //treeNodesNonTerminals.AddRange(newTreeNodes.Where(x => x.SymbolType == SymbolType.NonTerminal));
            }

            return root;
        }
    }
}
