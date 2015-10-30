using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Xml;

namespace StatisticsGenerator
{
    public class HardCodedFigureAnalyzerMain
    {
        // The problem parameters to analyze
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> figure;
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> givens;

        private GeometryTutorLib.Precomputer.CoordinatePrecomputer precomputer;
        private GeometryTutorLib.Hypergraph.Hypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> graph;
        private GeometryTutorLib.GenericInstantiator.Instantiator instantiator;
        private GeometryTutorLib.Pebbler.PebblerHypergraph<GeometryTutorLib.ConcreteAST.GroundedClause, GeometryTutorLib.Hypergraph.EdgeAnnotation> pebblerGraph;
        private GeometryTutorLib.Pebbler.Pebbler pebbler;
        private GeometryTutorLib.ProblemAnalyzer.PathGenerator pathGenerator;
        private GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator templateProblemGenerator = null;
        private GeometryTutorLib.ProblemAnalyzer.InterestingProblemCalculator interestingCalculator;
//        private GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector queryVector;
//        private GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace problemSpacePartitions;
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> goals;
//        private XmlWriter writer;

        public HardCodedFigureAnalyzerMain(List<GeometryTutorLib.ConcreteAST.GroundedClause> fs,
                                           List<GeometryTutorLib.ConcreteAST.GroundedClause> gs,
                                           List<GeometryTutorLib.ConcreteAST.GroundedClause> gls)
        {
            this.figure = fs;
            this.givens = gs;
            this.goals = gls;

            WriteProblemToXML();

            // Create the precomputer object for coordinate-based pre-comutation analysis
            precomputer = new GeometryTutorLib.Precomputer.CoordinatePrecomputer(figure);
            instantiator = new GeometryTutorLib.GenericInstantiator.Instantiator();
            //queryVector = new GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector(givens.Count);
        }

        private void WriteProblemToXML()
        {
            //XmlWriterSettings settings = new XmlWriterSettings();
            //settings.Indent = true;
            //using (XmlWriter writer = XmlWriter.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\TutorAIDump.xml", settings)) //%AppData%\Roaming
            //{
            //    //WORKAROUND: XmlWriter is readonly in using environment
            //    Action<string, List<GeometryTutorLib.ConcreteAST.GroundedClause>> write = null; //Allow passing write as paramater from within write
            //    write = (string s, List<GeometryTutorLib.ConcreteAST.GroundedClause> list) =>
            //     {
            //         writer.WriteStartElement(s);
            //         foreach (GeometryTutorLib.ConcreteAST.GroundedClause gc in list)
            //         {
            //             gc.DumpXML(write);
            //         }
            //         writer.WriteEndElement();
            //     };

            //    writer.WriteStartDocument();
            //    writer.WriteStartElement("Root");

            //    writer.WriteStartElement("Figure");
            //    write("component", figure);
            //    writer.WriteEndElement();

            //    writer.WriteStartElement("Givens");
            //    write("given", givens);
            //    writer.WriteEndElement();

            //    writer.WriteStartElement("Goals");
            //    write("goal", goals);
            //    writer.WriteEndElement();

            //    writer.WriteEndElement();
            //    writer.WriteEndDocument();
            //}
        }

        // Returns: <number of interesting problems, number of original problems generated>
        public ProofProblemFigureStatisticsAggregator AnalyzeFigure()
        {
            ProofProblemFigureStatisticsAggregator figureStats = new ProofProblemFigureStatisticsAggregator();

            // For statistical analysis, count the number of each particular type of implicit facts.
            CountIntrisicProperties(figureStats);

            // Start timing
            figureStats.stopwatch.Start();

            // Precompute all coordinate-based interesting relations (problem goal nodes)
            // These become the basis for the template-based problem generation (these are the goals)
            Precompute();

            // Handle givens that strengthen the intrinsic parts of the figure; modifies if needed
            givens = DoGivensStrengthenFigure();


            // Use a worklist technique to instantiate nodes to construct the hypergraph for this figure
            ConstructHypergraph();

            // Create the integer-based hypergraph representation
            ConstructPebblingHypergraph();

            // Pebble that hypergraph
            Pebble();

            // Analyze paths in the hypergraph to generate the pair of <forward problems, backward problems> (precomputed nodes are goals)
            KeyValuePair<List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>,
                         List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>> problems = GenerateTemplateProblems();

            // Combine the problems together into one list
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> candidateProbs = new List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>();
            candidateProbs.AddRange(problems.Key);
            //candidateProbs.AddRange(problems.Value); // converse

            figureStats.totalBackwardProblemsGenerated = problems.Value.Count;
            figureStats.totalProblemsGenerated = candidateProbs.Count;

            // Determine which, if any, of the problems are interesting (using definition that 100% of the givens are used)
            interestingCalculator = new GeometryTutorLib.ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens, goals);
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> interestingProblems = interestingCalculator.DetermineInterestingProblems(candidateProbs);
            figureStats.totalInterestingProblems = interestingProblems.Count;

            // Explicit number of facts: hypergraph size - figure facts
            figureStats.totalExplicitFacts = graph.vertices.Count - figure.Count;

            // Validate that the original book problems were generated.
            Validate(interestingProblems, figureStats);

            // Stop timing before we generate all of the statistics
            figureStats.stopwatch.Stop();


            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> strictlyInteresting = GetStrictlyInteresting(interestingProblems);

            // Construct partitions based on different queries
            // GenerateStatistics(interestingProblems, figureStats, strictlyInteresting);

            //
            // Is this figure complete? That is, do the assumptions define the figure completely?
            //
            figureStats.isComplete = templateProblemGenerator.DefinesFigure(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses());

            // Debug.WriteLine("Explicit Complete: " + figureStats.isComplete);

            // Construct the K-goal problems
            // CalculateKnonStrictCardinalities(interestingCalculator, interestingProblems, figureStats);

            return figureStats;
        }

        //
        // Given the problems with at least one assumption, construct ALL such combinations to form (I, G).
        //
        private void CalculateKnonStrictCardinalities(GeometryTutorLib.ProblemAnalyzer.InterestingProblemCalculator interestingCalculator,
                                                      List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems,
                                                      StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            // K-G  container: index 0 is 1-G, index 1 is 2-G, etc.
            List<List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>> KmgProblems = new List<List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>>();

            //
            // Create the new set of multigoal problems each with 1 goal: 1-G
            //
            List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> mgProblems = new List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>();
            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation> new1GProblem = new GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>();
                new1GProblem.AddProblem(problem);
                mgProblems.Add(new1GProblem);
            }

            // Add the 1-G problems to the K-G problem set.
            KmgProblems.Add(mgProblems);


            // Construct all of the remaining 
            CalculateKnonStrictCardinalities(KmgProblems, problems, ProofProblemFigureStatisticsAggregator.MAX_K);

            //
            // Now that we have 1, 2, ..., MAX_K -G multigoal problems, we must filter them.
            // That is, are the problems strictly interesting?
            //
            // Filtered K-G  container: index 0 is 1-G, index 1 is 2-G, etc.
            List<List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>> filteredKmgProblems = new List<List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>>();

            foreach (List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> originalKgProblems in KmgProblems)
            {
                filteredKmgProblems.Add(interestingCalculator.DetermineStrictlyInterestingMultiGoalProblems(originalKgProblems));
            }

            // Calculate the final numbers: counts of the k-G Strictly interesting problems.
            StringBuilder str = new StringBuilder();
            for (int k = 1; k <= ProofProblemFigureStatisticsAggregator.MAX_K; k++)
            {
                figureStats.kGcardinalities[k] = filteredKmgProblems[k - 1].Count;

                str.AppendLine(k + "-G: " + figureStats.kGcardinalities[k]);
            }

            Debug.WriteLine(str);

            if (GeometryTutorLib.Utilities.PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine(str);
            }
        }

        // Calculate k-G; a set of goals with k propositions
        private void CalculateKnonStrictCardinalities(List<List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>> kgProblems,
                                                      List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> interesting, int MAX_K)
        {
            for (int k = 2; k <= MAX_K; k++)
            {
                // (k-1)-G list: 
                List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> kMinus1Problems = kgProblems[k - 2];
                List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> kProblems = new List<GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>();

                // For each (k-1)-G problem, add each interesting problem, in turn.
                foreach (GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation> kMinus1Problem in kMinus1Problems)
                {
                    if (kMinus1Problem.givens.Count < givens.Count)
                    {
                        // For each k-G, make a copy of the (k-1)-G problem and add the interesting problem to it.
                        foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> sgProblem in interesting)
                        {
                            // Make a copy
                            GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation> newKGproblem = new GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation>(kMinus1Problem);

                            // Add the interesting problem to it; if the add is successful, the k-G problem is added to 
                            if (newKGproblem.AddProblem(sgProblem))
                            {
                                // Debug.WriteLine(kMinus1Problem.ToString() + " + " + sgProblem.ToString() + " = " + newKGproblem.ToString());

                                // Is this problem, based on (Givens, Goals) in this list already?
                                bool alreadyExists = false;
                                foreach (GeometryTutorLib.ProblemAnalyzer.MultiGoalProblem<GeometryTutorLib.Hypergraph.EdgeAnnotation> kProblem in kProblems)
                                {
                                    if (kProblem.HasSameGivensGoals(newKGproblem))
                                    {
                                        alreadyExists = true;
                                        break;
                                    }
                                }

                                if (!alreadyExists) kProblems.Add(newKGproblem);
                            }
                        }
                    }
                }

                // Add the complete set of k-G problems to the list
                kgProblems.Add(kProblems);
            }
        }


        private List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> GetStrictlyInteresting(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems)
        {
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> strictlyInteresting = new List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>();

            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                if (problem.interestingPercentage >= 100) strictlyInteresting.Add(problem);
            }

            return strictlyInteresting;
        }

        //
        // For statistical analysis only count the number of occurrences of each intrisic property.
        //
        private void CountIntrisicProperties(StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            foreach (GeometryTutorLib.ConcreteAST.GroundedClause clause in figure)
            {
                figureStats.totalImplicitFacts++;
                if (clause is GeometryTutorLib.ConcreteAST.Point) figureStats.numPoints++;
                else if (clause is GeometryTutorLib.ConcreteAST.InMiddle) figureStats.numInMiddle++;
                else if (clause is GeometryTutorLib.ConcreteAST.Segment) figureStats.numSegments++;
                else if (clause is GeometryTutorLib.ConcreteAST.Intersection) figureStats.numIntersections++;
                else if (clause is GeometryTutorLib.ConcreteAST.Triangle) figureStats.numTriangles++;
                else if (clause is GeometryTutorLib.ConcreteAST.Angle) figureStats.numAngles++;
                else if (clause is GeometryTutorLib.ConcreteAST.Quadrilateral) figureStats.numQuadrilaterals++;
                else if (clause is GeometryTutorLib.ConcreteAST.Circle) figureStats.numCircles++;
                else
                {
                    Debug.WriteLine("Did not count " + clause);
                    figureStats.totalImplicitFacts--;
                }
            }
        }

        //
        // Use threads to precompute all forward relations and strengthening 
        //
        private void Precompute()
        {
            //precomputer.CalculateRelations();
            //precomputer.CalculateStrengthening();

            Thread precomputeRelations = new Thread(new ThreadStart(precomputer.CalculateRelations));
            Thread precomputeStrengthening = new Thread(new ThreadStart(precomputer.CalculateStrengthening));

            // Start and indicate thread joins for these short computations threads
            try
            {
                precomputeRelations.Start();
                precomputeStrengthening.Start();
                precomputeRelations.Join();
                precomputeStrengthening.Join();
            }
            catch (ThreadStateException e)
            {
                Debug.WriteLine(e);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        //
        // Modify the given information to account for redundancy in stated nodes
        // That is, does given information strengthen a figure node?
        //
        private List<GeometryTutorLib.ConcreteAST.GroundedClause> DoGivensStrengthenFigure()
        {
            List<GeometryTutorLib.ConcreteAST.GroundedClause> modifiedGivens = new List<GeometryTutorLib.ConcreteAST.GroundedClause>();
            GeometryTutorLib.ConcreteAST.GroundedClause currentGiven = null;

            foreach (GeometryTutorLib.ConcreteAST.GroundedClause given in givens)
            {
                currentGiven = given;
                foreach (GeometryTutorLib.ConcreteAST.GroundedClause component in figure)
                {
                    if (component.CanBeStrengthenedTo(given))
                    {
                        currentGiven = new GeometryTutorLib.ConcreteAST.Strengthened(component, given);
                        break;
                    }
                }
                modifiedGivens.Add(currentGiven);
            }

            return modifiedGivens;
        }

        //
        // Construct the main Hypergraph
        //
        private void ConstructHypergraph()
        {
            // Resets all saved data to allow multiple problems
            GeometryTutorLib.GenericInstantiator.Instantiator.Clear();

            // Build the hypergraph through instantiation
            graph = instantiator.Instantiate(figure, givens);

            if (GeometryTutorLib.Utilities.DEBUG)
            {
                graph.DumpNonEquationClauses();
                graph.DumpEquationClauses();
            }
        }

        //
        // Create the Pebbler version of the hypergraph for analysis (integer-based hypergraph)
        //
        private void ConstructPebblingHypergraph()
        {
            // Create the Pebbler version of the hypergraph (all integer representation) from the original hypergraph
            pebblerGraph = graph.GetPebblerHypergraph();
        }

        //
        // Actually perform pebbling on the integer hypergraph (pebbles forward and backward)
        //
        private void Pebble()
        {
            pebbler = new GeometryTutorLib.Pebbler.Pebbler(graph, pebblerGraph);

            pathGenerator = new GeometryTutorLib.ProblemAnalyzer.PathGenerator(graph);

            // Acquire the integer values of the intrinsic / figure nodes
            List<int> intrinsicSet = GeometryTutorLib.Utilities.CollectGraphIndices(graph, figure);

            // Acquire the integer values of the givens (from the original 
            List<int> givenSet = GeometryTutorLib.Utilities.CollectGraphIndices(graph, givens);

            // Perform pebbling based on the <figure, given> pair.
            pebbler.Pebble(intrinsicSet, givenSet);

            if (GeometryTutorLib.Utilities.PEBBLING_DEBUG)
            {
                pebbler.DebugDumpEdges();
            }
        }

        //
        // Generate all of the problems based on the precomputed values (these precomputations are the problem goals)
        //
        private KeyValuePair<List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>,
                             List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>> GenerateTemplateProblems()
        {
            templateProblemGenerator = new GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator(graph, pebbler, pathGenerator);

            // Generate the problem pairs
            return templateProblemGenerator.Generate(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses(), givens);
        }

        //
        // Given, the list of generated, interesting problems, validate (for soundness) the fact that the original book problem was generated.
        // Do so by constructing a goal-based isomorphism partitioning and check that there exists a problem with the same given set.
        //
        private void Validate(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, ProofProblemFigureStatisticsAggregator figureStats)
        {
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector query = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructGoalIsomorphismQueryVector();

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace goalBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, query);
            
            goalBasedPartitions.ConstructPartitions(problems);

            // Validate that we have generated all of the original problems from the text.
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> generatedBookProblems = goalBasedPartitions.ValidateOriginalProblems(givens, goals);
            figureStats.totalBookProblemsGenerated = generatedBookProblems.Count;

            if (GeometryTutorLib.Utilities.PROBLEM_GEN_DEBUG)
            {
                goalBasedPartitions.DumpPartitions();
            }
            if (GeometryTutorLib.Utilities.BACKWARD_PROBLEM_GEN_DEBUG)
            {
                Debug.WriteLine("\nAll " + generatedBookProblems.Count + " Book-specified problems: \n");
                foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> bookProb in generatedBookProblems)
                {
                    Debug.WriteLine(bookProb.ConstructProblemAndSolution(graph));
                }
            }

            figureStats.goalPartitionSummary = goalBasedPartitions.GetGoalBasedPartitionSummary();
        }

        //
        // We may analyze the interesting problems constructing various partitions and queries
        //
        private void GenerateStatistics(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems,
            StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats,
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> strictlyInteresting)
        {
            GenerateAverages(problems, figureStats);
            GenerateIsomorphicStatistics(problems, figureStats);

            GenerateStrictAverages(strictlyInteresting, figureStats);
            GenerateStrictIsomorphicStatistics(strictlyInteresting, figureStats);

            GeneratePaperQuery(strictlyInteresting, figureStats);
        }

        private void GenerateAverages(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            int totalWidth = 0;
            int totalLength = 0;
            int totalDeductiveSteps = 0;

            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                totalWidth += problem.GetWidth();
                totalLength += problem.GetLength();
                totalDeductiveSteps += problem.GetNumDeductiveSteps();
            }

            figureStats.averageProblemWidth = ((double)(totalWidth)) / problems.Count;
            figureStats.averageProblemLength =((double)(totalLength)) / problems.Count;
            figureStats.averageProblemDeductiveSteps = ((double)(totalDeductiveSteps)) / problems.Count;
        }

        private void GenerateStrictAverages(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            figureStats.totalStrictInterestingProblems = problems.Count;

            int totalWidth = 0;
            int totalLength = 0;
            int totalDeductiveSteps = 0;

            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                totalWidth += problem.GetWidth();
                totalLength += problem.GetLength();
                totalDeductiveSteps += problem.GetNumDeductiveSteps();
            }

            figureStats.strictAverageProblemWidth = ((double)(totalWidth)) / problems.Count;
            figureStats.strictAverageProblemLength = ((double)(totalLength)) / problems.Count;
            figureStats.strictAverageProblemDeductiveSteps = ((double)(totalDeductiveSteps)) / problems.Count;
        }

        private void GenerateIsomorphicStatistics(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            //
            // Determine number of problems based on SOURCE isomorphism
            //
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector sourceQuery = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructSourceIsomorphismQueryVector();

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace sourceBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, sourceQuery);

            sourceBasedPartitions.ConstructPartitions(problems);

            figureStats.sourcePartitionSummary = sourceBasedPartitions.GetPartitionSummary();

            //
            // Determine number of problems based on DIFFICULTY of the problems (easy, medium difficult, extreme) based on the number of deductions
            //
            // Construct the partitions:
            // 25% Easy
            // 50% Medium
            // 75% Difficult
            // 100% Extreme
            //
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector difficultyQuery = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDeductiveBasedIsomorphismQueryVector(GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds());

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace difficultyBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, difficultyQuery);

            difficultyBasedPartitions.ConstructPartitions(problems);

            figureStats.difficultyPartitionSummary = difficultyBasedPartitions.GetDifficultyPartitionSummary();

            //
            // Determine number of interesting problems based percentage of givens covered.
            //
            // Construct the partitions:
            // 0-2 Easy
            // 3-5 Medium
            // 6-10 Difficult
            // 10+ Extreme
            //
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector interestingQuery = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingnessIsomorphismQueryVector(GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds());

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace interestingBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, interestingQuery);

            interestingBasedPartitions.ConstructPartitions(problems);

            figureStats.interestingPartitionSummary = interestingBasedPartitions.GetInterestingPartitionSummary();
        }

        private void GenerateStrictIsomorphicStatistics(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            //
            // Determine number of problems based on DIFFICULTY of the problems (easy, medium difficult, extreme) based on the number of deductions
            //
            // Construct the partitions:
            // 25% Easy
            // 50% Medium
            // 75% Difficult
            // 100% Extreme
            //
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector difficultyQuery = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDeductiveBasedIsomorphismQueryVector(GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructDifficultyPartitionBounds());

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace difficultyBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, difficultyQuery);

            difficultyBasedPartitions.ConstructPartitions(problems);

            figureStats.strictDifficultyPartitionSummary = difficultyBasedPartitions.GetDifficultyPartitionSummary();

            //
            // Determine number of interesting problems based percentage of givens covered.
            //
            // Construct the partitions:
            // 0-2 Easy
            // 3-5 Medium
            // 6-10 Difficult
            // 10+ Extreme
            //
            GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector interestingQuery = GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingnessIsomorphismQueryVector(GeometryTutorLib.ProblemAnalyzer.QueryFeatureVector.ConstructInterestingPartitionBounds());

            GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace interestingBasedPartitions = new GeometryTutorLib.ProblemAnalyzer.PartitionedProblemSpace(graph, interestingQuery);

            interestingBasedPartitions.ConstructPartitions(problems);

            figureStats.strictInterestingPartitionSummary = interestingBasedPartitions.GetInterestingPartitionSummary();
        }

        private void GeneratePaperQuery(List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> problems, StatisticsGenerator.ProofProblemFigureStatisticsAggregator figureStats)
        {
            List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>> sat = new List<GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation>>();
            int query = 0;
            int query2 = 0;
            int query3 = 0;
            int query4 = 0;

            foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems)
            {
                bool WRITE_PROBLEMS = true;
                int width = problem.GetWidth();
                int steps = problem.GetNumDeductiveSteps();
                int depth = problem.GetLength();

                if (6 <= steps && steps <= 10 && 4 <= width && width <= 8)
                {
                    query++;
                    if (WRITE_PROBLEMS)
                    {
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\singleProblemQuery.txt", true))
                        {
                            file.WriteLine(problem.ConstructProblemAndSolution(graph));
                        }
                    }
                }



                //if (3 <= steps && steps <= 7 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query2++;

                //    if (WRITE_PROBLEMS)
                //    {
                //        using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryProblems.txt", true))
                //        {
                //            file.WriteLine(problem.ConstructProblemAndSolution(graph));
                //        }
                //    }
                //}
                //if (steps >= 10 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query4++;
                //}

                //if (steps == 6 && depth == 4 && width == 5 && graph.vertices[problem.goal].data is ConcreteAST.CongruentTriangles)
                //{
                //    query3++;
                //}
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\teacherQuery.txt", true))
            {
                file.WriteLine(query);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent2.txt", true))
            {
                file.WriteLine(query2);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent3.txt", true))
            {
                file.WriteLine(query3);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"C:\Users\ctalvin\Desktop\output\queryStudent4.txt", true))
            {
                file.WriteLine(query4);
            }
        }
    }
}
