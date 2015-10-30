using System;
using System.Collections.Generic;

namespace GeometryTutorLib.ProblemAnalyzer
{
    public class QueryFeatureVector
    {
        public bool lengthPartitioning { get; private set; }
        public bool widthPartitioning { get; private set; }
        public bool deductiveStepsPartitioning { get; private set; }

        public bool rangedLengthPartitioning { get; private set; }
        public NumericPartition<int> lengthPartitions { get; private set; }
        public bool rangedWidthPartitioning { get; private set; }
        public NumericPartition<int> widthPartitions { get; private set; }
        public bool rangedDeductiveStepsPartitioning { get; private set; }
        public NumericPartition<int> stepsPartitions { get; private set; }

        public int minLength { get; private set; }
        public int maxLength { get; private set; }
        public int minWidth { get; private set; }
        public int maxWidth { get; private set; }

        // The number of desired goals from the original figure statement
        public int numberOfOriginalGivens { get; private set; }

        public bool interestingPartitioning { get; private set; }
        public NumericPartition<int> interestingPartitions { get; private set; }

        public bool sourceIsomorphism { get; private set; }
        public bool pathIsomorphism { get; private set; }
        public bool goalIsomorphism { get; private set; }

        //
        // By default, we turn everything off and make the widths / lengths a large range of values
        //
        public QueryFeatureVector()
        {
            lengthPartitioning = false;
            rangedLengthPartitioning = false;
            widthPartitioning = false;
            rangedWidthPartitioning = false;
            deductiveStepsPartitioning = false;
            rangedDeductiveStepsPartitioning = false;

            lengthPartitions = new NumericPartition<int>();
            widthPartitions = new NumericPartition<int>();
            stepsPartitions = new NumericPartition<int>();
            interestingPartitions = new NumericPartition<int>();

            interestingPartitioning = false;
            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = false;
        }

        public QueryFeatureVector(List<int> lengthParts, List<int> widthParts, List<int> stepParts)
        {
            lengthPartitioning = false;
            rangedLengthPartitioning = false;
            widthPartitioning = false;
            rangedWidthPartitioning = false;
            deductiveStepsPartitioning = false;
            rangedDeductiveStepsPartitioning = false;

            lengthPartitions = new NumericPartition<int>(lengthParts);
            widthPartitions = new NumericPartition<int>(widthParts);
            stepsPartitions = new NumericPartition<int>(stepParts);
            interestingPartitions = new NumericPartition<int>();

            interestingPartitioning = false;
            sourceIsomorphism = false;
            pathIsomorphism = false;
            goalIsomorphism = false;
        }

        //
        // A wrapper class to represent integer partitioning of values:
        //     ubs[0] ubs[1] ubs[2] ubs[3]
        //  [     |     |      |      |     ]
        //
        // Note: this is an inequality check implementation; for example we use <= to compare with the upper bound, NOT <
        //
        public class NumericPartition<T> where T : IComparable
        {
            private List<T> partitionUpperBounds;

            public NumericPartition() { partitionUpperBounds = new List<T>(); }

            public NumericPartition(List<T> ubs)
            {
                partitionUpperBounds = new List<T>(ubs);

                partitionUpperBounds.Sort();
            }

            public void SetPartitions(List<T> ubs)
            {
                partitionUpperBounds = new List<T>(ubs);

                partitionUpperBounds.Sort();
            }

            public T GetUpperBound(int index) { return partitionUpperBounds[index]; }
            public int Size() { return partitionUpperBounds.Count; }

            // Given the specific value, we reutrn the index; this facilitates comparing
            // indices to determine if two problems are in the same dictated partitions
            public int GetPartitionIndex(T value)
            {
                // General case with > 1 partition value
                for (int p = 0; p < partitionUpperBounds.Count; p++)
                {
                    if (value.CompareTo(partitionUpperBounds[p]) <= 0) return p;
                }

                // The value is greater than all given partitions (also handles case where only 1 partition value given)
                return partitionUpperBounds.Count;
            }
        }

        //
        // --------------------------- Factories for various query vectors -----------------------------------
        //
        public static List<int> ConstructDifficultyPartitionBounds()
        {
            List<int> partitionBounds = new List<int>();
            partitionBounds.Add(2);
            partitionBounds.Add(5);
            partitionBounds.Add(10);
            return partitionBounds;
        }

        public static List<int> ConstructInterestingPartitionBounds()
        {
            List<int> partitionBounds = new List<int>();
            partitionBounds.Add(25);
            partitionBounds.Add(50);
            partitionBounds.Add(75);
            partitionBounds.Add(99);
            partitionBounds.Add(100);
            return partitionBounds;
        }

        public static QueryFeatureVector ConstructGoalIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.goalIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructSourceIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.sourceIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructSourceAndGoalIsomorphismQueryVector()
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.sourceIsomorphism = true;
            query.goalIsomorphism = true;

            return query;
        }

        public static QueryFeatureVector ConstructDeductiveBasedIsomorphismQueryVector(List<int> partitions)
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.deductiveStepsPartitioning = true;
            query.rangedDeductiveStepsPartitioning = true;

            query.stepsPartitions.SetPartitions(partitions);

            return query;
        }

        public static QueryFeatureVector ConstructDeductiveGoalIsomorphismQueryVector(List<int> partitions)
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.deductiveStepsPartitioning = true;
            query.rangedDeductiveStepsPartitioning = true;

            query.goalIsomorphism = true;

            query.stepsPartitions.SetPartitions(partitions);

            return query;
        }

        public static QueryFeatureVector ConstructInterestingnessIsomorphismQueryVector(List<int> partitions)
        {
            QueryFeatureVector query = new QueryFeatureVector();

            query.interestingPartitioning = true;

            query.interestingPartitions.SetPartitions(partitions);

            return query;
        }

        public override string ToString()
        {
            string retS = "[ ";

            if (sourceIsomorphism) retS += "Source Isomorphism";
            if (pathIsomorphism) retS += "Path Isomorphism";
            if (goalIsomorphism) retS += "Goal Isomorphism";

            return retS + " ]";
        }
    }
}
