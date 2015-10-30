using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    //
    // Implements a k-ary tree.
    //
    public class TreeNode<T>
    {
        private TreeNode<T> parent;
        private List<TreeNode<T>> children;
        private T data;

        public TreeNode(T nodeVal)
        {
            parent = null;
            children = new List<TreeNode<T>>();
            data = nodeVal;
        }

        public TreeNode(T rootVal, List<T> kids)
        {
            data = rootVal;

            children = new List<TreeNode<T>>();
            foreach (T k in kids)
            {
                children.Add(new TreeNode<T>(this, k));
            }
        }

        public TreeNode(TreeNode<T> p, T value)
        {
            parent = p;
            data = value;
            children = new List<TreeNode<T>>();
        }

        public void AddChild(TreeNode<T> node)
        {
            children.Add(node);
        }

        private void AddChildren(List<TreeNode<T>> nodes)
        {
            foreach (TreeNode<T> node in nodes)
            {
                children.Add(node);
            }
        }

        public void AddChild(T value)
        {
            children.Add(new TreeNode<T>(this, value));
        }

        private void AddChildren(List<T> values)
        {
            foreach (T val in values)
            {
                children.Add(new TreeNode<T>(this, val));
            }
        }

        public T GetData() { return this.data; }
        public TreeNode<T> Parent() { return this.parent; }
        public List<TreeNode<T>> Children() { return this.children; }

        // Given some node in the tree (this), walk up the tree to the root and return the root. 
        public TreeNode<T> GetRoot()
        {
            TreeNode<T> prev = this;
            TreeNode<T> curr = this;
            while (curr != null)
            {
                prev = curr;
                curr = curr.parent;
            }

            return prev;
        }

        //
        // Add a new set of nodes: value -> kids[0], ..., value -> kids[n]
        //
        //public bool AddToLeaf(T value, List<T> kids)
        //{
        //    bool success = false;

        //    foreach (TreeNode<T> child in children)
        //    {
        //        if (child.IsLeaf())
        //        {
        //            if (child.data.Equals(value))
        //            {
        //                child.AddChildren(kids);
        //                success = true;
        //            }
        //        }
        //        else
        //        {
        //            success = child.AddToLeaf(value, kids) | success;
        //        }
        //    }

        //    return success;
        //}

        public string ToIndentString(int indentLevel)
        {
            string retS = "";

            for (int i = 0; i < indentLevel; i++)
            {
                retS += "  " + indentLevel + "  ";
            }

            retS += data.ToString() + "\n";

            foreach (TreeNode<T> child in children)
            {
                retS += child.ToIndentString(indentLevel + 1);
            }

            return retS;
        }

        public override string ToString()
        {
            return ToIndentString(0);
        }

        public bool IsLeaf()
        {
            return !children.Any();
        }

        private bool SingleChild()
        {
            return children.Count == 1;
        }

        ////
        //// Generate all possible paths given this node as the root
        ////
        //private List<List<T>> Generate()
        //{
        //    List<List<T>> newCombinedChildProblems = new List<List<T>>();
        //    List<List<T>> newOriginalChildProblems = new List<List<T>>();

        //    // If a leaf no problems are possible / any problems
        //    if (IsLeaf()) return newCombinedChildProblems;

        //    //
        //    // Does this node create a legitimate problem?
        //    // That is, does it have only one child
        //    //
        //    if (SingleChild())
        //    {
        //        // Generate a new problem: child |- this
        //        // Encode in the list as <child, this> in that order
        //        TreeNode<T> child = children[0];
        //        List<T> newProblem = new List<T>();
        //        newProblem.Add(child.data);

        //        // newProblem.Add(this.data); No need to add this data now; it is added later

        //        newCombinedChildProblems.Add(newProblem);
        //    }

        //    //
        //    // If we cannot generate a problem from this node directly (# children > 1)
        //    //
        //    // For each child, generate all possible problems.
        //    List<List<T>> allOriginalChildProblems = new List<List<T>>();
        //    List<List<T>> oldChildProblems = new List<List<T>>();
        //    foreach (TreeNode<T> child in children)
        //    {
        //        newOriginalChildProblems = child.Generate();
        //        allOriginalChildProblems.AddRange(newOriginalChildProblems);

        //        //
        //        // Powerset style construction of all combinations of paths from all children
        //        //

        //        // Now, make a copy and append this node to copies of all applicable subproblems
        //        foreach (List<T> newChildProb in newOriginalChildProblems)
        //        {
        //            // We need to check to see if the last element matches the child's data 
        //            if (newChildProb[newChildProb.Count - 1].Equals(child.data))
        //            {
        //                // Make a copy of this problem and add to the new combined list to append with later.
        //                List<T> newChildProbCopy = new List<T>(newChildProb);

        //                // Append to all new paths
        //                foreach (List<T> oldprob in oldChildProblems)
        //                {
        //                    oldprob.AddRange(newChildProbCopy);
        //                }
        //            }
        //        }

        //        if (!oldChildProblems.Any())
        //        {
        //            oldChildProblems.AddRange(newOriginalChildProblems);
        //        }
        //    }

        //    // Now, for each new combined paths, add this node as the LAST node in the list
        //    foreach (List<T> prob in newCombinedChildProblems)
        //    {
        //        prob.Add(this.data);
        //    }

        //    // Combine the original and the addended problems and return that
        //    newCombinedChildProblems.AddRange(allOriginalChildProblems);

        //    return newCombinedChildProblems;
        //}

        //public List<List<T>> GenerateAllProblemsAndSolutions()
        //{
        //    List<List<T>> problems = Generate();

        //    StringBuilder str = new StringBuilder();
        //    foreach (List<T> problem in problems)
        //    {
        //        str.Append("Tree: " + this.ToString() + "\nProblems:\n");
        //        foreach (T node in problem)
        //        {
        //            str.Append(node + " ");
        //        }
        //        str.AppendLine();
        //    }

        //    System.Diagnostics.Debug.WriteLine(str);

        //    return problems;
        //}
    }
}
