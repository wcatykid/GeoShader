using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    // This aggregation class stores a single set of problems defined by the query vector
    // With strict isomorphism, this is one element of an equivalence relation (where the relation is defined by the query vector)
    public class ProblemEquivalenceClass
    {
        // To access node value information; mapping problem values back to the Geometric Graph
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;
        public List<Problem<Hypergraph.EdgeAnnotation>> elements { get; private set; }

        public ProblemEquivalenceClass(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        {
            graph = g;
            elements = new List<Problem<Hypergraph.EdgeAnnotation>>();
        }

        public int Size()
        {
            return elements.Count;
        }

        public void Add(Problem<Hypergraph.EdgeAnnotation> p)
        {
            elements.Add(p);
        }

        //
        // Given a problem and query vector determine strict isomorphism between this problem and this partition of problems
        //
        public bool IsStrictlyIsomorphic(Problem<Hypergraph.EdgeAnnotation> newProblem, QueryFeatureVector query)
        {
            //
            // GOAL
            //
            if (query.goalIsomorphism)
            {
                if (!AreNodesIsomorphic(elements[0].goal, newProblem.goal)) return false;
            }

            //
            // LENGTH
            //
            if (query.lengthPartitioning)
            {
                if (query.rangedLengthPartitioning)
                {
                    if (!AreRangedEqualLength(query, elements[0], newProblem)) return false;
                }
                else
                {
                    if (!AreEqualLength(elements[0], newProblem)) return false;
                }
            }

            //
            // WIDTH
            //
            if (query.widthPartitioning)
            {
                if (query.rangedWidthPartitioning)
                {
                    if (!AreRangedEqualWidth(query, elements[0], newProblem)) return false;
                }
                else
                {
                    if (!AreEqualWidth(elements[0], newProblem)) return false;
                }
            }

            //
            // DEDUCTIVE STEPS
            //
            if (query.deductiveStepsPartitioning)
            {
                if (query.rangedDeductiveStepsPartitioning)
                {
                    if (!AreRangedEqualDeductiveSteps(query, elements[0], newProblem)) return false;
                }
                else
                {
                    if (!AreEqualDeductiveSteps(elements[0], newProblem)) return false;
                }
            }

            //
            // Add other query checks here....
            //

            //
            // Interestingness query (% of givens covered)
            //
            if (query.interestingPartitioning)
            {
                if (!AreRangedEqualInteresting(query, elements[0], newProblem)) return false;
            }

            //
            // SOURCE NODE 
            //
            if (query.sourceIsomorphism)
            {
                if (!AreSourceNodesIsomorphic(elements[0].givens, newProblem.givens)) return false;
            }

            return true;
        }

        private bool AreEqualLength(Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return thisProblem.GetLength() == thatProblem.GetLength();
        }

        private bool AreEqualWidth(Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return thisProblem.GetWidth() == thatProblem.GetWidth();
        }

        private bool AreEqualDeductiveSteps(Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return thisProblem.GetNumDeductiveSteps() == thatProblem.GetNumDeductiveSteps();
        }

        private bool AreRangedEqualLength(QueryFeatureVector query, Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return query.lengthPartitions.GetPartitionIndex(thisProblem.GetLength()) == query.lengthPartitions.GetPartitionIndex(thatProblem.GetLength());
        }

        private bool AreRangedEqualWidth(QueryFeatureVector query, Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return query.widthPartitions.GetPartitionIndex(thisProblem.GetWidth()) == query.widthPartitions.GetPartitionIndex(thatProblem.GetWidth());
        }

        private bool AreRangedEqualDeductiveSteps(QueryFeatureVector query, Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return query.stepsPartitions.GetPartitionIndex(thisProblem.GetNumDeductiveSteps()) ==
                   query.stepsPartitions.GetPartitionIndex(thatProblem.GetNumDeductiveSteps());
        }

        private bool AreRangedEqualInteresting(QueryFeatureVector query, Problem<Hypergraph.EdgeAnnotation> thisProblem, Problem<Hypergraph.EdgeAnnotation> thatProblem)
        {
            return query.interestingPartitions.GetPartitionIndex(thisProblem.interestingPercentage) ==
                   query.interestingPartitions.GetPartitionIndex(thatProblem.interestingPercentage);
        }

        //
        // A set of nodes are isomorphic if:
        //   1. They have the same cardinality
        //   2. A permutation creates a 1-to-1 node isomorphism
        //
        private bool AreSourceNodesIsomorphic(List<int> src1, List<int> src2)
        {
            // Same Cardinality required
            if (src1.Count != src2.Count) return false;

            // Simple verification of singleton sets
            if (src1.Count == 1) return AreNodesIsomorphic(src1[0], src2[0]);

            // Match all nodes in s1 with a distinct term in s2
            bool[] marked = new bool[src1.Count];
            for (int s1 = 0; s1 < src1.Count - 1; s1++)
            {
                bool foundThisS1Node = false;
                for (int s2 = 0; s2 < src2.Count; s2++)
                {
                    if (!marked[s2])
                    {
                        if (AreNodesIsomorphic(src1[s1], src2[s2]))
                        {
                            foundThisS1Node = true;
                            marked[s2] = true;
                            break;
                        }
                    }
                }

                if (!foundThisS1Node) return false;
            }

            return !marked.Contains(true);


            //// Acquire the set of possible isomorpshisms for each node in list 1
            //List<List<int>> nodeIsos = GetNodeIsomorphimIndices(src1, src2);

            //// A null indicates failure to match a node in src1 with anything in src2
            //if (nodeIsos == null) return false;

            //// Match a subset exactly to indicate an isomorphism.
            //return SetEqualityPossible(nodeIsos);
        }

        //
        // Collect all the nodes of list 2 which are isomorphic to a node in list 1
        //
        //private List<List<int>> GetNodeIsomorphimIndices(List<int> src1, List<int> src2)
        //{
        //    List<List<int>> nodeIsos = new List<List<int>>();

        //    for (int s1 = 0; s1 < src1.Count; s1++)
        //    {
        //        List<int> s2NodesIsoToS1Indices = new List<int>();

        //        for (int s2 = 0; s2 < src2.Count; s2++)
        //        {
        //            if (AreNodesIsomorphic(src1[s1], src2[s2])) s2NodesIsoToS1Indices.Add(s2);
        //        }

        //        // This node did not match any
        //        if (!s2NodesIsoToS1Indices.Any()) return null;

        //        nodeIsos.Add(s2NodesIsoToS1Indices);
        //    }

        //    return nodeIsos;
        //}

        //
        // Does there exist a direct isomorphism between two sets;
        // Use the pair of sets to determine if there exists a unique covering set
        //
        // 
        //
        //
        //private bool SetEqualityPossible(List<List<int>> pairs)
        //{
        //    int[] set = new int[pairs.Count];
        //    int[] indexInSet = new int[pairs.Count];

        //    // Init to unused
        //    for (int s = 0; s < set.Length; s++)
        //    {
        //        set[s] = -1;
        //    }

        //    // Collect all singleton elements
        //    for (int pList = 0; pList < pairs.Count; pList++)
        //    {
        //        if (pairs[pList].Count == 1)
        //        {
        //            // Two singletons use the same node
        //            if (set.Contains(pairs[pList][0])) return false;

        //            set[pList] = pairs[pList][0];
        //            indexInSet[pList] = 0;
        //        }
        //    }

        //    // For all other non-singleton sets, can we find a combination that creates the desired, unique matching set
        //    //foreach (List<int> pair in pairs)
        //    //{
        //    //    if (pair.Count != 1)
        //    //    {
        //    //        bool added = false;
        //    //        foreach (int p in pair)
        //    //        {
        //    //            if (!set.Contains(p))
        //    //            {
        //    //                set.Add(p);
        //    //                added = true;
        //    //            }
        //    //        }

        //    //        if (!added) return false;
        //    //    }
        //    //}

        //    //
        //    // Check for non-(-1) and uniqueness
        //    //
        //    for (int s1 = 0; s1 < set.Length; s1++)
        //    {
        //        // Unused
        //        if (set[s1] == -1) return false;

        //        // Uniqueness
        //        for (int s2 = 0; s2 < set.Length; s2++)
        //        {
        //            if (set[s1] == set[s2]) return false;
        //        }
        //    }

        //    return true;
        //}

        //// Return the index of the added element
        //private int TryPermutation(List<int> set, List<int> newList)
        //{
        //    bool added = false;
        //    foreach (int p in newList)
        //    {
        //        if (!set.Contains(p))
        //        {
        //            set.Add(p);
        //            added = true;
        //        }
        //    }

        //    if (!added) return -1;

        //    return 0;
        //}


        // Given two nodes, determine if they are isomorphic according to our definition:
        // Two nodes are isomorphic if they are of the same type and the same relation
        // instance with the same strength.
        private bool AreNodesIsomorphic(int node1, int node2)
        {
            // A node is strongly isomorphic to itself
            if (node1 == node2) return true;

            // Do the nodes have the same type: algebraic or geometric?
            if (!IsSameType(node1, node2)) return false;

            // Are the nodes strongly related?
            return StronglyRelated(node1, node2);
        }

        //
        // Do the nodes have the same type: algebraic or geometric?
        //
        private bool IsSameType(int node1, int node2)
        {
            GroundedClause firstNode = graph.GetNode(node1);
            GroundedClause secondNode = graph.GetNode(node2);

            return firstNode.IsGeometric() && secondNode.IsGeometric() ||
                   firstNode.IsAlgebraic() && secondNode.IsAlgebraic();
        }

        /// <summary>
        /// Determines if the given nodes are the same regardless of type, but considering the type of relationship.
        /// </summary>
        /// <param name="node1">Integer representation of the node in the hypergraph</param>
        /// <param name="node2">Integer representation of the node in the hypergraph</param>
        /// <returns>This function will return FALSE if one node is congruence of segments and the other is congruence of triangles</returns>
        private bool StronglyRelated(int node1, int node2)
        {
            // Check the weak relationship first
            if (!WeaklyRelated(node1, node2)) return false;

            GroundedClause firstNode = graph.GetNode(node1);
            GroundedClause secondNode = graph.GetNode(node2);

            return StronglyRelated(firstNode, secondNode);
        }

        private bool StronglyRelated(GroundedClause firstNode, GroundedClause secondNode)
        {
            if (firstNode is SegmentRatio && secondNode is SegmentRatio) return true;
            if (firstNode is ProportionalAngles && secondNode is ProportionalAngles) return true;

            if (firstNode is Congruent && secondNode is Congruent)
            {
                if (firstNode is CongruentAngles && secondNode is CongruentAngles) return true;
                if (firstNode is CongruentSegments && secondNode is CongruentSegments) return true;
                if (firstNode is CongruentTriangles && secondNode is CongruentTriangles) return true;
                return false;
            }

            if (firstNode is Equation && secondNode is Equation)
            {
                if (firstNode is AngleEquation && secondNode is AngleEquation) return true;
                if (firstNode is SegmentEquation && secondNode is SegmentEquation) return true;
                return false;
            }

            //
            // We may strenghthen for many reasons (right triangle, isosceles triangle as well as perpendicular bisector)
            // Compare those strengthened values for a relationship
            //
            if (firstNode is Strengthened && secondNode is Strengthened)
            {
                Strengthened streng1 = firstNode as Strengthened;
                Strengthened streng2 = secondNode as Strengthened;

                return StronglyRelated(streng1.strengthened, streng2.strengthened);
            }

            //if (firstNode is Similar)
            //{
            //    return secondNode is Similar;
            //}

            return firstNode.GetType() == secondNode.GetType();
        }


        /// <summary>
        /// Determines if the given nodes are the same regardless of type or the specific type of relationship.
        /// </summary>
        /// <param name="node1">Integer representation of the node in the hypergraph</param>
        /// <param name="node2">Integer representation of the node in the hypergraph</param>
        /// <returns>That is, this function will return TRUE if one node is congruence of segments and the other is congruence of triangles</returns>
        private bool WeaklyRelated(int node1, int node2)
        {
            GroundedClause firstNode = graph.GetNode(node1);
            GroundedClause secondNode = graph.GetNode(node2);

            //System.Diagnostics.Debug.WriteLine("Related Nodes? " + firstNode.ToString() + " " + secondNode.ToString());

            if (firstNode is Congruent)
            {
                return secondNode is Congruent;
            }

            if (firstNode is Equation)
            {
                return secondNode is Equation;
            }

            //if (firstNode is Similar)
            //{
            //    return secondNode is Similar;
            //}

            return firstNode.GetType() == secondNode.GetType();
        }


        // For debugging purposes
        public override string ToString()
        {
            String retS = "";

            foreach (Problem<Hypergraph.EdgeAnnotation> p in elements)
            {
                retS += p.ConstructProblemAndSolution(graph) + "\n\n";
            }

            return retS;
        }
    }
}
