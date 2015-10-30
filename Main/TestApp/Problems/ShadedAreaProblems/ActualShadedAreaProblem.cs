using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using LiveGeometry.TutorParser;

namespace GeometryTestbed
{
    public abstract class ActualShadedAreaProblem : ActualProblem
    {
        // Atomic regions
        public List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> goalRegions { get; protected set; }

        // Known values stated in the problem.
        public GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator known { get; protected set; }

        private double solutionArea;
        protected void SetSolutionArea(double a) { solutionArea = a; }
        public double GetSolutionArea() { return solutionArea; }

        //
        // Statistics Generation
        //
        // All statistics returned from the analysis
        protected StatisticsGenerator.ShadedAreaFigureStatisticsAggregator figureStats;

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
        public static int TotalTotalProperties = 0;
        public static int TotalExplicitFacts = 0;

        public static System.TimeSpan TotalTime = new System.TimeSpan();

        public ActualShadedAreaProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
            goalRegions = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
            known = new GeometryTutorLib.Area_Based_Analyses.KnownMeasurementsAggregator();

            solutionArea = -1;
        }

        public override void Run()
        {
            if (!this.problemIsOn) return;

            // Map the set of clauses from parser to one set of intrinsic clauses.
            ConstructIntrinsicSet();

            // Create the analyzer
            HardCodedShadedAreaMain analyzer = new HardCodedShadedAreaMain(intrinsic, given, known, goalRegions, this.parser.implied, GetSolutionArea());

            // Perform and time the analysis
            figureStats = analyzer.AnalyzeFigure();





            //
            // If we know it's complete, keep that overridden completeness.
            // Otherwise, determine completeness through analysis of the nodes in the hypergraph.
            //
            if (!this.isComplete) this.isComplete = figureStats.isComplete;

            //System.Diagnostics.Debug.WriteLine("Resultant Complete: " + this.isComplete +"\n");


            if (this.isComplete)
            {
            }
            else
            {
            }

            // Add to the cumulative statistics
            ActualShadedAreaProblem.TotalTime = ActualShadedAreaProblem.TotalTime.Add(figureStats.stopwatch.Elapsed);

            ActualShadedAreaProblem.TotalPoints += figureStats.numPoints;
            ActualShadedAreaProblem.TotalSegments += figureStats.numPoints;
            ActualShadedAreaProblem.TotalInMiddle += figureStats.numInMiddle;
            ActualShadedAreaProblem.TotalAngles += figureStats.numAngles;
            ActualShadedAreaProblem.TotalTriangles += figureStats.numTriangles;
            ActualShadedAreaProblem.TotalIntersections += figureStats.numIntersections;
            ActualShadedAreaProblem.TotalTotalProperties += figureStats.totalProperties;

            ActualShadedAreaProblem.TotalExplicitFacts += figureStats.totalExplicitFacts;
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
            statsString += figureStats.totalProperties + "\t";

            statsString += figureStats.totalExplicitFacts + "\t";

            statsString += this.goals.Count + "\t";

            // Format and display the elapsed time for this problem
            statsString += System.String.Format("{0:00}:{1:00}.{2:00}",
                                                figureStats.stopwatch.Elapsed.Minutes,
                                                figureStats.stopwatch.Elapsed.Seconds, figureStats.stopwatch.Elapsed.Milliseconds / 10);

            return statsString;
        }
    }
}