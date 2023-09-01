using System.Diagnostics.CodeAnalysis;

namespace SPO_LR_Lib.Comparers
{
    internal class TreeNodeComparer : IEqualityComparer<TreeNode>
    {
        private readonly Func<TreeNode?, TreeNode?, bool> func;
        private readonly Func<TreeNode, int> func1;

        public TreeNodeComparer(Func<TreeNode?, TreeNode?, bool> func, Func<TreeNode, int> func1)
        {
            this.func = func;
            this.func1 = func1;
        }

        public bool Equals(TreeNode? x, TreeNode? y)
        {
            return func(x, y);
        }

        public int GetHashCode([DisallowNull] TreeNode obj)
        {
            return func1(obj);
        }
    }
}
