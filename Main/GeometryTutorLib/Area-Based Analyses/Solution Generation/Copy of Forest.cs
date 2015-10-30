using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    //
    // Implements a forest of k-ary trees
    //
    public class Forest<T>
    {
        private List<TreeNode<T>> treeList;

        public Forest()
        {
            treeList = new List<TreeNode<T>>();
        }

        //
        // Adds a treenode as a leaf, if possible
        // Counts the number of successful additions
        //
        //public int AddToLeaf(T rootVal, List<T> children)
        //{
        //    int numAdditions = 0;

        //    foreach (TreeNode<T> tree in treeList)
        //    {
        //        numAdditions += tree.AddToLeaf(rootVal, children) ? 1 : 0;
        //    }

        //    return numAdditions;
        //}

        //
        // Strictly adds one new tree consisting of a root node with specified children
        //
        public void AddNewTree(T rootVal, List<T> children)
        {
            treeList.Add(new TreeNode<T>(rootVal, children));
        }

        //public void GenerateAllPaths()
        //{
        //    foreach (TreeNode<T> tree in treeList)
        //    {
        //        tree.GenerateAllProblemsAndSolutions();
        //    }
        //}

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            for (int t = 0; t < treeList.Count; t++)
            {
                str.AppendLine("Tree (" + t + "):\n" + treeList[t].ToString());
            }

            return str.ToString();
        }
    }
}
