using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib;
using System.Diagnostics;
using System.Threading;

namespace GeometryTutorLib
{
    public class UIFigureAnalyzerMain
    {
        // The problem parameters to analyze
        private List<ConcreteAST.GroundedClause> figure;
        private List<ConcreteAST.GroundedClause> givens;

        private Precomputer.CoordinatePrecomputer precomputer;
        public Hypergraph.Hypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> graph { get; private set; }
        private GenericInstantiator.Instantiator instantiator;
        private Pebbler.PebblerHypergraph<ConcreteAST.GroundedClause, Hypergraph.EdgeAnnotation> pebblerGraph;
        private Pebbler.Pebbler pebbler;
        private ProblemAnalyzer.PathGenerator pathGenerator;
        private GeometryTutorLib.ProblemAnalyzer.TemplateProblemGenerator templateProblemGenerator = null;
        private ProblemAnalyzer.InterestingProblemCalculator interestingCalculator;
        private ProblemAnalyzer.QueryFeatureVector queryVector;
        private ProblemAnalyzer.PartitionedProblemSpace problemSpacePartitions;

        private EngineUIBridge.HypergraphWrapper hgWrapper;
        public EngineUIBridge.HypergraphWrapper GetHypergraphWrapper() { return hgWrapper; }

        public UIFigureAnalyzerMain(EngineUIBridge.ProblemDescription pdesc)
        {
            figure = pdesc.figure;            
            givens = pdesc.givens;

            // Create the precomputer object for coordinate-based pre-comutation analysis
            precomputer = new Precomputer.CoordinatePrecomputer(figure);
            instantiator = new GenericInstantiator.Instantiator();
            queryVector = new ProblemAnalyzer.QueryFeatureVector();

            EngineUIBridge.JustificationSwitch.SetAssumptions(EngineUIBridge.Assumption.GetAssumptions());
        }

        public List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> AnalyzeFigure()
        {
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
            KeyValuePair<List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>, List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>> problems = GenerateTemplateProblems();

            // Combine the problems together into one list
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> candidateProbs = new List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>();
            candidateProbs.AddRange(problems.Key);
            // candidateProbs.AddRange(problems.Value);

            // Determine which, if any, of the problems are interesting (using definition that 100% of the givens are used)
            interestingCalculator = new ProblemAnalyzer.InterestingProblemCalculator(graph, figure, givens);
            List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>> interestingProblems = interestingCalculator.DetermineInterestingProblems(candidateProbs);

            // Partition the problem-space based on the query vector defined (by us or the user)
            problemSpacePartitions = new ProblemAnalyzer.PartitionedProblemSpace(graph, queryVector);
            problemSpacePartitions.ConstructPartitions(interestingProblems);

            // Validate that we have generated all of the original problems from the text.     NO GOALS; no validation
            // List<ProblemAnalyzer.Problem> generatedBookProblems = problemSpacePartitions.ValidateOriginalProblems(givens, goals);

            if (Utilities.PROBLEM_GEN_DEBUG) problemSpacePartitions.DumpPartitions();

            //if (Utilities.DEBUG)
            //{
            //    Debug.WriteLine("\nAll " + generatedBookProblems.Count + " Book-specified problems: \n");
            //    foreach (ProblemAnalyzer.Problem bookProb in generatedBookProblems)
            //    {
            //        Debug.WriteLine(bookProb.ConstructProblemAndSolution(graph));
            //    }
            //}

            //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"generatedProblems.txt", true))
            //{
            //    //
            //    // Forward Problems
            //    //
            //    foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in interestingProblems)
            //    {
            //        file.WriteLine(problem.ConstructProblemAndSolution(graph).ToString());
            //    }

            //    //
            //    // Converse Problems
            //    //
            //    foreach (GeometryTutorLib.ProblemAnalyzer.Problem<GeometryTutorLib.Hypergraph.EdgeAnnotation> problem in problems.Value)
            //    {
            //        file.WriteLine(problem.ConstructProblemAndSolution(graph).ToString());
            //    }
            //}

            // Adding all the backward problems to the interesting forward problems.
            interestingProblems.AddRange(problems.Value);

            return interestingProblems;
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
        private List<ConcreteAST.GroundedClause> DoGivensStrengthenFigure()
        {
            List<ConcreteAST.GroundedClause> modifiedGivens = new List<ConcreteAST.GroundedClause>();
            ConcreteAST.GroundedClause currentGiven = null;

            foreach (ConcreteAST.GroundedClause given in givens)
            {
                currentGiven = given;
                foreach (ConcreteAST.GroundedClause component in figure)
                {
                    if (component.CanBeStrengthenedTo(given))
                    {
                        currentGiven = new ConcreteAST.Strengthened(component, given);
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
            GenericInstantiator.Instantiator.Clear();

            // Build the hypergraph through instantiation
            graph = instantiator.Instantiate(figure, givens);

            // Construct the hypergraph wrapper to give access to the front-end.
            hgWrapper = new EngineUIBridge.HypergraphWrapper(graph);

            if (Utilities.DEBUG)
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
            pebbler = new Pebbler.Pebbler(graph, pebblerGraph);

            pathGenerator = new ProblemAnalyzer.PathGenerator(graph);

            // Acquire the integer values of the intrinsic / figure nodes
            List<int> intrinsicSet = CollectGraphIndices(figure);

            // Acquire the integer values of the givens (from the original 
            List<int> givenSet = CollectGraphIndices(givens);

            // Perform pebbling based on the <figure, given> pair.
            pebbler.Pebble(intrinsicSet, givenSet);

            if (Utilities.PEBBLING_DEBUG)
            {
                Debug.WriteLine("Forward Vertices after pebbling:");
                for (int i = 0; i < pebblerGraph.vertices.Length; i++)
                {
                    StringBuilder strLocal = new StringBuilder();
                    strLocal.Append(pebblerGraph.vertices[i].id + ": pebbled(" + pebblerGraph.vertices[i].pebbled + ")");
                    Debug.WriteLine(strLocal.ToString());
                }

                pebbler.DebugDumpClauses();
            }
        }

        //
        // Generate all of the problems based on the precomputed values (these precomputations are the problem goals)
        //
        private KeyValuePair<List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>, List<ProblemAnalyzer.Problem<Hypergraph.EdgeAnnotation>>> GenerateTemplateProblems()
        {
            templateProblemGenerator = new ProblemAnalyzer.TemplateProblemGenerator(graph, pebbler, pathGenerator);

            // Generate the problem pairs
            return templateProblemGenerator.Generate(precomputer.GetPrecomputedRelations(), precomputer.GetStrengthenedClauses(), givens);
        }

        //
        // Acquires the hypergraph index value of the given nodes using structural equality
        //
        private List<int> CollectGraphIndices(List<ConcreteAST.GroundedClause> clauses)
        {
            List<int> indices = new List<int>();

            foreach (ConcreteAST.GroundedClause gc in clauses)
            {
                int index = Utilities.StructuralIndex(graph, gc);
                if (index != -1)
                {
                    indices.Add(index);
                }
                else
                {
                    Debug.WriteLine("We expect to find the given node (we did not): " + gc.ToString());
                }
            }

            return indices;
        }

        ////
        //// Given a series of points, generate all objects associated with segments and InMiddles
        ////
        //private List<ConcreteAST.GroundedClause> GenerateSegmentClauses(ConcreteAST.Collinear collinear)
        //{
        //    List<ConcreteAST.GroundedClause> newClauses = new List<ConcreteAST.GroundedClause>();

        //    //
        //    // Generate all ConcreteAST.Segment and ConcreteAST.InMiddle objects
        //    //
        //    for (int p1 = 0; p1 < collinear.points.Count - 1; p1++)
        //    {
        //        for (int p2 = p1 + 1; p2 < collinear.points.Count; p2++)
        //        {
        //            ConcreteAST.Segment newSegment = new ConcreteAST.Segment(collinear.points[p1], collinear.points[p2]);
        //            newClauses.Add(newSegment);
        //            for (int imIndex = p1 + 1; imIndex < p2; imIndex++)
        //            {
        //                newClauses.Add(new ConcreteAST.InMiddle(collinear.points[imIndex], newSegment));
        //            }
        //        }
        //    }

        //    return newClauses;
        //}

        ////
        //// Given a series of points, generate all objects associated with segments and InMiddles
        ////
        //private List<ConcreteAST.GroundedClause> GenerateAngleIntersectionPolygonClauses(List<ConcreteAST.GroundedClause> clauses)
        //{
        //    List<ConcreteAST.GroundedClause> newClauses = new List<ConcreteAST.GroundedClause>();

        //    // Find all the ConcreteAST.Segment and ConcreteAST.Point objects
        //    List<ConcreteAST.Segment> segments = new List<ConcreteAST.Segment>();
        //    List<ConcreteAST.Point> points = new List<ConcreteAST.Point>();
        //    foreach (ConcreteAST.GroundedClause clause in clauses)
        //    {
        //        if (clause is ConcreteAST.Segment) segments.Add(clause as ConcreteAST.Segment);
        //        if (clause is ConcreteAST.Point) points.Add(clause as ConcreteAST.Point);
        //    }

        //    List<ConcreteAST.Triangle> newTriangles = GenerateTriangleClauses(clauses, segments);
        //    List<ConcreteAST.Intersection> newIntersections = GenerateIntersectionClauses(newTriangles, segments, points);
        //    List<ConcreteAST.Angle> newAngles = GenerateAngleClauses(newIntersections);

        //    newAngles.ForEach(angle => newClauses.Add(angle));
        //    newIntersections.ForEach(intersection => newClauses.Add(intersection));
        //    newTriangles.ForEach(tri => newClauses.Add(tri));

        //    return newClauses;
        //}

        ////
        //// Generate all ConcreteAST.Triangle clauses based on segments
        ////
        //private List<ConcreteAST.Triangle> GenerateTriangleClauses(List<ConcreteAST.GroundedClause> clauses, List<ConcreteAST.Segment> segments)
        //{
        //    List<ConcreteAST.Triangle> newTriangles = new List<ConcreteAST.Triangle>();
        //    for (int s1 = 0; s1 < segments.Count - 2; s1++)
        //    {
        //        for (int s2 = s1 + 1; s2 < segments.Count - 1; s2++)
        //        {
        //            ConcreteAST.Point vertex1 = segments[s1].SharedVertex(segments[s2]);
        //            if (vertex1 != null)
        //            {
        //                for (int s3 = s2 + 1; s3 < segments.Count; s3++)
        //                {
        //                    ConcreteAST.Point vertex2 = segments[s3].SharedVertex(segments[s1]);
        //                    ConcreteAST.Point vertex3 = segments[s3].SharedVertex(segments[s2]);
        //                    if (vertex2 != null && vertex3 != null)
        //                    {
        //                        // Vertices must be distinct
        //                        if (!vertex1.Equals(vertex2) && !vertex1.Equals(vertex3) && !vertex2.Equals(vertex3))
        //                        {
        //                            // Vertices must be non-collinear
        //                            ConcreteAST.Segment side1 = new ConcreteAST.Segment(vertex1, vertex2);
        //                            ConcreteAST.Segment side2 = new ConcreteAST.Segment(vertex2, vertex3);
        //                            ConcreteAST.Segment side3 = new ConcreteAST.Segment(vertex1, vertex3);
        //                            if (!side1.IsCollinearWith(side2))
        //                            {
        //                                // Construct the triangle based on the sides to ensure reflexivity clauses are generated

        //                                newTriangles.Add(new ConcreteAST.Triangle(Precomputer.ClauseConstructor.GetProblemSegment(clauses, side1),
        //                                                                          Precomputer.ClauseConstructor.GetProblemSegment(clauses, side2), Precomputer.ClauseConstructor.GetProblemSegment(clauses, side3)));
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return newTriangles;
        //}

        ////
        //// Generate all covering intersection clauses; that is, generate maximal intersections (a subset of all intersections)
        ////
        //private List<ConcreteAST.Intersection> GenerateIntersectionClauses(List<ConcreteAST.Triangle> triangles, List<ConcreteAST.Segment> segments, List<ConcreteAST.Point> points)
        //{
        //    List<ConcreteAST.Intersection> newIntersections = new List<ConcreteAST.Intersection>();

        //    //
        //    // Each triangle has 3 valid intersections
        //    //
        //    foreach (ConcreteAST.Triangle triangle in triangles)
        //    {
        //        ConcreteAST.Point vertex = triangle.SegmentA.SharedVertex(triangle.SegmentB);
        //        AddIntersection(newIntersections, new ConcreteAST.Intersection(vertex, triangle.SegmentA, triangle.SegmentB));

        //        vertex = triangle.SegmentB.SharedVertex(triangle.SegmentC);
        //        AddIntersection(newIntersections, new ConcreteAST.Intersection(vertex, triangle.SegmentB, triangle.SegmentC));

        //        vertex = triangle.SegmentA.SharedVertex(triangle.SegmentC);
        //        AddIntersection(newIntersections, new ConcreteAST.Intersection(vertex, triangle.SegmentA, triangle.SegmentC));
        //    }

        //    //
        //    // Find the maximal segments (remove all sub-segments from the list)
        //    //
        //    List<ConcreteAST.Segment> maximalSegments = new List<ConcreteAST.Segment>();
        //    for (int s1 = 0; s1 < segments.Count; s1++)
        //    {
        //        bool isSubsegment = false;
        //        for (int s2 = 0; s2 < segments.Count; s2++)
        //        {
        //            if (s1 != s2)
        //            {
        //                if (segments[s2].HasSubSegment(segments[s1]))
        //                {
        //                    isSubsegment = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (!isSubsegment) maximalSegments.Add(segments[s1]);
        //    }

        //    //
        //    // Acquire all intersections from the maximal segment list
        //    //
        //    for (int s1 = 0; s1 < maximalSegments.Count - 1; s1++)
        //    {
        //        for (int s2 = s1 + 1; s2 < maximalSegments.Count; s2++)
        //        {
        //            // An intersection should not be between collinear segments
        //            if (!maximalSegments[s1].IsCollinearWith(maximalSegments[s2]))
        //            {
        //                // The point must be 'between' both segment endpoints
        //                ConcreteAST.Point numericInter = maximalSegments[s1].FindIntersection(maximalSegments[s2]);
        //                if (maximalSegments[s1].PointLiesOnAndBetweenEndpoints(numericInter) &&
        //                    maximalSegments[s2].PointLiesOnAndBetweenEndpoints(numericInter))
        //                {
        //                    // Find the actual point for which there is an intersection between the segments
        //                    ConcreteAST.Point actualInter = null;
        //                    foreach (ConcreteAST.Point pt in points)
        //                    {
        //                        if (numericInter.StructurallyEquals(pt))
        //                        {
        //                            actualInter = pt;
        //                            break;
        //                        }
        //                    }

        //                    // Create the intersection
        //                    if (actualInter != null)
        //                    {
        //                        AddIntersection(newIntersections, new ConcreteAST.Intersection(actualInter, maximalSegments[s1], maximalSegments[s2]));
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return newIntersections;
        //}

        ////
        //// Generate all angles based on the intersections
        ////
        //private List<ConcreteAST.Angle> GenerateAngleClauses(List<ConcreteAST.Intersection> intersections)
        //{
        //    List<ConcreteAST.Angle> newAngles = new List<ConcreteAST.Angle>();

        //    foreach (ConcreteAST.Intersection inter in intersections)
        //    {
        //        // 1 angle
        //        if (inter.StandsOnEndpoint())
        //        {
        //            AddAngle(newAngles, (new ConcreteAST.Angle(inter.lhs.OtherPoint(inter.intersect), inter.intersect, inter.rhs.OtherPoint(inter.intersect))));
        //        }
        //        // 2 angles
        //        else if (inter.StandsOn())
        //        {
        //            ConcreteAST.Point up = null;
        //            ConcreteAST.Point left = null;
        //            ConcreteAST.Point right = null;
        //            if (inter.lhs.HasPoint(inter.intersect))
        //            {
        //                up = inter.lhs.OtherPoint(inter.intersect);
        //                left = inter.rhs.Point1;
        //                right = inter.rhs.Point2;
        //            }
        //            else
        //            {
        //                up = inter.rhs.OtherPoint(inter.intersect);
        //                left = inter.lhs.Point1;
        //                right = inter.lhs.Point2;
        //            }

        //            AddAngle(newAngles, new ConcreteAST.Angle(left, inter.intersect, up));
        //            AddAngle(newAngles, new ConcreteAST.Angle(right, inter.intersect, up));
        //        }
        //        // 4 angles
        //        else
        //        {
        //            AddAngle(newAngles, new ConcreteAST.Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point1));
        //            AddAngle(newAngles, new ConcreteAST.Angle(inter.lhs.Point1, inter.intersect, inter.rhs.Point2));
        //            AddAngle(newAngles, new ConcreteAST.Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point1));
        //            AddAngle(newAngles, new ConcreteAST.Angle(inter.lhs.Point2, inter.intersect, inter.rhs.Point2));
        //        }
        //    }

        //    return newAngles;
        //}

        //// Add an angle to the list uniquely
        //private void AddAngle(List<ConcreteAST.Angle> angles, ConcreteAST.Angle thatAngle)
        //{
        //    if (thatAngle.measure == 0 || thatAngle.measure == 180)
        //    {
        //        System.Diagnostics.Debug.WriteLine("");
        //    }

        //    foreach (ConcreteAST.Angle thisAngle in angles)
        //    {
        //        if (thisAngle.Equates(thatAngle)) return;
        //    }

        //    angles.Add(thatAngle);
        //}

        //// Add an intersection to the list uniquely
        //private void AddIntersection(List<ConcreteAST.Intersection> intersections, ConcreteAST.Intersection thatInter)
        //{
        //    foreach (ConcreteAST.Intersection inter in intersections)
        //    {
        //        if (inter.StructurallyEquals(thatInter)) return;
        //    }

        //    intersections.Add(thatInter);
        //}


        //// Add an angle to the list uniquely
        //private ConcreteAST.Segment GetProblemSegment(List<ConcreteAST.GroundedClause> clauses, ConcreteAST.Segment thatSegment)
        //{
        //    foreach (ConcreteAST.GroundedClause clause in clauses)
        //    {
        //        if (clause.StructurallyEquals(thatSegment)) return clause as ConcreteAST.Segment;
        //    }

        //    return null;
        //}

        //// Acquire an established angle
        //private ConcreteAST.Angle GetProblemAngle(List<ConcreteAST.GroundedClause> clauses, ConcreteAST.Angle thatAngle)
        //{
        //    foreach (ConcreteAST.GroundedClause clause in clauses)
        //    {
        //        if (clause is ConcreteAST.Angle)
        //        {
        //            if (clause.StructurallyEquals(thatAngle)) return clause as ConcreteAST.Angle;
        //        }
        //    }

        //    return null;
        //}

        //// Acquire an established triangle
        //private ConcreteAST.Triangle GetProblemTriangle(List<ConcreteAST.GroundedClause> clauses, ConcreteAST.Triangle thatTriangle)
        //{
        //    foreach (ConcreteAST.GroundedClause clause in clauses)
        //    {
        //        if (clause.StructurallyEquals(thatTriangle)) return clause as ConcreteAST.Triangle;
        //    }

        //    return null;
        //}

        //// Acquire an established intersection
        //private ConcreteAST.Intersection GetProblemIntersection(List<ConcreteAST.GroundedClause> clauses, ConcreteAST.Segment segment1, ConcreteAST.Segment segment2)
        //{
        //    foreach (ConcreteAST.GroundedClause clause in clauses)
        //    {
        //        ConcreteAST.Intersection inter = clause as ConcreteAST.Intersection;
        //        if (inter != null)
        //        {
        //            if (inter.HasSegment(segment1) && inter.HasSegment(segment2)) return inter;
        //        }
        //    }

        //    return null;
        //}
    }
}
