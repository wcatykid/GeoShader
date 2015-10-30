using System;
using System.Collections.Generic;
using GeometryTutorLib.Pebbler;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // Implements a hashtable of problems
    //   + Hashing is based on the goal node ONLY
    //   + Problems will appear once in the table
    //   + No resizing / rehashing is needed because we will know ahead of time how many nodes there are
    //     in the hypergraph and problems are based on nodes in the graph
    //
    public class ProblemHashMap<A>
    {
        private readonly int TABLE_SIZE;
        private List<Problem<A>>[] table;
        private bool[] generated;
        public int size { get; private set; }
        private Pebbler.HyperEdgeMultiMap<A> edgeDatabase;
        private readonly int MAX_GIVENS;
        // private readonly bool FORWARD_GENERATION;
        private readonly int DEFAULT_MAX_BACKWARD_GIVENS = 2;

        // If the user specifies the size, we will never have to rehash
        public ProblemHashMap(Pebbler.HyperEdgeMultiMap<A> edges, int sz, int maxGivens)
        {
            size = 0;
            TABLE_SIZE = sz;

            table = new List<Problem<A>>[TABLE_SIZE];
            generated = new bool[TABLE_SIZE];

            edgeDatabase = edges;
            MAX_GIVENS = maxGivens;
            // FORWARD_GENERATION = true;
        }

        public ProblemHashMap(Pebbler.HyperEdgeMultiMap<A> edges, int sz)
        {
            size = 0;
            TABLE_SIZE = sz;

            table = new List<Problem<A>>[TABLE_SIZE];
            generated = new bool[TABLE_SIZE];

            edgeDatabase = edges;
            MAX_GIVENS = DEFAULT_MAX_BACKWARD_GIVENS;
            // FORWARD_GENERATION = false;
        }

        public bool HasNodeBeenGenerated(int node) { return generated[node]; }
        public void SetGenerated(int node)
        {
            // If the node results in no problems, create an empty list
            if (table[node] == null) table[node] = new List<Problem<A>>(); 
            generated[node] = true;
        }

        //
        // Get a single list of all the problems
        //
        public List<Problem<A>> GetAll()
        {
            List<Problem<A>> all = new List<Problem<A>>();

            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] != null) all.AddRange(table[i]);
            }

            if (all.Count != size)
            {
                throw new Exception("Unexpectedly the number of problems advertised (" + size + ")is inconsistent with the actual (" + all.Count + ") number of problems.");
            }

            return all;
        }

        // Add a list of problems to the structure unchecked (we assume all have the same goal node)
        public void PutUnchecked(List<Problem<A>> problems)
        {
            int goal = problems[0].goal;
            foreach (Problem<A> problem in problems)
            {
                if (problem.goal != goal)
                {
                    throw new ArgumentException("Not all unchecked problems have the same goal: " + goal + " " + problem.goal);
                }
            }

            if (table[problems[0].goal] == null)
            {
                table[problems[0].goal] = new List<Problem<A>>(problems);
                size += problems.Count;
            }
            else
            {
                // table[problems[0].goal].AddRange(problems);
                throw new ArgumentException("We don't expect to add problems in this UNCHECKED manner (with goal): " + problems[0].goal);
            }
        }

        // Can any edge in the edgeDatabase be used to deduce any of the givens?
        private PebblerHyperEdge<A> BasicMinimality(Problem<A> newProblem)
        {
            // Combine the givens and path into a single list
            List<int> givensAndPath = new List<int>(newProblem.givens);
            givensAndPath.AddRange(newProblem.path);

            // For each given, determine if it could have been simply deduced from a combination of given and path nodes
            foreach (int given in newProblem.givens)
            {
                // Acquire all edges with this given as a goal
                List<PebblerHyperEdge<A>> edges = edgeDatabase.GetBasedOnGoal(given);

                if (edges != null)
                {
                    // Analyze each basic edge to see if all of the hyperedge nodes are in the givens or path
                    foreach (PebblerHyperEdge<A> edge in edges)
                    {
                        if (Utilities.Subset<int>(givensAndPath, edge.sourceNodes)) return edge;
                    }

                }
            }

            return null;
        }

        //
        // Add the problem to all source node hash values
        //
        public void Put(Problem<A> newProblem)
        {
            if (newProblem.givens.Count > MAX_GIVENS) return;

            //  Check that no edges may be used to deduce any given in the problem
            Pebbler.PebblerHyperEdge<A> edge = BasicMinimality(newProblem);
            if (edge != null)
            {
                throw new ArgumentException("The following problem is not minimal since " + edge + " can deduce a given in " + newProblem);
            }

            // It is important that only when problems are added to the list, the new lists are allocated
            // Keeping null lists is important to determine if no problems exist vs. exploration has not occurred
            if (table[newProblem.goal] == null)
            {
                table[newProblem.goal] = new List<Problem<A>>();
                table[newProblem.goal].Add(newProblem);
                size++;
                return;
            }

            //
            // We verify minimality here for problems in the map
            // We note that the goals equate already; we are verifying the givens (not the suppressed)
            // Based on this criteria for entry in the table, a problem may only equate with a single other problem in terms of goal / sources
            List<Problem<A>> oldProblems = table[newProblem.goal];
            for (int p = 0; p < oldProblems.Count; p++)
            {
                // Check if the givens from the minimal problem and this candidate problem equate exactly
                if (Utilities.EqualSets<int>(newProblem.givens, oldProblems[p].givens))
                {
                    // Choose the shorter problem (fewer edges wins)
                    if (newProblem.edges.Count < oldProblems[p].edges.Count)
                    {
                        if (Utilities.PROBLEM_GEN_DEBUG) System.Diagnostics.Debug.WriteLine("In ProblemHashMap, removing problem " + oldProblems[p] + " for " + newProblem);

                        // Remove the old problem and add the new problem
                        table[newProblem.goal].RemoveAt(p);
                        table[newProblem.goal].Add(newProblem);
                    }
                    // else the list remains unchanged

                    // Either way, we are done.
                    return;
                }
                // Check if the givens from new problem are a subset of the givens of the minimal problem.
                else if (Utilities.Subset<int>(newProblem.givens, oldProblems[p].givens))
                {
                    if (Utilities.PROBLEM_GEN_DEBUG)
                    {
                        System.Diagnostics.Debug.WriteLine("Filtering for Minimal Givens: " + newProblem.ToString() + " for " + oldProblems[p].ToString());
                    }

                    return;
                }
                // Check if the givens from new problem are a subset of the givens of the minimal problem.
                else if (Utilities.Subset<int>(oldProblems[p].givens, newProblem.givens))
                {
                    if (Utilities.PROBLEM_GEN_DEBUG)
                    {
                        System.Diagnostics.Debug.WriteLine("Filtering for Minimal Givens: " + oldProblems[p].ToString() + " for " + newProblem.ToString());
                    }
                    table[newProblem.goal].RemoveAt(p);
                    table[newProblem.goal].Add(newProblem);
                    return;
                }
            }

            // No problems did equate; so add this new problem
            table[newProblem.goal].Add(newProblem);
            size++;
        }

        // Acquire a problem based on the goal only
        public List<Problem<A>> Get(Problem<A> p)
        {
            return Get(p.goal);
        }

        //
        // Another option to acquire the pertinent problems
        //
        public List<Problem<A>> Get(int key)
        {
            if (key < 0 || key >= TABLE_SIZE)
            {
                throw new ArgumentException("ProblemHashMap(" + key + ")");
            }

            return table[key];
        }

        public override string ToString()
        {
            String retS = "";

            for (int ell = 0; ell < TABLE_SIZE; ell++)
            {
                if (table[ell] != null)
                {
                    retS += ell + ":\n";
                    foreach (Problem<A> problem in table[ell])
                    {
                        retS += problem.ToString() + "\n";
                    }
                }
            }

            return retS;
        }
    }
}