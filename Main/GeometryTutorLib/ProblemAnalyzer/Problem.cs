using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Pebbler;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // A problem is defined as a the sub-hypergraph from a set of source nodes to a goal node
    //
    public class Problem<A>
    {
        // Problem Statement
        public List<int> givens { get; private set; }
        public int goal { get; private set; }

        // Path from start of problem to end of problem
        public List<int> suppressedGivens { get; private set; }

        // Path from start of problem to end of problem
        public List<int> path { get; private set; }
        public List<PebblerHyperEdge<A>> edges { get; private set; }
        public DiGraph graph { get; private set; }

        // For final determination of interestingness
        public int interestingPercentage = 0;

        // For backward problem generation
        public Problem()
        {
            givens = new List<int>();
            goal = -1;

            path = new List<int>();
            edges = new List<PebblerHyperEdge<A>>();
            suppressedGivens = new List<int>();

            graph = new DiGraph();
        }

        public Problem(PebblerHyperEdge<A> edge)
        {
            givens = new List<int>(edge.sourceNodes);
            goal = edge.targetNode;

            path = new List<int>();
            edges = new List<PebblerHyperEdge<A>>();
            edges.Add(edge);

            suppressedGivens = new List<int>();

            graph = new DiGraph();
            graph.AddHyperEdge(givens, goal);
        }

        public Problem(Problem<A> thatProblem)
        {
            givens = new List<int>(thatProblem.givens);
            goal = thatProblem.goal;

            path = new List<int>(thatProblem.path);
            edges = new List<PebblerHyperEdge<A>>(thatProblem.edges);
            suppressedGivens = new List<int>(thatProblem.suppressedGivens);

            graph = new DiGraph(thatProblem.graph);
        }

        public int GetNumDeductiveSteps()
        {
            return edges.Count;
        }

        private int memoizedLength = -1;
        public int GetLength()
        {
            if (memoizedLength == -1) memoizedLength = graph.GetLength();
            return memoizedLength;
        }

        private int memoizedWidth = -1;
        public int GetWidth()
        {
            if (memoizedWidth == -1) memoizedWidth = graph.GetWidth();
            return memoizedWidth;
        }

        public bool ContainsGoalEdge(int targetNode)
        {
            foreach (PebblerHyperEdge<A> edge in edges)
            {
                if (edge.targetNode == targetNode) return true;
            }

            return false;
        }

        public bool ContainsCycle()
        {
            return graph.ContainsCycle();
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        //
        // Just a simple hashing mechanism
        //
        public long GetHashKey()
        {
            long key = 1;

            key *= givens[0];

            if (path.Any()) key *= path[0];

            key *= goal;

            return key;
        }

        public bool InSource(int n)
        {
            return givens.Contains(n);
        }

        public bool InPath(int n)
        {
            return path.Contains(n);
        }

        public bool HasGoal(int n)
        {
            return goal == n;
        }

        private void AddEdge(PebblerHyperEdge<A> edge)
        {
            // Add to the graph
            graph.AddHyperEdge(edge.sourceNodes, edge.targetNode);

            if (this.edges.Contains(edge)) return;

            // Add in an ordered manner according to the target node.
            int e = 0;
            for ( ; e < this.edges.Count; e++)
            {
                if (edge.targetNode < this.edges[e].targetNode) break;
            }

            this.edges.Insert(e, edge);
        }

        //
        // Create a new problem based on thisProblem and thatProblem in accordance with the above comments (repeated here)
        //
        // This problem                       { This Givens } { This Path } -> This Goal
        // The new problem is of the form:    { That Givens } { That Path } -> Goal
        //                       Combined:    { New Givens  U  This Givens \minus This Goal} {This Path  U  This Goal } -> Goal
        //
        public void Append(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph,
                           HyperEdgeMultiMap<A> forwardEdges, Problem<A> thatProblem)
        {
            if (thatProblem.goal == -1)
            {
                throw new ArgumentException("Attempt to append with an empty problem " + this + " " + thatProblem);
            }

            //
            // If this is an empty problem, populate it like a copy constructor and return
            //
            if (this.goal == -1)
            {
                givens = new List<int>(thatProblem.givens);
                goal = thatProblem.goal;

                path = new List<int>(thatProblem.path);
                edges = new List<PebblerHyperEdge<A>>(thatProblem.edges);

                suppressedGivens = new List<int>(thatProblem.suppressedGivens);

                thatProblem.edges.ForEach(edge => this.AddEdge(edge));
                return;
            }

            //
            // Standard appending of an existent problem to another existent problem
            //
            if (!this.givens.Contains(thatProblem.goal))
            {
                throw new ArgumentException("Attempt to append problems that do not connect goal->given" + this + " " + thatProblem);
            }

            // Degenerate by removing the new problem goal from THIS source node.
            this.givens.Remove(thatProblem.goal);

            // Add the 'new problem' goal node to the path of the new Problem (uniquely)
            Utilities.AddUnique<int>(this.path, thatProblem.goal);

            // Add the path nodes to THIS path
            Utilities.AddUniqueList<int>(this.path, thatProblem.path);

            // Add all the new sources to the degenerated old sources; do so uniquely
            Utilities.AddUniqueList<int>(this.givens, thatProblem.givens);
            Utilities.AddUniqueList<int>(this.suppressedGivens, thatProblem.suppressedGivens);

            // Add all of the edges of that problem to this problem; this also adds to the problem graph
            thatProblem.edges.ForEach(edge => this.AddEdge(edge));

            if (this.ContainsCycle())
            {
                throw new Exception("Problem contains a cycle" + this.graph.GetStronglyConnectedComponentDump());
                // Remove an edge from this problem?
            }

            // Now, if there exists a node in the path AND in the givens, remove it from the givens.
            foreach (int p in this.path)
            {
                if (this.givens.Remove(p))
                {
                    // if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("A node existed in the path AND givens (" + p + "); removing from givens");
                }
            }

            PerformDeducibilityCheck(forwardEdges);
        }

        //
        // The combination of new information may lead to other given information being deducible
        //
        //
        // foreach given in the problem
        //   find all edges with target given 
        //   foreach edge with target with given
        //     if (all of the source nodes in edge are in the given OR path) then
        //       if this is a minimal edge (fewer sources better) then
        //         save edge
        //   if (found edge) then
        //     AddEdge to problem
        //     move target given to path
        //       
        private void PerformDeducibilityCheck(HyperEdgeMultiMap<A> edgeDatabase)
        {
            // All the givens and path nodes for this problem; this includes the new edgeSources
            List<int> problemGivensAndPath = new List<int>(this.givens);
            problemGivensAndPath.AddRange(this.path);

            // foreach given in the problem
            
            List<int> tempGivens = new List<int>(this.givens); // Make a copy because we may be modifying it below
            foreach (int given in tempGivens)
            {
                PebblerHyperEdge<A> savedEdge = null;

                // find all edges with target given 
                List<PebblerHyperEdge<A>> forwardEdges = edgeDatabase.GetBasedOnGoal(given);
                if (forwardEdges != null)
                {
                    // foreach edge with target with given
                    foreach (PebblerHyperEdge<A> edge in forwardEdges)
                    {
                        // if (all of the source nodes in edge are in the given OR path) then
                        if (Utilities.Subset<int>(problemGivensAndPath, edge.sourceNodes))
                        {
                            // if this is a minimal edge (fewer sources better) then
                            if (savedEdge == null) savedEdge = edge;
                            else if (edge.sourceNodes.Count < savedEdge.sourceNodes.Count)
                            {
                                savedEdge = edge;
                            }
                        }
                    }

                    if (savedEdge != null)
                    {
                        // if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("CTA: Found another edge which can deduce givens." + savedEdge);

                        // Add the found edge to the problem
                        this.AddEdge(savedEdge);

                        // move target given to path: (1) remove from givens; (2) add to path 
                        this.givens.Remove(savedEdge.targetNode);
                        Utilities.AddUnique<int>(this.path, savedEdge.targetNode);
                    }
                }
            }
        }

        //
        // Problems are equal only if the givens, goal, and paths are the same
        //
        public override bool Equals(object obj)
        {
            Problem<Hypergraph.EdgeAnnotation> thatProblem = obj as Problem<Hypergraph.EdgeAnnotation>;
            if (thatProblem == null) return false;

            if (this.goal != thatProblem.goal) return false;

            if (this.givens.Count != thatProblem.givens.Count) return false;

            if (this.path.Count != thatProblem.path.Count) return false;

            // Union the sets; if the union is the same size as the original, they are the same
            List<int> union = new List<int>(this.givens);
            Utilities.AddUniqueList<int>(union, thatProblem.givens);
            if (union.Count != this.givens.Count) return false;

            union = new List<int>(this.path);
            Utilities.AddUniqueList<int>(union, thatProblem.path);
            if (union.Count != this.path.Count) return false;

            return true;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Given { ");
            foreach (int g in givens)
            {
                str.Append(g + " ");
            }
            str.Append("}, Path { ");
            foreach (int p in path)
            {
                str.Append(p + " ");
            }
            str.Append("} -> " + goal);

            return str.ToString();
        }

        //
        // Determine which of the given nodes will be suppressed
        //
        public void DetermineSuppressedGivens(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph)
        {
            // Determine the suppressed nodes in the graph and break
            // the givens into those that must be explicitly stated to the user and those that are implicit.
            foreach (int g in givens)
            {
                ConcreteAST.GroundedClause clause = graph.vertices[g].data;
                if (clause.IsAxiomatic() || clause.IsIntrinsic() || !clause.IsAbleToBeASourceNode())
                {
                    suppressedGivens.Add(g);
                }
            }
            suppressedGivens.ForEach(s => givens.Remove(s));
        }

        public string ConstructProblemAndSolution(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph)
        {
            // Sort the givens and path for ease in readability; they are reverse-sorted
            TopologicalSortProblem();

            StringBuilder str = new StringBuilder();

            str.AppendLine("Source: ");
            for (int g = givens.Count - 1; g >= 0; g--)
            {
                str.AppendLine("\t (" + givens[g] + ")" + graph.GetNode(givens[g]).ToString());
            }
            str.AppendLine("Suppressed Source: ");
            foreach (int s in suppressedGivens)
            {
                str.AppendLine("\t (" + s + ")" + graph.GetNode(s).ToString());
            }
            str.AppendLine("HyperEdges:");
            foreach (PebblerHyperEdge<A> edge in edges)
            {
                str.AppendLine("\t" + edge.ToString() + "\t" + edge.annotation.ToString());
            }
            str.AppendLine("  Path:");
            for (int p = path.Count - 1; p >= 0; p--)
            {
                str.AppendLine("\t (" + path[p] + ")" + graph.GetNode(path[p]).ToString());
            }

            str.Append("  -> Goal: (" + goal + ")" + graph.GetNode(goal).ToString());

            return str.ToString();
        }

        private void TopologicalSortProblem()
        {
            List<int> sortedGiven = new List<int>();
            List<int> sortedPath = new List<int>();

            List<int> sortedNodes = this.graph.TopologicalSort();

            foreach (int node in sortedNodes)
            {
                if (givens.Contains(node)) sortedGiven.Add(node);
                else if (path.Contains(node)) sortedPath.Add(node);
                else if (!suppressedGivens.Contains(node) && !goal.Equals(node))
                {
                    throw new ArgumentException("Node " + node + " is not in either given, suppressed, path, nor goal for " + this.ToString());
                }
            }

            givens = new List<int>(sortedGiven);
            path = new List<int>(sortedPath);
        }

        public string EdgeAndSCCDump()
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine("HyperEdges:");
            foreach (PebblerHyperEdge<A> edge in edges)
            {
                str.AppendLine("\t" + edge.ToString());
            }

            str.AppendLine(this.graph.GetStronglyConnectedComponentDump());

            return str.ToString();
        }
    }
}