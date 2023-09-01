using System.Data;

namespace WinFormsApp1
{
    public partial class FormTree : Form
    {
        public FormTree()
        {
            InitializeComponent();
        }

        public Form SetupTreeView(SPO_LR_Lib.TreeNode rootNode)
        {
            //построение графического дерева выражений
            var treeNode = GetTreeNodeFrom(rootNode);
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(treeNode);
            return this;
        }

        /// <summary>
        /// Рекурсивный метод обхода дерева выражений
        /// </summary>
        /// <param name="root">корень дерева выражений</param>
        /// <returns></returns>
        private TreeNode GetTreeNodeFrom(SPO_LR_Lib.TreeNode root)
        {
            //узел графического дерева
            var node = new TreeNode();

            //если узел дерева выражений - не терминал
            if (root.SymbolType == SPO_LR_Lib.SymbolType.NonTerminal)
            {
                //подписать как буква нетерминала
                node.Text = root.Symbol;
                node.NodeFont = new Font("default", 12, FontStyle.Bold);


                //вызвать этот же метод для всех потомков данного узла дерева
                var childNodes = root.Childs.Select(x => GetTreeNodeFrom(x));

                node.Nodes.Clear();
                //добавить найденные дочерние узлы граф. дерева в текущий узел
                node.Nodes.AddRange(childNodes.ToArray());
            }
            else
            {
                //если терминал - подписать как исходную лексему
                node.Text = root.OriginLexem;
            }

            return node;
        }

    }
}
