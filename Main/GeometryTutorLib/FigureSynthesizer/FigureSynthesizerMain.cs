using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.GeometryTestbed;

namespace GeometryTutorLib
{
    /// <summary>
    /// Main routine for the search technique to synthesize figures.
    /// </summary>
    public static partial class FigureSynthesizerMain
    {
        public static bool SynthesizerMain(Dictionary<ShapeType, int> figureCountMap, TemplateType type)
        {
            //
            // Convert the incoming dictionary to a simple list of shapes to process in order.
            //
            List<List<ShapeType>> shapesSets = ConvertShapeMapToList(figureCountMap);

            bool success = true;
            foreach (List<ShapeType> shapes in shapesSets)
            {
                //
                // Construct the figure recursively.
                //
                List<FigSynthProblem> problems = SynthesizeFromTemplateAndFigures(shapes, type);

                //
                // Construct the problem so that it can be passed to the Solver.
                //
                success = ConstructProblemsToSolve(problems) || success;
            }

            return success;
        }


        //
        // Abusive, comprehensive test of all combinations of figures with the basic templates.
        //
        public static void SynthesizerMainDataGeneration()
        {
            List<ShapeType> shapes = new List<ShapeType>();

            shapes.Add(ShapeType.RIGHT_TRIANGLE);
            shapes.Add(ShapeType.RECTANGLE);
            shapes.Add(ShapeType.SQUARE);

            for (int s1 = 0; s1 < shapes.Count; s1++)
            {
                bool worked = true;
                for (int s2 = 0; s2 < shapes.Count; s2++)
                {
                    List<ShapeType> sTypes = new List<ShapeType>();
                    sTypes.Add(shapes[s1]);
                    sTypes.Add(shapes[s2]);

                    Dictionary<ShapeType, int> figureCountMap = ConvertListToShapeMap(sTypes);

                    System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------- " + shapes[s1] + " - " + shapes[s2] + " --------------------------------------------------------");
                    worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_MINUS_BETA) || worked;

                    System.Diagnostics.Debug.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++ " + shapes[s1] + " + " + shapes[s2] + " +++++++++++++++++++++++++++++++++++++++++++++++++++++++");
                    worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_PLUS_BETA) || worked;

                    // Add a value at the end so it can be deleted.
                    sTypes.Add(shapes[s2]);
                    for (int s3 = 0; s3 < shapes.Count; s3++)
                    {
                        sTypes.RemoveAt(sTypes.Count - 1);
                        sTypes.Add(shapes[s3]);

                        figureCountMap = ConvertListToShapeMap(sTypes);

                        System.Diagnostics.Debug.WriteLine("-----------------------------------------------------------------" + shapes[s1] + " + " + shapes[s2] + " + " + shapes[s3] + "-----------------------------------------------------------------");
                        worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA) || worked;

                        System.Diagnostics.Debug.WriteLine("-----------------------------------------------------------------" + shapes[s1] + " + (" + shapes[s2] + " - " + shapes[s3] + ")" + "-----------------------------------------------------------------");
                        worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN) || worked;

                        System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------- (" + shapes[s1] + " + " + shapes[s2] + ") - " + shapes[s3] + " -----------------------------------------------------------------");
                        worked = SynthesizerMain(figureCountMap, TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA) || worked;

                        System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------- (" + shapes[s1] + " - " + shapes[s2] + ") - " + shapes[s3] + " -----------------------------------------------------------------");
                        worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA) || worked;

                        System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------- (" + shapes[s1] + " - (" + shapes[s2] + " - " + shapes[s3] + ") -----------------------------------------------------------------");
                        worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN) || worked;
                    }
                }
            }
        }

        //
        // Abusive, comprehensive test of all combinations of figures with the basic templates.
        //
        public static void SynthesizerMainTester()
        {
            List<ShapeType> shapes = new List<ShapeType>();

            shapes.Add(ShapeType.CIRCLE);
            shapes.Add(ShapeType.SQUARE);
            shapes.Add(ShapeType.RIGHT_TRIANGLE);

            //shapes.Add(ShapeType.TRIANGLE);
            // DONE shapes.Add(ShapeType.EQUILATERAL_TRIANGLE);
            //shapes.Add(ShapeType.ISOSCELES_TRIANGLE);
            // DONEshapes.Add(ShapeType.RECTANGLE);
            // DONE shapes.Add(ShapeType.RIGHT_TRIANGLE);
            // DONE shapes.Add(ShapeType.SQUARE);
            // NO shapes.Add(ShapeType.KITE);
            // DONE shapes.Add(ShapeType.ISO_TRAPEZOID);
            //shapes.Add(ShapeType.TRAPEZOID);



            //shapes.Add(ShapeType.QUADRILATERAL);

            //shapes.Add(ShapeType.RHOMBUS);

            // No: shapes.Add(ShapeType.ISO_RIGHT_TRIANGLE);

            for (int s1 = 0; s1 < shapes.Count; s1++)
            {
                bool worked = true;
                for (int s2 = s1 + 1; s2 < shapes.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        List<ShapeType> sTypes = new List<ShapeType>();
                        sTypes.Add(shapes[s1]);
                        sTypes.Add(shapes[s2]);

                        Dictionary<ShapeType, int> figureCountMap = ConvertListToShapeMap(sTypes);

                        //System.Diagnostics.Debug.WriteLine("----------------------------------------------------------------- " + shapes[s1] + " - " + shapes[s2] + " --------------------------------------------------------");
                        //worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_MINUS_BETA) || worked;

                        // Add a value at the end so it can be deleted.
                        sTypes.Add(shapes[s2]);
                        for (int s3 = s2 + 1; s3 < shapes.Count; s3++)
                        {
                            sTypes.RemoveAt(sTypes.Count - 1);
                            sTypes.Add(shapes[s3]);

                            figureCountMap = ConvertListToShapeMap(sTypes);

                            System.Diagnostics.Debug.WriteLine(shapes[s1] + " - " + shapes[s2] + " - " + shapes[s3]);
                            worked = SynthesizerMain(figureCountMap, TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA) || worked;
                        }
                    }
                }

                if (worked) System.Diagnostics.Debug.WriteLine(shapes[s1] + " is golden.");
            }
        }

        public static void SynthesizerMain()
        {
            //
            // Make up a list of shapes to compose.
            //
            Dictionary<ShapeType, int> figureCountMap = new Dictionary<ShapeType, int>();

            figureCountMap[ShapeType.RIGHT_TRIANGLE] = 2;
            figureCountMap[ShapeType.SQUARE] = 1;

            //
            // Convert the incoming dictionary to a simple list of shapes to process in order.
            //
            List<ShapeType> shapes = ConvertShapeMapToList(figureCountMap)[0];

            //
            // Construct the figure recursively.
            //
            List<FigSynthProblem> problems = new List<FigSynthProblem>();
            try
            {
                problems = SynthesizeFromTemplateAndFigures(shapes, TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            //
            // Debug output of the problems.
            //
            foreach (FigSynthProblem problem in problems)
            {
                System.Diagnostics.Debug.WriteLine(problem.ToString());
            }

            //
            // Construct the problem so that it can be passed to the Solver.
            //
            ConstructProblemsToSolve(problems);
        }

#if HARD_CODED_UI
        private static bool drawnAProblem = false;
#endif
        private static bool ConstructProblemsToSolve(List<FigSynthProblem> problems)
        {
            List<FigSynthShadedAreaProblem> shadedAreaProblems = new List<FigSynthShadedAreaProblem>();

            int problemCount = 0;
            bool worked = true;
            foreach (FigSynthProblem problem in problems)
            {
                if (Utilities.FIGURE_SYNTHESIZER_DEBUG)
                {
                    string label = "";
                    for (int i = 0; i < 80 / ((problemCount / 10) + 1); i++) label += problemCount + " ";
                    System.Diagnostics.Debug.WriteLine(label);

                    System.Diagnostics.Debug.WriteLine(problem.ToString());
                    problemCount++;
                }

                //try
                //{
                //    // Create the problem
                //    GeometryTestbed.FigSynthShadedAreaProblem shadedProb = ConstructProblem(problem);

                //    // Add the problem to a running list (nothing done with the list yet).
                //    shadedAreaProblems.Add(shadedProb);

                //    // Actually run this problem (and solve).
                //    shadedProb.Run();

                //    // Data dump for statistics gathering.
                //    System.Diagnostics.Debug.WriteLine(shadedProb.ToString());
                //}
                //catch (ArgumentException e)
                //{
                //    System.Diagnostics.Debug.WriteLine("Argument: " + e.ToString());
                //    worked = false;
                //}
                //catch (NotImplementedException e)
                //{
                //    System.Diagnostics.Debug.WriteLine("\t\t Unimplmented");
                //    worked = false;
                //}
                //catch (Exception e)
                //{
                //    System.Diagnostics.Debug.WriteLine("Failed: " + e.ToString());
                //    worked = false;
                //}

#if HARD_CODED_UI
                //if (!drawnAProblem)
                //{
                //    UIProblemDrawer.getInstance().draw(shadedProb.MakeUIProblemDescription());
                //    shadedProb.Run();
                //    return shadedAreaProblems;
                //}
                //drawnAProblem = true;
#endif
            }

            return worked;
        }

        private static int figCounter = 1;
        private static GeometryTestbed.FigSynthShadedAreaProblem ConstructProblem(FigSynthProblem problem)
        {
            GeometryTestbed.FigSynthShadedAreaProblem shadedArea = new GeometryTestbed.FigSynthShadedAreaProblem(true, true);

            //
            // Name the problem (uniquely).
            //
            shadedArea.SetName("Fig-Synthesized " + (figCounter++));

            //
            // Construct the points.
            //
            List<Point> points = problem.CollectPoints();
            shadedArea.SetPoints(points);

            //
            // Construct the collinear relationships.
            //
            List<Segment> segments;
            List<Collinear> collinear;

            AcquireCollinearAndSegments(problem.CollectSegments(), points, out segments, out collinear);

            shadedArea.SetSegments(segments);
            shadedArea.SetCollinear(collinear);

            //
            // Construct circles.
            //
            shadedArea.SetCircles(problem.CollectCircles());

            //
            // Invoke the parser.
            //
            shadedArea.InvokeParser();

            //
            // Set the wanted atomic regions.
            //
            shadedArea.SetWantedRegions(shadedArea.GetRemainingRegionsFromParser(problem));

            //
            // Set the known values.
            // Acquire all of the givens using constant propagation for each figure construction.
            //
            shadedArea.SetKnowns(problem.AcquireKnowns());

            //
            // Set the problem given clauses.
            //
            List<GroundedClause> givens = problem.GetGivens();
            problem.GetMidpoints().ForEach(m => givens.Add(m));
            shadedArea.SetGivens(givens);

            //
            // Set the actual area of the solution (area of wanted regions).
            //
            shadedArea.SetSolutionArea(problem.GetCoordinateArea());

            return shadedArea;
        }

        //
        // Given the list of shapes AND a desired template, synthesize.
        //
        public static List<FigSynthProblem> SynthesizeFromTemplateAndFigures(List<ShapeType> shapeList, TemplateType type)
        {
            if (!VerifyInputParameters(shapeList, type)) return new List<FigSynthProblem>();

            return SynthesizeFromTemplate(shapeList, type);
        }

        //
        // Given a template: \alpha - \beta , etc.
        //
        public static List<FigSynthProblem> SynthesizeFromTemplate(List<ShapeType> shapeList, TemplateType type)
        {
            //
            // Construct the default shape for the bigger shape-type specified.
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapeList[0]);

            switch (type)
            {
                //
                // Two Shapes
                //
                case TemplateType.ALPHA_MINUS_BETA:                           // a - b
                    return ConstructSequentialSubtraction(shapeList);

                case TemplateType.ALPHA_PLUS_BETA:                            // a + b
                    return ConstructSequentialAddition(shapeList);

                //
                // Three Shapes
                //
                case TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA:                 // a + b + c
                    return ConstructSequentialAddition(shapeList);

                case TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN:  // a + (b - c) = (b - c) + a
                case TemplateType.ALPHA_MINUS_BETA_PLUS_GAMMA:                // (a - b) + c
                    return ConstructParenSubtractionThenAppend(shapeList);

                case TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA:  // (a + b) - c
                    return ConstructAppendThenSubtract(shapeList);

                case TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA:               // a - b - c
                    return ConstructSequentialSubtraction(shapeList);

                case TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN: // a - (b - c)
                    return ConstructGroupedSubtraction(shapeList);
            }

            return new List<FigSynthProblem>();
        }

        //
        // (a - b) + c
        // a + (b - c) = (b - c) + a
        //
        //  First subtraction x = (a - b)...then append (x + c).
        //
        private static List<FigSynthProblem> ConstructParenSubtractionThenAppend(List<ShapeType> shapes)
        {
            //
            // Construct  a - b
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);
            List<FigSynthProblem> aMinusB = SubtractShape(defaultLargeFigure, shapes[1]);

            //
            // For each of the aMinusB problems, the outer bounds is defined by a shape; append the new shape.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem top in aMinusB)
            {
                List<FigSynthProblem> bMinusC = AddShape(((top as BinarySynthOperation).leftProblem as UnarySynth).figure, shapes[2]);

                foreach (FigSynthProblem bc in bMinusC)
                {
                    newSynths.Add(FigSynthProblem.AppendAddition(top, bc));
                }
            }

            return newSynths;
        }

        //
        // (a + b) - c
        //
        //  First append, then subtract from the entire shape.
        //
        private static List<FigSynthProblem> ConstructAppendThenSubtract(List<ShapeType> shapes)
        {
            //
            // Construct  a + b
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);
            List<FigSynthProblem> aPlusB = AddShape(defaultLargeFigure, shapes[1]);

            //
            // For each of the aPlusB problems, the outer bounds is defined by the exterior segments / set of points.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem top in aPlusB)
            {
                Polygon outerPoly = Polygon.MakePolygon(top.GetExteriorSegments());
                List<FigSynthProblem> xMinusC = SubtractShape(outerPoly, shapes[2]);

                foreach (FigSynthProblem x in xMinusC)
                {
                    newSynths.Add(FigSynthProblem.AppendFigureSubtraction(top, x));
                }
            }

            return newSynths;
        }

        //
        // Perform a - (b - c) =
        //                         1. a - b 
        //                         2. b - c
        //
        private static List<FigSynthProblem> ConstructGroupedSubtraction(List<ShapeType> shapes)
        {
            //
            // Construct  a - b
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);
            List<FigSynthProblem> aMinusB = SubtractShape(defaultLargeFigure, shapes[1]);

            //
            // For each of the aMinusB problems, the outer bounds is defined by a shape; subtract the new shape.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem top in aMinusB)
            {
                List<FigSynthProblem> bMinusC = SubtractShape(((top as BinarySynthOperation).rightProblem as UnarySynth).figure, shapes[2]);

                foreach (FigSynthProblem bc in bMinusC)
                {
                    newSynths.Add(FigSynthProblem.AppendFigureSubtraction(top, bc));
                }
            }

            return newSynths;
        }

        //
        // Perform a - b - ... - c
        //
        private static List<FigSynthProblem> ConstructSequentialSubtraction(List<ShapeType> shapes)
        {
            //
            // Construct base case
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);

            List<FigSynthProblem> startSynths = SubtractShape(defaultLargeFigure, shapes[1]);

            //
            // Recursive construction of each shape (level in the recursion).
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>(startSynths);
            for (int s = 2; s < shapes.Count; s++)
            {
                newSynths = ConstructSubtraction(newSynths, shapes[s]);
            }

            return newSynths;
        }

        //
        // Recursive case
        //
        private static List<FigSynthProblem> ConstructSubtraction(List<FigSynthProblem> synths, ShapeType shapeType)
        {
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();

            foreach (FigSynthProblem synth in synths)
            {
                newSynths.AddRange(Subtract(synth, shapeType));
            }

            return newSynths;
        }

        //
        // Perform subtraction with a synthesized problem.
        //
        private static List<FigSynthProblem> Subtract(FigSynthProblem synth, ShapeType shapeType)
        {
            List<AtomicRegion> openAtoms = synth.GetOpenRegions();

            //
            // Perform subtraction with all open regions with the new shape.
            //
            List<FigSynthProblem> newSubs = new List<FigSynthProblem>();
            foreach (AtomicRegion atom in openAtoms)
            {
                ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;

                if (shapeAtom != null)
                {
                    newSubs.AddRange(SubtractShape(shapeAtom.shape, shapeType));
                }
            }

            //
            // Combine the existent problem with the newly subtracted region.
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
            foreach (FigSynthProblem newSub in newSubs)
            {
                // Makes a copy out of the outer problem and appends the subtraction operation.
                newSynths.Add(FigSynthProblem.AppendAtomicSubtraction(synth, newSub));
            }

            //
            // Eliminate symmetric problems.
            //
            return FigSynthProblem.RemoveSymmetric(newSynths);
        }



        //
        // Perform a - (b - c) =
        //                         1. a - b 
        //                         2. b - c
        //
        //private static List<FigSynthProblem> ConstructGroupedSubtraction(List<ShapeType> shapes)
        //{
        //    //
        //    // Construct  a - b
        //    //
        //    Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);
        //    List<FigSynthProblem> aMinusB = SubtractShape(defaultLargeFigure, shapes[1]);

        //    //
        //    // For each of the aMinusB problems, the outer bounds is defined by a shape; subtract the new shape.
        //    //
        //    List<FigSynthProblem> newSynths = new List<FigSynthProblem>();
        //    foreach (FigSynthProblem top in aMinusB)
        //    {
        //        List<FigSynthProblem> bMinusC = SubtractShape(((top as BinarySynthOperation).rightProblem as UnarySynth).figure, shapes[2]);

        //        foreach (FigSynthProblem bc in bMinusC)
        //        {
        //            newSynths.Add(FigSynthProblem.AppendFigureSubtraction(top, bc));
        //        }
        //    }

        //    return newSynths;
        //}

        //
        // Given a template: \alpha - \beta , etc.
        //
        public static void SynthesizeFromTemplate(TemplateType type)
        {
            switch (type)
            {
                // Two Shapes
                case TemplateType.ALPHA_MINUS_BETA:                           // a - b
                    break;
                case TemplateType.ALPHA_PLUS_BETA:                            // a + b
                    break;
                // Three Shapes
                case TemplateType.ALPHA_PLUS_BETA_PLUS_GAMMA:                 // a + b + c
                    break;
                case TemplateType.ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN:  // a + (b - c)
                    break;
                case TemplateType.LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA:  // (a + b) - c
                    break;
                case TemplateType.ALPHA_MINUS_BETA_MINUS_GAMMA:               // a - b - c
                    break;
                case TemplateType.ALPHA_MINUS_BETA_PLUS_GAMMA:                // a - b + c
                    break;
                case TemplateType.ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN: // a - (b - c)
                    break;
            }
        }

        //
        // Given a set of Figures, synthesize a new problem
        //
        public static void SynthesizeFromFigures(Dictionary<Figure, int> FigureMap)
        {
            if (FigureMap.Count > 3)
            {
                throw new ArgumentException("Cannot synthesize a figure with more than 3 Figures.");
            }

        }

        //
        // Given a set of points, can we construct any shape?
        // If we can construct a shape, we then have a resultant mess...
        //
        public static List<FigSynthProblem> SubtractShape(Figure outerShape, ShapeType type)
        {
            List<FigSynthProblem> problems = new List<FigSynthProblem>();
            List<Connection> conns = new List<Connection>();
            List<Point> points = new List<Point>();

            try
            {
                conns = outerShape.MakeAtomicConnections();
                points = outerShape.allComposingPoints;
            } catch (Exception) { return problems; }

            switch (type)
            {
                case ShapeType.TRIANGLE:
                    problems = Triangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.ISOSCELES_TRIANGLE:
                    problems = IsoscelesTriangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RIGHT_TRIANGLE:
                    problems = RightTriangle.SubtractShape(outerShape, conns, points);
                    break;

                //case ShapeType.ISO_RIGHT_TRIANGLE:
                //    problems =  Triangle.SubtractShape(outerShape, conns, points);

                case ShapeType.EQUILATERAL_TRIANGLE:
                    problems = EquilateralTriangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.KITE:
                    problems = Kite.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.QUADRILATERAL:
                    problems = Quadrilateral.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.TRAPEZOID:
                    problems = Trapezoid.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.ISO_TRAPEZOID:
                    problems = IsoscelesTrapezoid.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.PARALLELOGRAM:
                    problems = Parallelogram.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RECTANGLE:
                    problems = Rectangle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.RHOMBUS:
                    problems = Rhombus.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.SQUARE:
                    problems = Square.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.CIRCLE:
                    problems = Circle.SubtractShape(outerShape, conns, points);
                    break;

                case ShapeType.SECTOR:
                    problems = Sector.SubtractShape(outerShape, conns, points);
                    break;
            }

            return problems;
        }

        //
        // Perform a - b - ... - c
        //
        private static List<FigSynthProblem> ConstructSequentialAddition(List<ShapeType> shapes)
        {
            //
            // Construct base case
            //
            Figure defaultLargeFigure = Figure.ConstructDefaultShape(shapes[0]);

            List<FigSynthProblem> startSynths = AddShape(defaultLargeFigure, shapes[1]);

            //
            // Recursive construction of each shape (level in the recursion).
            //
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>(startSynths);
            for (int s = 2; s < shapes.Count; s++)
            {
                newSynths = ConstructAddition(newSynths, shapes[s]);
            }

            return newSynths;
        }

        //
        // Recursive case
        //
        private static List<FigSynthProblem> ConstructAddition(List<FigSynthProblem> synths, ShapeType shapeType)
        {
            List<FigSynthProblem> newSynths = new List<FigSynthProblem>();

            foreach (FigSynthProblem synth in synths)
            {
                newSynths.AddRange(AddShape(synth, shapeType));
            }

            return newSynths;
        }

        public static List<FigSynthProblem> AddShape(Figure outerShape, ShapeType type)
        {
            List<FigSynthProblem> problems = new List<FigSynthProblem>();

            switch (type)
            {
                case ShapeType.TRIANGLE:
                    problems.AddRange(Triangle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.ISOSCELES_TRIANGLE:
                    problems.AddRange(IsoscelesTriangle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.RIGHT_TRIANGLE:
                    problems.AddRange(RightTriangle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
                //case ShapeType.ISO_RIGHT_TRIANGLE:
                //    problems.AddRange(Triangle.AppendShape(points);

                case ShapeType.EQUILATERAL_TRIANGLE:
                    problems.AddRange(EquilateralTriangle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
                case ShapeType.KITE:
                    problems.AddRange(Kite.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
                case ShapeType.QUADRILATERAL:
                    problems.AddRange(Quadrilateral.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
                case ShapeType.TRAPEZOID:
                    problems.AddRange(Trapezoid.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
                case ShapeType.ISO_TRAPEZOID:
                    problems.AddRange(IsoscelesTrapezoid.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.PARALLELOGRAM:
                    problems.AddRange(Parallelogram.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.RECTANGLE:
                    problems.AddRange(Rectangle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.RHOMBUS:
                    problems.AddRange(Rhombus.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.SQUARE:
                    problems.AddRange(Square.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.CIRCLE:
                    problems.AddRange(Circle.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;

                case ShapeType.SECTOR:
                    problems.AddRange(Sector.AppendShape(outerShape, outerShape.GetCompleteSideSegments()));
                    break;
            }

            return problems;
        }

        //
        // Append to an existent problem.
        //
        public static List<FigSynthProblem> AddShape(FigSynthProblem problem, ShapeType type)
        {
            // Create a polygon based only on exterior segments.
            Polygon outer = Polygon.MakePolygon(problem.GetExteriorSegments());

            // Append the shapes
            List<FigSynthProblem> newSynths = AddShape(outer, type);

            // Create the new problems by appending.
            List<FigSynthProblem> appended = new List<FigSynthProblem>();
            foreach (FigSynthProblem synth in newSynths)
            {
                appended.Add(FigSynthProblem.AppendAddition(problem, synth));
            }

            return appended;
        }

        //
        // Determine which of the inner Shape points apply to the given connection; add them as collinear points.
        //
        private static void AcquireCollinearAndSegments(List<Segment> segments, List<Point> points,
                                                        out List<Segment> trueSegments, out List<Collinear> collinear)
        {
            trueSegments = new List<Segment>();
            collinear = new List<Collinear>();

            // Maximal segments (containing no subsegments).
            List<Segment> maximal = new List<Segment>();

            //
            // Prune subsegments away
            //
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                bool max = true;
                for (int s2 = 0; s2 < segments.Count; s2++)
                {
                    if (s1 != s2)
                    {
                        if (segments[s1].HasSubSegment(segments[s2]))
                        {
                            max = false;
                            break;
                        }
                    }
                }
                if (max) maximal.Add(segments[s1]);
            }

            //
            // Place all points on appropriate segments for collinearity.
            //
            foreach (Segment max in maximal)
            {
                max.ClearCollinear();
                foreach (Point pt in points)
                {
                    if (max.PointLiesOn(pt))
                    {
                        max.AddCollinearPoint(pt);
                    }
                }

                //
                // Convert to collinear or straight-up segments.
                //
                if (max.collinear.Count > 2)
                {
                    collinear.Add(new Collinear(max.collinear));
                }
                else trueSegments.Add(max);
            }
        }
    }
}