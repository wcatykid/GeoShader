using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // A subset of an actual problem which is defined by the set of edges and the goal node (as a catalyst)
    //
    public class GradedProblemTree
    {
        private int goal;
        // Edges are <Many-Many>-to-many: <source list, target-list>
        private Dictionary<List<List<int>>, List<int>> edges;

        // The original hypergraph (for reference purposes)
        private Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;

        public GradedProblemTree(Problem<Hypergraph.EdgeAnnotation> thatProblem, Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        {
            goal = thatProblem.goal;
            edges = new Dictionary<List<List<int>>, List<int>>();
            this.graph = g;

            // Note: the edges in this structure are reversed
            foreach (PebblerHyperEdge<Hypergraph.EdgeAnnotation> edge in thatProblem.edges)
            {
                // Construct the list of lists (as nodes are now multi-element lists)
                List<List<int>> sources = new List<List<int>>();
                foreach (int src in edge.sourceNodes)
                {
                    sources.Add(Utilities.MakeList<int>(src));
                }
                edges.Add(sources, Utilities.MakeList<int>(edge.targetNode));
            }
        }

        //
        // Get the edge that has the given target node
        // Note: due to contraction, this is a true hyperedge with many-to-many nodes
        // 
        private KeyValuePair<List<List<int>>, List<int>> GetProblemEdge(int nodeToFind)
        {
            // We can check the edge list if it is not a contracted node
            foreach (KeyValuePair<List<List<int>>, List<int>> edge in edges)
            {
                if (edge.Value.Contains(nodeToFind)) return edge;
            }

            return new KeyValuePair<List<List<int>>, List<int>>(null, null);
        }

        //
        // The graph minor contraction operation: removing an edge
        //
        public void Contract(KeyValuePair<List<List<int>>, List<int>> edgeToRemove)
        {
            //
            // The target node-set of the edge is added to the 'node-list' of all source nodes
            //
            foreach (List<int> sourceNodeList in edgeToRemove.Key)
            {
                foreach (int src in sourceNodeList)
                {
                    KeyValuePair<List<List<int>>, List<int>> edgeToGivenSource = GetProblemEdge(src);

                    edgeToGivenSource.Value.AddRange(edgeToRemove.Value);
                }
            }

            // Remove the actual edge from the problem tree
            edges.Remove(edgeToRemove.Key);
        }

        //
        // Problem-based Isomorphism (strict hypergraph isomorphism)
        //
        // Returns the goal of the edge in which the problems differ which causes non-isomorphism
        //public int IsStrictlyIsomorphic(GradedProblemTree thatProblem)
        //{
        //    // Starting with the edge connecting to the goal verify isomorphism
        //    return StrictEdgeIsomorphism(GetProblemEdge(this.goal), thatProblem, thatProblem.GetProblemEdge(thatProblem.goal));
        //}

        //
        // Returns the goal of the edge in which the problems differ which causes non-isomorphism
        // -1 indicates a success isomorphism
        //
        //private int StrictEdgeIsomorphism(KeyValuePair<List<List<int>>, List<int>> thisEdge, GradedProblemTree thatProblem, KeyValuePair<List<List<int>>, List<int>> thatEdge)
        //{
        //    // Compare the targets of the given edges
        //    if (thisEdge.Value.Count != thatEdge.Value.Count) return thatEdge.Value[0];

        //    // The target node set must create an isomorphism with the set of nodes 
        //    if (!AreNodesSubsetIsomorphic(thisEdge.Value, thatEdge.Value)) return thatEdge.Value[0];



        //    // The source node counts must equate
        //    if (thisEdge.sourceNodes.Count != thatEdge.sourceNodes.Count) return thisEdge;

        //    //
        //    // All source nodes in this problem must be isomoprhic to all source nodes in that edge; that is, there must be a combination that expresses the isomorphishm
        //    // this is, in the worst case, an n^2 operation
        //    //
        //    int srcCount = thisEdge.sourceNodes.Count;
        //    for (int thisIndex = 0; thisIndex < srcCount; thisIndex++)
        //    {
        //        bool[] marked = new bool[thisEdge.sourceNodes.Count];
        //        for (int thatIndex = 0; thatIndex < srcCount; thatIndex++)
        //        {
        //            List<int> contractNodes = null;
        //            if (!nodes.TryGetValue(edgeSourceNode, out nodeVals))
        //            {
        //                throw new ArgumentException("Attempt to acquire contract node from dictionary failed unexpectedly: " + thisNode);
        //            }

                    
        //            if (AreNodesSubsetIsomorphic(thisSrc, thatSrc))
        //            {
        //            }
        //        }
        //    }

        //    //
        //    // Using the goal node as a starting point, we walk backward along the edges.
        //    //
        //    PebblerHyperEdge thisProblemEdge = this.GetProblemEdge(goal);
        //    PebblerHyperEdge newProblemEdge = thatProblem.GetProblemEdge(thatProblem.goal);
        //    while (thisProblemEdge.sourceNodes.Count == newProblemEdge.sourceNodes.Count)
        //    {

        //    }
        //}

        //private List<int> permutation

        //private int ElementOfSet(List<int> smaller, List<int> larger)
        //{

        //}

        //// Given two nodes, determine if they are isomorphic according to our definition:
        //// Two nodes are isomorphic if they are of the same type and the same relation
        //// instance with the same strength.
        ////
        //// This function is for graded isomorphism as a node, due to contraction, are many nodes
        ////
        //private bool AreNodesSubsetIsomorphic(List<int> smaller, List<int> larger)
        //{
        //    if (smaller.Count != larger.Count) return false;

        //    return StrictSubset(new List<int>(smaller), new List<int>(larger));
        //}





//        //
//        // Given a problem and query vector determine strict isomorphism between this problem and this partition of problems
//        //
//        public bool IsStrictlyIsomorphic(Problem newProblem, QueryFeatureVector query)
//        {
//            if (query.goalIsomorphism)
//            {
//                if (!AreNodesIsomorphic(this.goal, newProblem.goal)) return false;
//            }

//            //
//            // Add other query checks here....
//            //

//            return true;
//        }

//        // Given two nodes, determine if they are isomorphic according to our definition:
//        // Two nodes are isomorphic if they are of the same type and the same relation
//        // instance with the same strength.
//        private bool AreNodesIsomorphic(int node1, int node2)
//        {
//            // A node is strongly isomorphic to itself
//            if (node1 == node2) return true;

//            // Do the nodes have the same type: algebraic or geometric?
//            if (!IsSameType(node1, node2)) return false;

//            // Are the nodes strongly related?
//            return StronglyRelated(node1, node2);
//        }

//        //
//        // Do the nodes have the same type: algebraic or geometric?
//        //
//        private bool IsSameType(int node1, int node2)
//        {
//            GroundedClause firstNode = graph.GetNode(node1);
//            GroundedClause secondNode = graph.GetNode(node2);

//            return firstNode.IsGeometric() && secondNode.IsGeometric() ||
//                   firstNode.IsAlgebraic() && secondNode.IsAlgebraic();
//        }

//        /// <summary>
//        /// Determines if the given nodes are the same regardless of type, but considering the type of relationship.
//        /// </summary>
//        /// <param name="node1">Integer representation of the node in the hypergraph</param>
//        /// <param name="node2">Integer representation of the node in the hypergraph</param>
//        /// <returns>This function will return FALSE if one node is congruence of segments and the other is congruence of triangles</returns>
//        private bool StronglyRelated(int node1, int node2)
//        {
//            // Check the weak relationship first
//            if (!WeaklyRelated(node1, node2)) return false;

//            GroundedClause firstNode = graph.GetNode(node1);
//            GroundedClause secondNode = graph.GetNode(node2);

//            return StronglyRelated(firstNode, secondNode);
//        }

//        private bool StronglyRelated(GroundedClause firstNode, GroundedClause secondNode)
//        {
//            if (firstNode is SegmentRatio && secondNode is SegmentRatio) return true;
//            if (firstNode is ProportionalAngles && secondNode is ProportionalAngles) return true;

//            if (firstNode is Congruent && secondNode is Congruent)
//            {
//                if (firstNode is CongruentAngles && secondNode is CongruentAngles) return true;
//                if (firstNode is CongruentSegments && secondNode is CongruentSegments) return true;
//                if (firstNode is CongruentTriangles && secondNode is CongruentTriangles) return true;
//                return false;
//            }

//            if (firstNode is Equation && secondNode is Equation)
//            {
//                if (firstNode is AngleEquation && secondNode is AngleEquation) return true;
//                if (firstNode is SegmentEquation && secondNode is SegmentEquation) return true;
//                return false;
//            }

//            //
//            // We may strenghthen for many reasons (right triangle, isosceles triangle as well as perpendicular bisector)
//            // Compare those strengthened values for a relationship
//            //
//            if (firstNode is Strengthened && secondNode is Strengthened)
//            {
//                Strengthened streng1 = firstNode as Strengthened;
//                Strengthened streng2 = secondNode as Strengthened;

//                return StronglyRelated(streng1.strengthened, streng2.strengthened);
//            }

//            //if (firstNode is Similar)
//            //{
//            //    return secondNode is Similar;
//            //}

//            return firstNode.GetType() == secondNode.GetType();
//        }


//        /// <summary>
//        /// Determines if the given nodes are the same regardless of type or the specific type of relationship.
//        /// </summary>
//        /// <param name="node1">Integer representation of the node in the hypergraph</param>
//        /// <param name="node2">Integer representation of the node in the hypergraph</param>
//        /// <returns>That is, this function will return TRUE if one node is congruence of segments and the other is congruence of triangles</returns>
//        private bool WeaklyRelated(int node1, int node2)
//        {
//            GroundedClause firstNode = graph.GetNode(node1);
//            GroundedClause secondNode = graph.GetNode(node2);

//            //System.Diagnostics.Debug.WriteLine("Related Nodes? " + firstNode.ToString() + " " + secondNode.ToString());

//            if (firstNode is Congruent)
//            {
//                return secondNode is Congruent;
//            }

//            if (firstNode is Equation)
//            {
//                return secondNode is Equation;
//            }

//            //if (firstNode is Similar)
//            //{
//            //    return secondNode is Similar;
//            //}

//            return firstNode.GetType() == secondNode.GetType();
//        }





//        public override int GetHashCode()
//        {
//            return base.GetHashCode();
//        }

//        //
//        // Just a simple hashing mechanism
//        //
//        public long GetHashKey()
//        {
//            long key = 1;

//            key *= givens[0];

//            if (path.Any()) key *= path[0];

//            key *= goal;

//            return key;
//        }

//        public bool InSource(int n)
//        {
//            return givens.Contains(n);
//        }

//        public bool InPath(int n)
//        {
//            return path.Contains(n);
//        }

//        public bool HasGoal(int n)
//        {
//            return goal == n;
//        }

//        //
//        // Create a new problem by removing the target and appending the new sources
//        //
//        // This problem                       { This Givens } { This Path } -> This Goal
//        // The new problem is of the form:    { New Givens } { Path: emptyset } -> Goal
//        //                       Combined:    { New Givens  U  This Givens \minus This Goal} {This Path  U  This Goal } -> Goal
//        //
//        public Problem CombineAndCreateNewBackwardProblem(Problem problemToInsert)
//        {
//            // If this is the first node in the sequence, return the other problem
//            if (goal == -1) return new Problem(problemToInsert);

//            // Make a copy of this (old) problem.
//            Problem newProblem = new Problem(this);

//            // degenerate the target node by removing the new target from the old sources
//            newProblem.givens.Remove(problemToInsert.goal);

//            // Add all the new sources to the degenerated old sources; do so uniquely
//            Utilities.AddUniqueList<int>(newProblem.givens, problemToInsert.givens);

//            // Combine all the paths of the old and the new problems together; do so uniquely
//            // Utilities.AddUniqueList<int>(newProblem.path, problemToInsert.path);

//            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
//            Utilities.AddUnique<int>(newProblem.path, problemToInsert.goal);

//            // Now, if there exists a node in the path AND in the givens, remove it from the path
//            foreach (int src in newProblem.givens)
//            {
//                newProblem.path.Remove(src);
//            }

////System.Diagnostics.Debug.WriteLine("Combining --------------------------\n" + "\t" + this + "\t" + problemToInsert + "\n = \t" + newProblem + "\n-----------------------");

//            return newProblem;
//        }

//        //
//        // Create a new problem by removing the target and appending the new sources
//        //
//        public Problem CombineAndCreateNewProblem(Problem problemToInsert)
//        {
//            // Make a copy of this (old) problem.
//            Problem newProblem = new Problem(this);

//            // degenerate the target node by removing the new target from the old sources
//            newProblem.givens.Remove(problemToInsert.goal);

//            // Add all the new sources to the degenerated old sources; do so uniquely
//            Utilities.AddUniqueList<int>(newProblem.givens, problemToInsert.givens);

//            // Combine all the paths of the old and the new problems together; do so uniquely
//            Utilities.AddUniqueList<int>(newProblem.path, problemToInsert.path);

//            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
//            Utilities.AddUnique<int>(newProblem.path, problemToInsert.goal);

//            // Now, if there exists a node in the path AND in the givens, remove it from the path
//            foreach (int src in newProblem.givens)
//            {
//                if (newProblem.path.Contains(src))
//                {
//                    newProblem.path.Remove(src);
//                }
//            }

////System.Diagnostics.Debug.WriteLine("Combining --------------------------\n" + this +  problemToInsert + " = " + newProblem + "\n-----------------------");

//            return newProblem;
//        }

//        //
//        // Problems are equal only if the givens, goal, and paths are the same
//        //
//        public override bool Equals(object obj)
//        {
//            Problem thatProblem = obj as Problem;
//            if (thatProblem == null) return false;

//            if (this.goal != thatProblem.goal) return false;

//            if (this.givens.Count != thatProblem.givens.Count) return false;

//            if (this.path.Count != thatProblem.path.Count) return false;

//            // Union the sets; if the union is the same size as the original, they are the same
//            List<int> union = new List<int>(this.givens);
//            Utilities.AddUniqueList<int>(union, thatProblem.givens);
//            if (union.Count != this.givens.Count) return false;

//            union = new List<int>(this.path);
//            Utilities.AddUniqueList<int>(union, thatProblem.path);
//            if (union.Count != this.path.Count) return false;

//            return true;
//        }

//        public override string ToString()
//        {
//            StringBuilder str = new StringBuilder();

//            str.Append("Problem: { ");
//            foreach (int g in givens)
//            {
//                str.Append(g + " ");
//            }
//            str.Append("} -> " + goal);

//            str.Append("   Path: { ");
//            foreach (int g in givens)
//            {
//                str.Append(g + " ");
//            }
//            str.Append("}, { ");
//            foreach (int p in path)
//            {
//                str.Append(p + " ");
//            }
//            str.Append("} -> " + goal);

//            return str.ToString();
//        }

//        public string ConstructProblemAndSolution(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph)
//        {
//            // Determine the suppressed nodes in the graph and break
//            // the givens into those that must be explicitly stated to the user and those that are implicit.
//            foreach (int g in givens)
//            {
//                ConcreteAST.GroundedClause clause = graph.vertices[g].data;
//                if (clause.IsAxiomatic() || clause.IsIntrinsic())
//                {
//                    suppressedGivens.Add(g);
//                }
//            }
//            suppressedGivens.ForEach(s => givens.Remove(s));

//            //
//            // Sort the givens and path for ease in readability
//            //
//            givens.Sort();
//            suppressedGivens.Sort();
//            path.Sort();

//            StringBuilder str = new StringBuilder();

//            str.AppendLine("Source: ");
//            foreach (int g in givens)
//            {
//                str.AppendLine("\t (" + g + ")" + graph.GetNode(g).ToString());
//            }
//            str.AppendLine("Suppressed Source: ");
//            foreach (int s in suppressedGivens)
//            {
//                str.AppendLine("\t (" + s + ")" + graph.GetNode(s).ToString());
//            }
//            str.AppendLine("HyperEdges:");
//            foreach (PebblerHyperEdge edge in edges)
//            {
//                str.AppendLine("\t" + edge.ToString());
//            }
//            str.AppendLine("  Path:");
//            foreach (int p in path)
//            {
//                str.AppendLine("\t (" + p + ")" + graph.GetNode(p).ToString());
//            }

//            str.Append("  -> Goal: (" + goal + ")" + graph.GetNode(goal).ToString());

//            return str.ToString();
//        }
    }
}
