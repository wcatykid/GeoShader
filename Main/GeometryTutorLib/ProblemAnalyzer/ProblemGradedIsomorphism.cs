using System;
using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.ProblemAnalyzer
{
    // This aggregation class stores a single set of problems defined by the query vector
    // With strict isomorphism, this is one element of an equivalence relation (where the relation is defined by the query vector)
    public class ProblemGradedIsomorphism
    {
        //
        // Graded Isomorphism Algorithm:
        //    1. Choose the larger problem to modify (based on number of edges).
        //    2. while the modified smaller problem is not strictly isomorphic to the modified larger problem
        //         a. Remove an arc from the larger problem.
        //         b. Perform contraction on the nodes connecting the edge.
        //              i. Contraction creates a set relationship with all the relations combining
        //              ii. That is, for all edge source nodes, union the relationship of the target node together
        //
        // Strict Isomorphism Algorithm:
        //    1. Basic criteria must equate:
        //        a. Number of Nodes
        //        b. Number of Edges
        //        c. Length of the problem (depth of the tree)
        //        d. Width of the problem (width of the tree)
        //    2. Once a topological equivalence is established.
        //        a. In a subset manner, all nodes of one problem must be a relational subset to the corresponding node.
        //        b. That is, if A is node in problem P1 and B is node in problem P2
        //           the nodes are strictly isomoprhic if A is relationally isomorphic to some element E in node B
        //

        //public static int GradedIsomorphism(Problem problem1, Problem problem2, Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph)
        //{
        //    // 1. Choose the larger problem to modify (based on number of edges).
        //    Problem modifiedProblem = problem1.edges.Count > problem2.edges.Count ? problem1 : problem2;
        //    Problem staticProblem = problem1.edges.Count > problem2.edges.Count ? problem2 : problem1;

        //    GradedProblemTree problemToContract = new GradedProblemTree(modifiedProblem, graph);
        //    GradedProblemTree staticProblemTree = new GradedProblemTree(staticProblem, graph);

        //    while (!problemToContract.IsStrictlyIsomorphic(staticProblemTree))
        //    {

        //    }

        //    //

        //    //
        //    // while()
        //    //
        //}















        // To access node value information; mapping problem values back to the Geometric Graph
        //Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;

        //List<Problem> partition;

        //public ProblemPartition(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g)
        //{
        //    graph = g;
        //    partition = new List<Problem>();
        //}

        //public int Size()
        //{
        //    return partition.Count;
        //}

        ////
        //// CTA: This should add the problem UNIQUELY???
        ////
        //public void Add(Problem p)
        //{
        //    partition.Add(p);
        //}

        ////
        //// Given a problem and query vector determine strict isomorphism between this problem and this partition of problems
        ////
        //public bool IsStrictlyIsomorphic(Problem newProblem, QueryFeatureVector query)
        //{
        //    if (partition.Count == 0) return false;

        //    if (query.goalIsomorphism)
        //    {
        //        if (!AreNodesIsomorphic(partition[0].goal, newProblem.goal)) return false;
        //    }

        //    //
        //    // Add other query checks here....
        //    //

        //    return true;
        //}

        //// Given two nodes, determine if they are isomorphic according to our definition:
        //// Two nodes are isomorphic if they are of the same type and the same relation
        //// instance with the same strength.
        //private bool AreNodesIsomorphic(int node1, int node2)
        //{
        //    // A node is strongly isomorphic to itself
        //    if (node1 == node2) return true;

        //    // Do the nodes have the same type: algebraic or geometric?
        //    if (!IsSameType(node1, node2)) return false;

        //    // Are the nodes strongly related?
        //    return StronglyRelated(node1, node2);
        //}

        ////
        //// Do the nodes have the same type: algebraic or geometric?
        ////
        //private bool IsSameType(int node1, int node2)
        //{
        //    GroundedClause firstNode = graph.GetNode(node1);
        //    GroundedClause secondNode = graph.GetNode(node2);

        //    return firstNode.IsGeometric() && secondNode.IsGeometric() ||
        //           firstNode.IsAlgebraic() && secondNode.IsAlgebraic();
        //}

        ///// <summary>
        ///// Determines if the given nodes are the same regardless of type, but considering the type of relationship.
        ///// </summary>
        ///// <param name="node1">Integer representation of the node in the hypergraph</param>
        ///// <param name="node2">Integer representation of the node in the hypergraph</param>
        ///// <returns>This function will return FALSE if one node is congruence of segments and the other is congruence of triangles</returns>
        //private bool StronglyRelated(int node1, int node2)
        //{
        //    // Check the weak relationship first
        //    if (!WeaklyRelated(node1, node2)) return false;

        //    GroundedClause firstNode = graph.GetNode(node1);
        //    GroundedClause secondNode = graph.GetNode(node2);

        //    return StronglyRelated(firstNode, secondNode);
        //}

        //private bool StronglyRelated(GroundedClause firstNode, GroundedClause secondNode)
        //{
        //    if (firstNode is Congruent && secondNode is Congruent)
        //    {
        //        if (firstNode is CongruentAngles && secondNode is CongruentAngles) return true;
        //        if (firstNode is CongruentSegments && secondNode is CongruentSegments) return true;
        //        if (firstNode is CongruentTriangles && secondNode is CongruentTriangles) return true;
        //        return false;
        //    }

        //    if (firstNode is Equation && secondNode is Equation)
        //    {
        //        if (firstNode is AngleEquation && secondNode is AngleEquation) return true;
        //        if (firstNode is SegmentEquation && secondNode is SegmentEquation) return true;
        //        return false;
        //    }

        //    //
        //    // We may strenghthen for many reasons (right triangle, isosceles triangle as well as perpendicular bisector)
        //    // Compare those strengthened values for a relationship
        //    //
        //    if (firstNode is Strengthened && secondNode is Strengthened)
        //    {
        //        Strengthened streng1 = firstNode as Strengthened;
        //        Strengthened streng2 = secondNode as Strengthened;

        //        return StronglyRelated(streng1.strengthened, streng2.strengthened);
        //    }

        //    //if (firstNode is Similar)
        //    //{
        //    //    return secondNode is Similar;
        //    //}

        //    return firstNode.GetType() == secondNode.GetType();
        //}


        ///// <summary>
        ///// Determines if the given nodes are the same regardless of type or the specific type of relationship.
        ///// </summary>
        ///// <param name="node1">Integer representation of the node in the hypergraph</param>
        ///// <param name="node2">Integer representation of the node in the hypergraph</param>
        ///// <returns>That is, this function will return TRUE if one node is congruence of segments and the other is congruence of triangles</returns>
        //private bool WeaklyRelated(int node1, int node2)
        //{
        //    GroundedClause firstNode = graph.GetNode(node1);
        //    GroundedClause secondNode = graph.GetNode(node2);

        //    //System.Diagnostics.Debug.WriteLine("Related Nodes? " + firstNode.ToString() + " " + secondNode.ToString());

        //    if (firstNode is Congruent)
        //    {
        //        return secondNode is Congruent;
        //    }

        //    if (firstNode is Equation)
        //    {
        //        return secondNode is Equation;
        //    }

        //    //if (firstNode is Similar)
        //    //{
        //    //    return secondNode is Similar;
        //    //}

        //    return firstNode.GetType() == secondNode.GetType();
        //}

        //// For debugging purposes
        //public override string ToString()
        //{
        //    String retS = "";

        //    foreach (Problem p in partition)
        //    {
        //        retS += p.ConstructProblemAndSolution(graph) + "\n\n";
        //    }

        //    return retS;
        //}
    }
}
