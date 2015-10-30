using System.Collections.Generic;
using System.Diagnostics;

namespace GeometryTutorLib.GeometryTestbed
{
    public class MainProgram
    {
        //
        // Clean the data directory.
        //
        private static void CleanDataDirectory()
        {
            string dataDir = @"C:\Users\ctalvin\Desktop\output";
            string[] txtList = System.IO.Directory.GetFiles(dataDir, "*.txt");

            foreach (string f in txtList)
            {
                System.Diagnostics.Debug.WriteLine("Deleting: " + f);
                System.IO.File.Delete(f);
            }
        }

        static void Main(string[] args)
        {
            CleanDataDirectory();

#if FIGURE_SYNTHESIZER
            GeometryTutorLib.FigureSynthesizerMain.SynthesizerMainTester();
            //GeometryTutorLib.FigureSynthesizerMain.SynthesizerMain();
            //GeometryTutorLib.FigureSynthesizerMain.SynthesizerMainDataGeneration();
#else
            List<GeometryTestbed.ActualProblem> problems = ConstructAllGeoShaderHardCodedProblems();

            DumpGeoShaderStatisticsHeader();

            int problemCount = 0;
            foreach (GeometryTestbed.ActualProblem problem in problems)
            {
                if (problem.problemIsOn) // We may turn problems on / off: check on
                {
                    problem.Run();

                    Debug.Write(++problemCount + "\t");
                    Debug.WriteLine(problem.ToString() + "\n");
                }
            }

            DumpGeoShaderStatisticsHeader();
            DumpGeoShaderAggregateTotals(problemCount);
#endif
        }

        private static List<GeometryTutorLib.GeometryTestbed.ActualProblem> ConstructAllGeoShaderHardCodedProblems()
        {
            return ShadedAreaProblems.GetProblems();
        }

        private static void DumpGeoShaderStatisticsHeader()
        {
            string header = "";
            header += "Problem #\t\t";
            header += "Name\t\t\t\t  ";
            header += "Imp(Fig)\t";
            header += "Exp(Fig)\t";

            header += "Shapes\t";
            header += "Root\t";
            header += "Atoms\t";

            header += "Implicit Time\t";
            header += "GeoTutor Time\t";
            header += "Solver Time\t";

            header += "Int Probs\t";
            header += "Complete\t";

            header += "Orig Interesting\t";

            Debug.WriteLine(header);
        }

        private static void DumpGeoShaderAggregateTotals(int numFigures)
        {
            string output = "";

            output += "------------------------------------------------------------------------------------------------------------------------------------------------------------------\n";

            output += "\t\t" + numFigures + "\t\t\t\t\t\t\t\t";

            output += GeometryTestbed.ActualShadedAreaProblem.TotalImplicitFacts + "\t\t   ";
            output += GeometryTestbed.ActualShadedAreaProblem.TotalExplicitFacts + "\t\t";

            output += GeometryTestbed.ActualShadedAreaProblem.TotalShapes + "\t\t";
            output += GeometryTestbed.ActualShadedAreaProblem.TotalRootShapes + "\t\t";
            output += GeometryTestbed.ActualShadedAreaProblem.TotalAtomicRegions + "\t\t\t";

            output += Utilities.TimeToString(ActualShadedAreaProblem.TotalImplicitTime) + "\t\t";
            output += Utilities.TimeToString(ActualShadedAreaProblem.TotalDeductionTime) + "\t\t";
            output += Utilities.TimeToString(ActualShadedAreaProblem.TotalSolverTime) + "\t";

            output += GeometryTestbed.ActualShadedAreaProblem.TotalInteresting + "\t\t   ";
            output += GeometryTestbed.ActualShadedAreaProblem.TotalComplete + "\t\t\t";

            output += GeometryTestbed.ActualShadedAreaProblem.TotalOriginalInteresting + "\t";

            Debug.WriteLine(output);
        }

        //
        //
        //
        // GeoTutor statistics code.
        //
        //
        //
        private static List<GeometryTutorLib.GeometryTestbed.ActualProblem> ConstructAllHardCodedGeoTutorProblems()
        {
            List<ActualProblem> problems = new List<ActualProblem>();

            problems.AddRange(GeometryTestbed.JurgensenProblems.GetProblems());
            problems.AddRange(GeometryTestbed.GlencoeProblems.GetProblems());
            problems.AddRange(GeometryTestbed.IndianTextProblems.GetProblems());
            problems.AddRange(GeometryTestbed.HoltWorkbookProblems.GetProblems());
            problems.AddRange(GeometryTestbed.McDougallProblems.GetProblems());
            problems.AddRange(GeometryTestbed.McDougallWorkbookProblems.GetProblems());
            problems.AddRange(GeometryTestbed.KiteProblems.GetProblems());
            problems.AddRange(GeometryTestbed.AngleArcProblems.GetProblems());

            return problems;
        }

        private static void DumpGeoTutorStatisticsHeader()
        {
            string header = "";
            header += "Problem #\t";
            header += "Name\t\t\t";
            header += "Points\t";
            header += "Segments\t";
            header += "InMiddle\t";
            header += "Intersections\t";
            header += "Angles\t";
            header += "Triangles\t";
            header += "TotalProperties\t";

            header += "Tot Explicit Facts\t";

            header += "# Book Probs\t";
            header += "# Probs\t";
            header += "# of Int Probs\t";
            header += "# of Strict Int Probs\t";
            header += "# Backward Probs\t";

            header += "Ave Width\t";
            header += "Ave Length\t";
            header += "Ave Deductive\t";
            header += "Ave Strict Width\t";
            header += "Ave Strict Length\t";
            header += "Ave Strict Deductive\t";
            header += "Time To Generate\t";

            header += "# Goal Partitions\t";
            header += "# Src Partitions\t\t";
            header += "Difficulty Partitions: ";
            List<int> difficultyPartitions = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds();
            header += "0-" + difficultyPartitions[0] + "\t";
            for (int i = 0; i < difficultyPartitions.Count - 1; i++)
            {
                header += (difficultyPartitions[i] + 1) + "-" + difficultyPartitions[i + 1] + "\t";
            }
            header += ">=" + (difficultyPartitions[difficultyPartitions.Count - 1] + 1) + "\t";

            header += "Strict Difficulty Partitions: ";
            header += "0-" + difficultyPartitions[0] + "\t";
            for (int i = 0; i < difficultyPartitions.Count - 1; i++)
            {
                header += (difficultyPartitions[i] + 1) + "-" + difficultyPartitions[i + 1] + "\t";
            }
            header += ">=" + (difficultyPartitions[difficultyPartitions.Count - 1] + 1) + "\t";

            header += "Interesting Partitions: ";
            List<int> interestingPartitions = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds();
            header += "0-" + interestingPartitions[0] + "\t";
            for (int i = 0; i < interestingPartitions.Count - 1; i++)
            {
                header += interestingPartitions[i] + "-" + interestingPartitions[i + 1] + "\t";
            }
            header += ">=" + interestingPartitions[interestingPartitions.Count - 1] + "\t";

            header += "Strict Interesting Partitions: ";
            header += "0-" + interestingPartitions[0] + "\t";
            for (int i = 0; i < interestingPartitions.Count - 1; i++)
            {
                header += interestingPartitions[i] + "-" + interestingPartitions[i + 1] + "\t";
            }
            header += ">=" + interestingPartitions[interestingPartitions.Count - 1] + "\t";

            Debug.WriteLine(header);
        }

        private static void DumpGeoTutorAggregateTotals(int numFigures)
        {
            string output = "";

            output += "----------------------------------------- Summary -----------------------------------------\n";

            output += numFigures + "\t\t\t\t\t\t\t";

            output += GeometryTestbed.ActualProofProblem.TotalPoints + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalSegments + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalInMiddle + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalIntersections + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalAngles + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalTriangles + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalImplicitFacts + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalPoints + "\t || ";
            //output += GeometryTestbed.ActualProofProblem.TotalCircles + "\t";
            //output += GeometryTestbed.ActualProofProblem.TotalQuadrilaterals + "\t";

            output += GeometryTestbed.ActualProofProblem.TotalExplicitFacts + "\t || ";

            output += GeometryTestbed.ActualProofProblem.TotalOriginalBookProblems + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalGoals + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalProblemsGenerated + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalInterestingProblems + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalStrictInterestingProblems + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalBackwardProblemsGenerated + "\t";

            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalProblemWidth / GeometryTestbed.ActualProofProblem.TotalInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalProblemLength / GeometryTestbed.ActualProofProblem.TotalInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalDeducedSteps / GeometryTestbed.ActualProofProblem.TotalInterestingProblems);

            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalStrictProblemWidth / GeometryTestbed.ActualProofProblem.TotalStrictInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalStrictProblemLength / GeometryTestbed.ActualProofProblem.TotalStrictInterestingProblems);
            output += System.String.Format("{0:N2}\t", GeometryTestbed.ActualProofProblem.TotalStrictDeducedSteps / GeometryTestbed.ActualProofProblem.TotalStrictInterestingProblems);


            output += System.String.Format("{0:00}:{1:00}.{2:00}",
                                           GeometryTestbed.ActualProofProblem.TotalTime.Minutes,
                                           GeometryTestbed.ActualProofProblem.TotalTime.Seconds,
                                           GeometryTestbed.ActualProofProblem.TotalTime.Milliseconds / 10);

            // Queries
            output += "\t" + GeometryTestbed.ActualProofProblem.TotalGoalPartitions + "\t";
            output += GeometryTestbed.ActualProofProblem.TotalSourcePartitions +"\t||\t";

            // Query: Difficulty Partitioning
            foreach (int numProbs in GeometryTestbed.ActualProofProblem.totalDifficulty)
            {
                output += numProbs + "\t";
            }

            output += "||\t";
            // Query: Strict Difficulty Partitioning
            foreach (int numProbs in GeometryTestbed.ActualProofProblem.totalStrictDifficulty)
            {
                output += numProbs + "\t";
            }

            output += "||\t";
            // Query: Interesting Partitioning
            foreach (int numProbs in GeometryTestbed.ActualProofProblem.totalInteresting)
            {
                output += numProbs + "\t";
            }

            output += "||\t";
            // Query: Strict Interesting Partitioning
            foreach (int numProbs in GeometryTestbed.ActualProofProblem.totalStrictInteresting)
            {
                output += numProbs + "\t";
            }

            Debug.WriteLine(output);
        }
    }
}