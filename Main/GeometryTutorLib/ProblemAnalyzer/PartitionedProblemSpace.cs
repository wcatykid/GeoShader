using System;
using System.Linq;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    //
    // This class facilitates storing groups of problems.
    // So, if a set of problems are alike in some manner, they are easily accessible
    // using feature vector access
    //
    public class PartitionedProblemSpace
    {
        // To access node value information; mapping problem values back to the Geometric Graph
        Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph;

        // The list of equivalent problem sets as defined by the query vector
        List<ProblemEquivalenceClass> partitions;
        QueryFeatureVector query;

        private int totalProblems;

        public PartitionedProblemSpace(Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> g, QueryFeatureVector query)
        {
            graph = g;
            partitions = new List<ProblemEquivalenceClass>();
            this.query = query;
            totalProblems = 0;
        }

        public void ConstructPartitions(List<Problem<Hypergraph.EdgeAnnotation>> problems)
        {
            totalProblems += problems.Count;
            //
            // For each problem, add to the appropriate partition based on the query vector
            //
            foreach (Problem<Hypergraph.EdgeAnnotation> problem in problems)
            {
                bool added = false;
                foreach (ProblemEquivalenceClass partition in partitions)
                {
                    if (partition.IsStrictlyIsomorphic(problem, query))
                    {
                        partition.Add(problem);
                        added = true;
                        break; // If this problem is in several partitions, we should delete this; this is more applicable with non-strict isomorphism
                    }
                }
                // If this problem was not added into a partition, create a new partition
                if (!added)
                {
                    ProblemEquivalenceClass newPartition = new ProblemEquivalenceClass(graph);
                    newPartition.Add(problem);
                    partitions.Add(newPartition);
                }
            }
        }

        //
        // Validate that we have generated all of the original problems from the text (based strictly on givens and goals)
        //
        public List<Problem<Hypergraph.EdgeAnnotation>> ValidateOriginalProblems(List<ConcreteAST.GroundedClause> givens, List<ConcreteAST.GroundedClause> goals)
        {
#if RELEASE //Define in Properties->Build->Compilation Symbols to turn off this section
            if (!goals.Any()) return;
#else
            if (!goals.Any())
            {
                throw new ArgumentException("Specified problem does not have any goals.");
            }
#endif
            // Acquire the indices of the givens
            List<int> givenIndices = new List<int>();
            foreach (ConcreteAST.GroundedClause given in givens)
            {
                int givenIndex = (given is ConcreteAST.Equation) ? graph.GetNodeIndex(given) : Utilities.StructuralIndex(graph, given);
                if (givenIndex == -1) throw new Exception("FATAL ERROR: GIVEN Node not found!!!" + given);
                givenIndices.Add(givenIndex);
            }

            //Acquire the indices of the goals
            List<int> goalIndices = new List<int>();
            foreach (ConcreteAST.GroundedClause goal in goals)
            {
                int goalIndex = (goal is ConcreteAST.Equation) ? graph.GetNodeIndex(goal) : Utilities.StructuralIndex(graph, goal);
                if (goalIndex == -1) throw new Exception("FATAL ERROR: Goal Node not deduced!!!" + goal);
                goalIndices.Add(goalIndex);
            }

            //
            // Search through all the partitiions in the space for matching problems
            // We specifically seek problems with the same givens so we can then check the goals
            //
            List<Problem<Hypergraph.EdgeAnnotation>> validatedProblems = new List<Problem<Hypergraph.EdgeAnnotation>>();
            bool[] marked = new bool[goals.Count];
            foreach (ProblemEquivalenceClass partition in partitions)
            {
                foreach (Problem<Hypergraph.EdgeAnnotation> problem in partition.elements)
                {
                    // Does this problem have one of the goals?
                    if (goalIndices.Contains(problem.goal))
                    {
                        // Does this problem have a subset of the givens?
                        if (Utilities.Subset<int>(givenIndices, problem.givens))  // if (Utilities.EqualSets<int>(problem.givens, givenIndices))
                        {
                            // Success, we generated a book problem directly
                            // We ensured previously that there does not exist more than one problem with the same given set and goal node;
                            // therefore, we need not track that this problem is matched with a different problem.
                            validatedProblems.Add(problem);
                            marked[goalIndices.IndexOf(problem.goal)] = true;
                        }
                    }
                }
            }

            if (marked.Contains(false))
            {
                string report = "";

                for (int g = 0; g < goals.Count; g++)
                {
                    if (!marked[g]) report += goals[g] + "\n";
                }
 
                throw new Exception("Not all problems from the book were validated: " + report);
            }

            return validatedProblems;
        }

        //
        // Construct a list of all partitions summarizing the number of problems with same goal type: <type, number of type>
        //
        public List<KeyValuePair<ConcreteAST.GroundedClause, int>> GetGoalBasedPartitionSummary()
        {
            List<KeyValuePair<ConcreteAST.GroundedClause, int>> data = new List<KeyValuePair<ConcreteAST.GroundedClause, int>>();

            foreach (ProblemEquivalenceClass partition in partitions)
            {
                data.Add(new KeyValuePair<ConcreteAST.GroundedClause, int>(graph.vertices[partition.elements[0].goal].data, partition.Size()));
            }

            return data;
        }

        //
        // Construct a list of all partitions summarizing the number of problems with same goal type: <type, number of type>
        //
        public List<int> GetPartitionSummary()
        {
            List<int> partitionSizes = new List<int>();

            foreach (ProblemEquivalenceClass partition in partitions)
            {
                partitionSizes.Add(partition.Size());
            }

            return partitionSizes;
        }

        //
        // Construct a list of all partitions summarizing the number of problems with same goal type: <type, number of type>
        //
        public Dictionary<int, int> GetDifficultyPartitionSummary()
        {
            Dictionary<int, int> partitionPairs = new Dictionary<int, int>();

            // It is possible that there will be NO problems in an anticipated partition
            for (int p = 0; p < partitions.Count; p++)
            {
                int upperBoundIndex = query.stepsPartitions.GetPartitionIndex(partitions[p].elements[0].GetNumDeductiveSteps());
                int upperBoundValue = upperBoundIndex == query.stepsPartitions.Size() ? int.MaxValue : query.stepsPartitions.GetUpperBound(upperBoundIndex);

                partitionPairs.Add(upperBoundValue, partitions[p].Size());
            }

            return partitionPairs;
        }

        public Dictionary<int, int> GetInterestingPartitionSummary()
        {
            Dictionary<int, int> partitionPairs = new Dictionary<int, int>();

            // It is possible that there will be NO problems in an anticipated partition
            for (int p = 0; p < partitions.Count; p++)
            {
                int upperBoundIndex = query.interestingPartitions.GetPartitionIndex(partitions[p].elements[0].interestingPercentage);
                int upperBoundValue = upperBoundIndex == query.interestingPartitions.Size() ? int.MaxValue : query.interestingPartitions.GetUpperBound(upperBoundIndex);

                partitionPairs.Add(upperBoundValue, partitions[p].Size());
            }

            return partitionPairs;
        }

        // For debugging purposes
        public void DumpPartitions()
        {
            for (int p = 0; p < partitions.Count; p++)
            {
                System.Diagnostics.Debug.WriteLine("Partition (" + (p + 1) + ") contains " + partitions[p].Size() + " problems.\n" + partitions[p].ToString());
            }

            System.Diagnostics.Debug.WriteLine("Using query " + query.ToString() + ", there are " + partitions.Count + " partitions with " + totalProblems + " problems.\n");
        }
    }
}