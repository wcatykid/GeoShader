using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTutorLib.GeometryTestbed
{
    public abstract class ActualProofProblem : ActualProblem
    {

        //
        // Statistics Generation
        //
        // All statistics returned from the analysis
        protected StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats;

        //
        // Aggregation variables for all <figure, given, goal> pairings.
        //
        public static int TotalPoints = 0;
        public static int TotalSegments = 0;
        public static int TotalInMiddle = 0;
        public static int TotalIntersections = 0;
        public static int TotalAngles = 0;
        public static int TotalTriangles = 0;
        public static int TotalQuadrilaterals = 0;
        public static int TotalCircles = 0;
        public static int TotalImplicitFacts = 0;
        public static int TotalExplicitFacts = 0;

        public static System.TimeSpan TotalTime = new System.TimeSpan();
        public static int TotalGoals = 0;
        public static int TotalProblemsGenerated = 0;
        public static int TotalBackwardProblemsGenerated = 0;
        public static int TotalStrictInterestingProblems = 0;
        public static int TotalInterestingProblems = 0;
        public static int TotalOriginalBookProblems = 0;

        public static double TotalProblemWidth = 0;
        public static double TotalProblemLength = 0;
        public static double TotalDeducedSteps = 0;

        public static double TotalStrictProblemWidth = 0;
        public static double TotalStrictProblemLength = 0;
        public static double TotalStrictDeducedSteps = 0;

        public static int TotalGoalPartitions = 0;
        public static int TotalSourcePartitions = 0;
        public static int[] totalDifficulty = new int[GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds().Count + 1];
        public static int[] totalInteresting = new int[GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds().Count + 1];
        public static int[] totalStrictDifficulty = new int[GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds().Count + 1];
        public static int[] totalStrictInteresting = new int[GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds().Count + 1];

        public ActualProofProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
        }

        public override void Run()
        {
            if (!problemIsOn) return;

            else System.Diagnostics.Debug.WriteLine("----------------------------- " + this.problemName + " -----------------------------");

            // Map the set of clauses from parser to one set of intrinsic clauses.
            ConstructIntrinsicSet();

            // Create the analyzer
            StatisticsGenerator.HardCodedFigureAnalyzerMain analyzer = new StatisticsGenerator.HardCodedFigureAnalyzerMain(intrinsic, given, goals);

            // Perform and time the analysis
            figureStats = analyzer.AnalyzeFigure();

            //
            // If we know it's complete, keep that overridden completeness.
            // Otherwise, determine completeness through analysis of the nodes in the hypergraph.
            //
            if (!this.isComplete) this.isComplete = figureStats.isComplete;

            System.Diagnostics.Debug.WriteLine("Resultant Complete: " + this.isComplete +"\n");


            // Calculate the final numbers: counts of the k-G Strictly interesting problems.
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            for (int k = 1; k <= StatisticsGenerator.FigureStatisticsAggregator.MAX_K; k++)
            {
                str.Append(figureStats.kGcardinalities[k] + "\t");
            }

            if (this.isComplete)
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\complete.txt", true))
                //{
                //    file.WriteLine(str);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\completeTime.txt", true))
                //{
                //    file.WriteLine(figureStats.stopwatch.Elapsed);
                //}
            }
            else
            {
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\interesting.txt", true))
                //{
                //    file.WriteLine(str);
                //}
                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\interestingTime.txt", true))
                //{
                //    file.WriteLine(figureStats.stopwatch.Elapsed);
                //}
            }

            // Add to the cumulative statistics
            ActualProofProblem.TotalTime = ActualProofProblem.TotalTime.Add(figureStats.stopwatch.Elapsed);

            ActualProofProblem.TotalPoints += figureStats.numPoints;
            ActualProofProblem.TotalSegments += figureStats.numPoints;
            ActualProofProblem.TotalInMiddle += figureStats.numInMiddle;
            ActualProofProblem.TotalAngles += figureStats.numAngles;
            ActualProofProblem.TotalTriangles += figureStats.numTriangles;
            ActualProofProblem.TotalIntersections += figureStats.numIntersections;
            ActualProofProblem.TotalImplicitFacts += figureStats.totalImplicitFacts;

            ActualProofProblem.TotalExplicitFacts += figureStats.totalExplicitFacts;

            ActualProofProblem.TotalGoals += goals.Count;
            ActualProofProblem.TotalProblemsGenerated += figureStats.totalProblemsGenerated;
            ActualProofProblem.TotalBackwardProblemsGenerated += figureStats.totalBackwardProblemsGenerated;

            // Query: Interesting Partitioning
            int numProblemsInPartition;
            List<int> upperBounds = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProofProblem.totalInteresting[i] += figureStats.interestingPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProofProblem.totalInteresting[upperBounds.Count] += figureStats.interestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;

            upperBounds = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProofProblem.totalStrictInteresting[i] += figureStats.strictInterestingPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProofProblem.totalStrictInteresting[upperBounds.Count] += figureStats.strictInterestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;


            // Rest of Cumulative Stats
            ActualProofProblem.TotalStrictInterestingProblems += figureStats.totalStrictInterestingProblems;
            ActualProofProblem.TotalInterestingProblems += figureStats.totalInterestingProblems;
            ActualProofProblem.TotalOriginalBookProblems += goals.Count;

            // Averages
            ActualProofProblem.TotalDeducedSteps += figureStats.averageProblemDeductiveSteps * figureStats.totalInterestingProblems;
            ActualProofProblem.TotalProblemLength += figureStats.averageProblemLength * figureStats.totalInterestingProblems;
            ActualProofProblem.TotalProblemWidth += figureStats.averageProblemWidth * figureStats.totalInterestingProblems;
            ActualProofProblem.TotalStrictDeducedSteps += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemDeductiveSteps * figureStats.totalStrictInterestingProblems;
            ActualProofProblem.TotalStrictProblemLength += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemLength * figureStats.totalStrictInterestingProblems;
            ActualProofProblem.TotalStrictProblemWidth += figureStats.totalStrictInterestingProblems == 0 ? 0 : figureStats.strictAverageProblemWidth * figureStats.totalStrictInterestingProblems;

            // Queries
            ActualProofProblem.TotalGoalPartitions += figureStats.goalPartitionSummary.Count;
            ActualProofProblem.TotalSourcePartitions += figureStats.sourcePartitionSummary.Count;

            // Query: Difficulty Partitioning
            upperBounds = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProofProblem.totalDifficulty[i] += figureStats.difficultyPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProofProblem.totalDifficulty[upperBounds.Count] += figureStats.difficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;

            upperBounds = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            for (int i = 0; i < upperBounds.Count; i++)
            {
                ActualProofProblem.totalStrictDifficulty[i] += figureStats.strictDifficultyPartitionSummary.TryGetValue(upperBounds[i], out numProblemsInPartition) ? numProblemsInPartition : 0;
            }
            ActualProofProblem.totalStrictDifficulty[upperBounds.Count] += figureStats.strictDifficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
        }

        public override string ToString()
        {
            string statsString = "";

            //
            // Totals and Averages
            //
            statsString += this.problemName + ":\t";

            statsString += figureStats.numPoints + "\t";
            statsString += figureStats.numSegments + "\t";
            statsString += figureStats.numInMiddle + "\t";
            statsString += figureStats.numIntersections + "\t";
            statsString += figureStats.numAngles + "\t";
            statsString += figureStats.numTriangles + "\t";
            statsString += figureStats.totalImplicitFacts + "\t";

            statsString += figureStats.totalExplicitFacts + "\t";

            statsString += this.goals.Count + "\t";
            statsString += figureStats.totalBookProblemsGenerated + "\t";
            statsString += figureStats.totalProblemsGenerated + "\t";
            statsString += figureStats.totalInterestingProblems + "\t";
            statsString += figureStats.totalStrictInterestingProblems + "\t";
            statsString += figureStats.totalBackwardProblemsGenerated + "\t";

            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemWidth);
            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemLength);
            statsString += System.String.Format("{0:N2}\t", figureStats.averageProblemDeductiveSteps);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemWidth);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemLength);
            statsString += System.String.Format("{0:N2}\t", figureStats.strictAverageProblemDeductiveSteps);

            // Format and display the elapsed time for this problem
            statsString += System.String.Format("{0:00}:{1:00}.{2:00}",
                                                figureStats.stopwatch.Elapsed.Minutes,
                                                figureStats.stopwatch.Elapsed.Seconds, figureStats.stopwatch.Elapsed.Milliseconds / 10);

            //
            // Sample Query Output
            //
            // Goal Isomorphism
            //
            statsString += "\t\t" + figureStats.goalPartitionSummary.Count + "\t";

            // Source Isomorphism
            statsString += figureStats.sourcePartitionSummary.Count + "\t|\t";

            // Difficulty Partitioning
            int numProblemsInPartition;
            foreach (int upperBound in GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds())
            {
                statsString +=  figureStats.difficultyPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.difficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";

            foreach (int upperBound in GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds())
            {
                statsString += figureStats.strictDifficultyPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.strictDifficultyPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";


            // Interesting Partitioning
            foreach (int upperBound in GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds())
            {
                statsString += figureStats.interestingPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.interestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t|\t";

            // Strict Interesting Partitioning
            foreach (int upperBound in GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds())
            {
                statsString += figureStats.strictInterestingPartitionSummary.TryGetValue(upperBound, out numProblemsInPartition) ? numProblemsInPartition : 0;
                statsString += "\t";
            }
            statsString += figureStats.strictInterestingPartitionSummary.TryGetValue(int.MaxValue, out numProblemsInPartition) ? numProblemsInPartition : 0;
            statsString += "\t";

            return statsString;
        }
    }
}