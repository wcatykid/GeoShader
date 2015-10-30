using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public abstract class AbstractParserMain
    {
        //
        // Calculate all of the implied components of the figure.
        //
        public ImpliedComponentCalculator implied { get; protected set; }

        public AbstractParserMain()
        {
            // Reset the factory so we get points that start back at __A.
            GeometryTutorLib.PointFactory.Reset();
        }

        //
        // Identify and return all atomic regions
        //
        public List<AtomicRegion> GetAtomicRegions()
        {
            return implied.atomicRegions;
        }
        

        public GeometryTutorLib.EngineUIBridge.ProblemDescription MakeProblemDescription(List<GroundedClause> givens)
        {
            GeometryTutorLib.EngineUIBridge.ProblemDescription pdesc = new GeometryTutorLib.EngineUIBridge.ProblemDescription();

            pdesc.givens = givens;

            implied.allFigurePoints.ForEach(f => pdesc.figure.Add(f));
            implied.angles.ForEach(f => pdesc.figure.Add(f));
            implied.arcInMiddle.ForEach(f => pdesc.figure.Add(f));
            implied.ccIntersections.ForEach(f => pdesc.figure.Add(f));
            implied.circles.ForEach(f => pdesc.figure.Add(f));
            implied.collinear.ForEach(f => pdesc.figure.Add(f));
            implied.csIntersections.ForEach(f => pdesc.figure.Add(f));
            implied.inMiddles.ForEach(f => pdesc.figure.Add(f));
            implied.majorArcs.ForEach(f => pdesc.figure.Add(f));
            implied.majorSectors.ForEach(f => pdesc.figure.Add(f));
            implied.semiCircles.ForEach(f => pdesc.figure.Add(f));
            implied.minorArcs.ForEach(f => pdesc.figure.Add(f));
            implied.minorSectors.ForEach(f => pdesc.figure.Add(f));
            implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX].ForEach(f => pdesc.figure.Add(f));
            implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.QUADRILATERAL_INDEX].ForEach(f => pdesc.figure.Add(f));
            implied.segments.ForEach(f => pdesc.figure.Add(f));
            implied.ssIntersections.ForEach(f => pdesc.figure.Add(f));

            return pdesc;
        }

        //
        // Given a list of grounded clauses, get the structurally unique.
        //
        public GroundedClause Get(GroundedClause clause)
        {
            if (clause is GeometryTutorLib.ConcreteAST.Point)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Point>(implied.points, clause as GeometryTutorLib.ConcreteAST.Point);
            }
            else if (clause is GeometryTutorLib.ConcreteAST.Segment)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Segment>(implied.segments, clause as GeometryTutorLib.ConcreteAST.Segment);
            }
            else if (clause is Intersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<Intersection>(implied.ssIntersections, clause as Intersection);
            }
            else if (clause is CircleCircleIntersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<CircleCircleIntersection>(implied.ccIntersections, clause as CircleCircleIntersection);
            }
            else if (clause is CircleSegmentIntersection)
            {
                return GeometryTutorLib.Utilities.GetStructurally<CircleSegmentIntersection>(implied.csIntersections, clause as CircleSegmentIntersection);
            }
            else if (clause is Angle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<Angle>(implied.angles, clause as Angle);
            }
            else if (clause is InMiddle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<InMiddle>(implied.inMiddles, clause as InMiddle);
            }
            else if (clause is MinorArc)
            {
                return GeometryTutorLib.Utilities.GetStructurally<MinorArc>(implied.minorArcs, clause as MinorArc);
            }
            else if (clause is Semicircle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<Semicircle>(implied.semiCircles, clause as Semicircle);
            }
            else if (clause is MajorArc)
            {
                return GeometryTutorLib.Utilities.GetStructurally<MajorArc>(implied.majorArcs, clause as MajorArc);
            }
            else if (clause is ArcInMiddle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<ArcInMiddle>(implied.arcInMiddle, clause as ArcInMiddle);
            }

            else if (clause is Triangle)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Polygon>(implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX], clause as GeometryTutorLib.ConcreteAST.Polygon);
            }
            else if (clause is Quadrilateral)
            {
                return GeometryTutorLib.Utilities.GetStructurally<GeometryTutorLib.ConcreteAST.Polygon>(implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.QUADRILATERAL_INDEX], clause as GeometryTutorLib.ConcreteAST.Polygon);
            }

            return null;
        }

        public Intersection GetIntersection(GeometryTutorLib.ConcreteAST.Segment segment1, GeometryTutorLib.ConcreteAST.Segment segment2)
        {
            Point inter = (Point)Get(segment1.FindIntersection(segment2));

            return (Intersection)Get(new Intersection(inter, segment1, segment2));
        }
    }
}