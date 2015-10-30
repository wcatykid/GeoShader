using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class FigSynthProblem
    {
        public abstract List<Constraint> GetConstraints();
        public abstract List<Segment> GetAreaVariables();
        public abstract double EvaluateArea(KnownMeasurementsAggregator known);

        public abstract FigSynthProblem Copy();
        public abstract FigSynthProblem GetSynthByShape(Figure that);
        public abstract FigSynthProblem ReplaceUnary(Figure that, FigSynthProblem toSub);
        public abstract List<Point> CollectPoints();
        public abstract List<Segment> CollectSegments();
        public abstract List<Segment> CollectCompleteSegments();
        public abstract List<Point> CollectCompletePoints();
        public abstract List<Figure> CollectSubtractiveFigures(bool subtractFlag);
        public abstract List<Arc> CollectArcs();
        public abstract List<Circle> CollectCircles();
        public abstract double GetCoordinateArea();
        public abstract List<GroundedClause> GetGivens();
        public abstract List<Midpoint> GetMidpoints();

        // The allowable regions that we might insert a figure.
        // This is only meaningful at the top level of the problem.
        protected List<AtomicRegion> openRegions;
        public List<AtomicRegion> GetOpenRegions() { return openRegions; }
        public void SetOpenRegions(List<AtomicRegion> rs) { openRegions = rs; }

        public abstract List<Segment> GetExteriorSegments();

        //
        // Acquire the actual external points.
        //
        public List<Point> GetExteriorPoints()
        {
            List<Segment> exterior = GetExteriorSegments();
            List<Point> extPoints = new List<Point>();

            foreach (Segment seg in exterior)
            {
                Utilities.AddUnique<Point>(extPoints, seg.Point1);
                Utilities.AddUnique<Point>(extPoints, seg.Point2);
            }

            return extPoints;
        }

        //
        // Prune the list of synth objects to only asymmetric scenarios.
        //
        public static List<FigSynthProblem> RemoveSymmetric(List<FigSynthProblem> composed)
        {
            // Base case.
            if (!composed.Any()) return composed;

            // The eventual list of asymmetric scenarios.
            List<FigSynthProblem> unique = new List<FigSynthProblem>();

            // Prime the list of unique synths with the first one.
            unique.Add(composed[0]);

            for (int s = 1; s < composed.Count; s++)
            {
                bool asymmetric = true;
                foreach (FigSynthProblem uniqueAgg in unique)
                {
                    if (uniqueAgg.IsSymmetricTo(composed[s]))
                    {
                        asymmetric = false;
                        break;
                    }
                }

                if (asymmetric) unique.Add(composed[s]);
            }

            return unique;
        }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public virtual bool IsSymmetricTo(FigSynthProblem that)
        {
            // The number of atomic shapes is consistent
            if (this.openRegions.Count != that.openRegions.Count) return false;

            // We must have a 1-1 mapping of the remaining atomic shapes. This is in terms of congruence.
            int numRegions = openRegions.Count;
            bool[] marked = new bool[numRegions];
            foreach (AtomicRegion thisAtom in openRegions)
            {
                bool foundAtom = false;
                for (int a = 0; a < numRegions; a++)
                {
                    if (!marked[a])
                    {
                        if (thisAtom.CoordinateCongruent(that.openRegions[a]))
                        {
                            marked[a] = true;
                            foundAtom = true;
                            break;
                        }
                    }
                }

                if (!foundAtom) return false;
            }

            return true;
        }

        //
        // We have a set of constraints associated with the figure.
        // Also associated is a set of variables in which constraints are defined.
        // 1) Randomly choose one of the variables that defines the area formula, define it by its coordinate-based value.
        // 2) Push it through the constant propagator.
        // 3) From the list of variables, remove which of them that are now known.
        // 4) Repeat until the list of variables is empty.
        //
        public KnownMeasurementsAggregator AcquireKnowns()
        {
            // Acquire all unknown variables required to calculate the area.
            List<Segment> unknownAreaVars = this.GetAreaVariables();

            // The constraints for this problem.
            List<Constraint> constraints = this.GetConstraints();

            // The values we must state to the user in order to solve the problem.
            List<Segment> assumptions = new List<Segment>();

            //
            // Loop until all variables are known.
            //
            KnownMeasurementsAggregator known = new KnownMeasurementsAggregator();
            while (unknownAreaVars.Any())
            {
                // Acquire a new assumption.
                Segment newAssumption = unknownAreaVars[0];

                // remove that assumption since it is now known; add as an assumption.
                unknownAreaVars.RemoveAt(0);
                assumptions.Add(newAssumption);

                // Set this value as known with its intrinsic (corrdinate-based) length.
                known.AddSegmentLength(newAssumption, newAssumption.Length);

                // Propagate the new information through the constraints.
                ConstantPropagator.Propogate(known, constraints);

                // Check if any of the unknown variables are now known through constant propagation.
                unknownAreaVars = AcquireCurrentUnknowns(known, unknownAreaVars);
            }

            //
            // Create the known object.
            //
            KnownMeasurementsAggregator trueAssumptions = new KnownMeasurementsAggregator();
            foreach (Segment seg in assumptions)
            {
                trueAssumptions.AddSegmentLength(seg, seg.Length);
            }

            return trueAssumptions;
        }

        //
        // Filter the list of unknowns by any new information.
        //
        private List<Segment> AcquireCurrentUnknowns(KnownMeasurementsAggregator known, List<Segment> unknowns)
        {
            List<Segment> newUnknowns = new List<Segment>();

            foreach (Segment unknown in unknowns)
            {
                if (known.GetSegmentLength(unknown) < 0) newUnknowns.Add(unknown);
            }

            return newUnknowns;
        }

        //
        // Append subtraction to this current problem; the subtraction occurs within the inner figure (shape).
        //
        public static FigSynthProblem AppendFigureSubtraction(FigSynthProblem that, FigSynthProblem toAppend)
        {
            BinarySynthOperation binaryAppend = toAppend as BinarySynthOperation;
            if (binaryAppend == null) return null;

            if (that is SubtractionSynth)
            {
                // Verify that the outer part of toAppend is a figure in this problem.
                Figure theShape = (binaryAppend.leftProblem as UnarySynth).figure;
                FigSynthProblem leftShapeProblem = that.GetSynthByShape(theShape);

                if (leftShapeProblem == null)
                {
                    throw new ArgumentException("Shape is not in the given problem: " + theShape);
                }

                //
                // Create the new subtraction node and insert it into the copy.
                //
                // Since the 'left' expression was a shape, the 'right' is the actual shape we are appending.
                SubtractionSynth newSub = new SubtractionSynth(leftShapeProblem, binaryAppend.rightProblem);

                return that.Copy().ReplaceUnary(theShape, newSub);
            }
            else if (that is AdditionSynth)
            {
                // Verify that the external form of that matches with the LHS of toAppend.
                Polygon outerPoly = Polygon.MakePolygon(that.GetExteriorSegments());
                if (!outerPoly.StructurallyEquals((binaryAppend.leftProblem as UnarySynth).figure))
                {
                    throw new ArgumentException("Exterior polygons do not match: " + (binaryAppend.leftProblem as UnarySynth).figure);
                }

                // Make a copy of that.
                return new SubtractionSynth(that.Copy(), (binaryAppend.rightProblem as UnarySynth).figure);
            }
            else throw new ArgumentException("Expected Addition or Subtraction; acquired neither.");
        }

        //
        // Append subtraction to this current problem; the subtraction occurs within one of the open atomic regions.
        //
        public static FigSynthProblem AppendAtomicSubtraction(FigSynthProblem that, FigSynthProblem toAppend)
        {
            BinarySynthOperation binaryAppend = toAppend as BinarySynthOperation;
            if (binaryAppend == null) return null;

            // Verify that the outer part of toAppend is an open atomic region.
            if (!that.openRegions.Contains(new ShapeAtomicRegion((binaryAppend.leftProblem as UnarySynth).figure)))
            {
                throw new ArgumentException("Shape is not an open atomic region: " + (binaryAppend.leftProblem as UnarySynth).figure);
            }

            // Since the 'left' expression was an open region, the 'right' is the actual shape we are appending.
            SubtractionSynth newSub = new SubtractionSynth(that.Copy(), binaryAppend.rightProblem);

            // Update the open regions to the inner-most shape.
            newSub.SetOpenRegions(toAppend.openRegions);

            return newSub;
        }

        //
        // Append addition to this current problem; addition is to an exterior segment.
        //
        public static FigSynthProblem AppendAtomicAddition(FigSynthProblem that, FigSynthProblem toAppend)
        {
            //            AdditionSynth
            // TBC
            return new AdditionSynth(that, toAppend);
        }

        //
        // Append subtraction to this current problem.
        //
        public static FigSynthProblem AppendAddition(FigSynthProblem that, FigSynthProblem toAppend)
        {
            BinarySynthOperation binaryAppend = toAppend as BinarySynthOperation;
            if (binaryAppend == null) return null;

            if (that is AdditionSynth)
            {
                // Verify that the external form of that matches with the LHS of toAppend.
                Polygon testPoly = Polygon.MakePolygon(that.GetExteriorSegments());
                if (!testPoly.StructurallyEquals((binaryAppend.leftProblem as UnarySynth).figure))
                {
                    throw new ArgumentException("Exterior polygons do not match: " + (binaryAppend.leftProblem as UnarySynth).figure);
                }
            }
            // Create the new synth object
            AdditionSynth newSum = new AdditionSynth(that.Copy(), (binaryAppend.rightProblem as UnarySynth).figure);

            // The open regions that may be modified consist of a union of regions.
            List<AtomicRegion> newOpenRegions = new List<AtomicRegion>(that.openRegions);
            newOpenRegions.AddRange(toAppend.openRegions);

            newSum.SetOpenRegions(newOpenRegions);

            return newSum;
        }
    }

    public abstract class BinarySynthOperation : FigSynthProblem
    {
        public FigSynthProblem leftProblem { get; protected set; }
        public FigSynthProblem rightProblem { get; protected set; }

        public BinarySynthOperation(FigSynthProblem ell, FigSynthProblem r)
        {
            leftProblem = ell;
            rightProblem = r;
        }

        public BinarySynthOperation(Figure ell, Figure r)
        {
            leftProblem = new UnarySynth(ell);
            rightProblem = new UnarySynth(r);
        }

        public BinarySynthOperation(FigSynthProblem ell, Figure r)
        {
            leftProblem = ell;
            rightProblem = new UnarySynth(r);
        }

        public override List<Constraint> GetConstraints()
        {
            List<Constraint> constraints = new List<Constraint>();

            constraints.AddRange(leftProblem.GetConstraints());
            constraints.AddRange(rightProblem.GetConstraints());

            return constraints;
        }

        public override List<Segment> GetAreaVariables()
        {
            List<Segment> areaVars = new List<Segment>();

            areaVars.AddRange(leftProblem.GetAreaVariables());
            areaVars.AddRange(rightProblem.GetAreaVariables());

            return areaVars;
        }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            BinarySynthOperation binarySynth = that as BinarySynthOperation;
            if (binarySynth == null) return false;

            // The outer shapes must be congruent.
            if (!this.leftProblem.IsSymmetricTo(binarySynth.leftProblem)) return false;

            // The outer shapes must be congruent.
            if (!this.rightProblem.IsSymmetricTo(binarySynth.rightProblem)) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }

        public override FigSynthProblem GetSynthByShape(Figure that)
        {
            FigSynthProblem left = leftProblem.GetSynthByShape(that);
            if (left != null) return left;

            return rightProblem.GetSynthByShape(that);
        }

        public override FigSynthProblem ReplaceUnary(Figure that, FigSynthProblem toSub)
        {
            leftProblem = leftProblem.ReplaceUnary(that, toSub);
            rightProblem = rightProblem.ReplaceUnary(that, toSub);

            return this;
        }

        public override List<Point> CollectPoints()
        {
            List<Point> points = new List<Point>(leftProblem.CollectPoints());
            List<Point> right = new List<Point>(rightProblem.CollectPoints());

            Utilities.AddUniqueList<Point>(points, right);

            return points;
        }

        public override List<Segment> CollectSegments()
        {
            List<Segment> segments = new List<Segment>(leftProblem.CollectSegments());
            List<Segment> right = new List<Segment>(rightProblem.CollectSegments());

            Utilities.AddUniqueList<Segment>(segments, right);

            return segments;
        }

        public override List<Segment> CollectCompleteSegments()
        {
            List<Segment> segments = new List<Segment>(leftProblem.CollectCompleteSegments());
            List<Segment> right = new List<Segment>(rightProblem.CollectCompleteSegments());

            Utilities.AddUniqueList<Segment>(segments, right);

            return segments;
        }

        public override List<Point> CollectCompletePoints()
        {
            List<Point> points = new List<Point>(leftProblem.CollectCompletePoints());
            List<Point> right = new List<Point>(rightProblem.CollectCompletePoints());

            Utilities.AddUniqueList<Point>(points, right);

            return points;
        }

        public override List<Arc> CollectArcs()
        {
            List<Arc> arcs = new List<Arc>(leftProblem.CollectArcs());
            List<Arc> right = new List<Arc>(rightProblem.CollectArcs());

            Utilities.AddUniqueList<Arc>(arcs, right);

            return arcs;
        }

        public override List<Circle> CollectCircles()
        {
            List<Circle> circles = new List<Circle>(leftProblem.CollectCircles());
            List<Circle> right = new List<Circle>(rightProblem.CollectCircles());

            Utilities.AddUniqueList<Circle>(circles, right);

            return circles;
        }

        public override List<GroundedClause> GetGivens()
        {
            List<GroundedClause> left = leftProblem.GetGivens();
            List<GroundedClause> right = rightProblem.GetGivens();

            left.AddRange(right);

            return left;
        }

        public override List<Midpoint> GetMidpoints()
        {
            List<Midpoint> left = new List<Midpoint>(leftProblem.GetMidpoints());
            List<Midpoint> right = new List<Midpoint>(rightProblem.GetMidpoints());

            left.AddRange(right);

            return left;
        }

        //
        // Appending result in a set of exterior segments that may be attached to;
        // interior points result when two shapes are appended together (snapped into at least two places)
        //
        public override List<Segment> GetExteriorSegments()
        {
            // Acquire all maximal subsegments.
            List<Segment> left = Utilities.FilterForMaximal(leftProblem.GetExteriorSegments());
            List<Segment> right = Utilities.FilterForMaximal(rightProblem.GetExteriorSegments());

            left.AddRange(right);

            // Remove any shared subsegments.
            List<Segment> sharedList;
            List<Segment> exterior = Utilities.FilterShared(left, out sharedList);

            if (sharedList.Count != 2)
            {
                throw new ArgumentException("Expected only 2 shared segments; found (" + sharedList.Count + ")");
            }

            // Shared an entire side.
            if (sharedList[0].StructurallyEquals(sharedList[1])) return exterior;

            // Shared a subsegment.
            Segment max;
            Segment min;
            if (sharedList[0].HasSubSegment(sharedList[1]))
            {
                max = sharedList[0];
                min = sharedList[1];
            }
            else
            {
                max = sharedList[1];
                min = sharedList[0];
            }

            Point shared = max.SharedVertex(min);
            exterior.Add(new Segment(max.OtherPoint(shared), min.OtherPoint(shared)));

            return exterior;
        }

        public override List<Figure> CollectSubtractiveFigures(bool subtractFlag)
        {
            List<Figure> left = new List<Figure>(leftProblem.CollectSubtractiveFigures(subtractFlag));
            List<Figure> right = new List<Figure>(rightProblem.CollectSubtractiveFigures(subtractFlag));

            left.AddRange(right);

            return left;
        }
    }

    public class SubtractionSynth : BinarySynthOperation
    {
        public SubtractionSynth(FigSynthProblem ell, FigSynthProblem r) : base(ell, r) { }
        public SubtractionSynth(Figure ell, Figure r) : base(ell, r) { }
        public SubtractionSynth(FigSynthProblem ell, Figure r) : base(ell, r) { }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            SubtractionSynth subSynth = that as SubtractionSynth;
            if (subSynth == null) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            double leftArea = leftProblem.EvaluateArea(known);
            double rightArea = rightProblem.EvaluateArea(known);

            if (leftArea < 0 || rightArea < 0) return -1;

            return leftArea - rightArea;
        }

        public override FigSynthProblem Copy()
        {
            SubtractionSynth copy = new SubtractionSynth(this.leftProblem.Copy(), this.rightProblem.Copy());

            copy.SetOpenRegions(new List<AtomicRegion>(this.openRegions));

            return copy;
        }

        public override string ToString()
        {
            return "( " + leftProblem.ToString() + " - " + rightProblem.ToString() + " )";
        }

        public override double GetCoordinateArea()
        {
            double lArea = leftProblem.GetCoordinateArea();
            double rArea = rightProblem.GetCoordinateArea();

            return lArea - rArea;
        }

        public override List<Figure> CollectSubtractiveFigures(bool subtractFlag)
        {
            List<Figure> left = new List<Figure>(leftProblem.CollectSubtractiveFigures(subtractFlag));
            List<Figure> right = new List<Figure>(rightProblem.CollectSubtractiveFigures(!subtractFlag));

            //
            // a - (b - c)
            //
            //if (rightProblem is SubtractionSynth)
            //{
            //    // Acquire the left as an outer region
            //    List<AtomicRegion> rightAtoms = rightProblem.GetOpenRegions();

            //    // Covert right atoms to figures to return...
            //    foreach (AtomicRegion atom in rightAtoms)
            //    {
            //        left.Add((atom as ShapeAtomicRegion).shape);
            //    }

            //    return left;
            //}
            //else
            //{
                left.AddRange(right);

                return left;
//            }
        }
    }

    public class AdditionSynth : BinarySynthOperation
    {
        public AdditionSynth(FigSynthProblem ell, FigSynthProblem r) : base(ell, r) { }
        public AdditionSynth(Figure ell, Figure r) : base(ell, r) { }
        public AdditionSynth(FigSynthProblem ell, Figure r) : base(ell, r) { }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            AdditionSynth addSynth = that as AdditionSynth;
            if (addSynth == null) return false;

            // The atomic regions have to match 1-1 and onto.
            return base.IsSymmetricTo(that);
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            double leftArea = leftProblem.EvaluateArea(known);
            double rightArea = rightProblem.EvaluateArea(known);

            if (leftArea < 0 || rightArea < 0) return -1;

            return leftArea + rightArea;
        }

        public override FigSynthProblem Copy()
        {
            AdditionSynth copy = new AdditionSynth(this.leftProblem.Copy(), this.rightProblem.Copy());

            copy.SetOpenRegions(new List<AtomicRegion>(this.openRegions));

            return copy;
        }

        public override string ToString()
        {
            return "( " + leftProblem.ToString() + " + " + rightProblem.ToString() + " )";
        }

        public override double GetCoordinateArea()
        {
            return leftProblem.GetCoordinateArea() + rightProblem.GetCoordinateArea();
        }
    }

    //
    // Unary figure
    //
    public class UnarySynth : FigSynthProblem
    {
        public Figure figure { get; protected set; }

        public UnarySynth(Figure fig) { figure = fig; }

        //
        // A symmetric scenario is one in which:
        //  1) The inner shapes are congruent
        //  2) The remaining atomic regions match 1-1.
        //
        public override bool IsSymmetricTo(FigSynthProblem that)
        {
            UnarySynth unarySynth = that as UnarySynth;
            if (unarySynth == null) return false;

            // The outer shapes must be congruent.
            return this.figure.CoordinateCongruent(unarySynth.figure);

            // The atomic regions have to match 1-1 and onto.
            // return base.IsSymmetricTo(that);
        }

        public override FigSynthProblem Copy()
        {
            return new UnarySynth(figure);
        }

        public override FigSynthProblem GetSynthByShape(Figure that)
        {
            if (this.figure.Equals(that)) return this;

            return null;
        }

        //
        // If the figure matches this unary, return the problem to sub.
        // Otherwise return this (which indicates no substitution).
        //
        public override FigSynthProblem ReplaceUnary(Figure that, FigSynthProblem toSub)
        {
            if (figure.Equals(that)) return toSub;

            return this;
        }

        public override List<Point> CollectPoints()
        {
            if (this.figure is Circle) return Utilities.MakeList<Point>((figure as Circle).center);

            if (this.figure is Arc) return Utilities.MakeList<Point>((figure as Arc).theCircle.center);

            if (this.figure is Polygon) return (this.figure as Polygon).points;

            return new List<Point>();
        }

        public override List<Segment> CollectSegments()
        {
            if (this.figure is Polygon) return (this.figure as Polygon).orderedSides;

            return new List<Segment>();
        }

        public override List<Segment> CollectCompleteSegments()
        {
            if (this.figure is Polygon) return (this.figure as Polygon).GetCompleteSideSegments();

            return new List<Segment>();
        }

        public override List<Point> CollectCompletePoints()
        {
            if (this.figure is Polygon) return (this.figure as Figure).allComposingPoints;

            return new List<Point>();
        }

        public override List<Arc> CollectArcs()
        {
            if (this.figure is Arc) return Utilities.MakeList<Arc>(figure as Arc);

            return new List<Arc>();
        }

        public override List<Circle> CollectCircles()
        {
            if (this.figure is Circle) return Utilities.MakeList<Circle>(figure as Circle);

            if (this.figure is Arc) return Utilities.MakeList<Circle>((figure as Arc).theCircle);

            return new List<Circle>();
        }

        public override List<Constraint> GetConstraints()
        {
            return figure.GetConstraints();
        }

        public override List<Segment> GetAreaVariables()
        {
            return figure.GetAreaVariables();
        }

        public override double EvaluateArea(KnownMeasurementsAggregator known)
        {
            return figure.GetArea(known);
        }

        public override string ToString()
        {
            return figure.ToString();
        }

        public override double GetCoordinateArea()
        {
            double area = figure.CoordinatizedArea();

            return area;
        }

        public override List<GroundedClause> GetGivens()
        {
            Polygon thisPoly = figure as Polygon;
            if (thisPoly == null) return new List<GroundedClause>();

            //
            // Create the simple version of the figure; we already have the strengthened version.
            //
            Polygon simple = null;
            if (thisPoly.points.Count == 3) simple = new Triangle(thisPoly.points);
            if (thisPoly.points.Count == 4) simple = new Quadrilateral(thisPoly.orderedSides[0], thisPoly.orderedSides[2],
                                                                       thisPoly.orderedSides[1], thisPoly.orderedSides[3]);

            return Utilities.MakeList<GroundedClause>(new Strengthened(simple, this.figure));
        }

        public override List<Figure> CollectSubtractiveFigures(bool subtractFlag)
        {
            return subtractFlag ? Utilities.MakeList<Figure>(this.figure) : new List<Figure>();
        }

        public override List<Midpoint> GetMidpoints()
        {
            return this.figure.GetMidpointClauses();
        }

        public override List<Segment> GetExteriorSegments()
        {
            return this.figure.GetCompleteSideSegments();
        }
    }
}