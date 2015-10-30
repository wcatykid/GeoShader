using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using GeometryTutorLib.TutorParser;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.GeometryTestbed
{
    public class FigSynthShadedAreaProblem : ActualShadedAreaProblem
    {
        public FigSynthShadedAreaProblem(bool runOrNot, bool comp) : base(runOrNot, comp)
        {
        }

        public void SetPoints(List<Point> pts) { this.points = pts; }
        public void SetSegments(List<Segment> segs) { this.segments = segs; }
        public void SetCollinear(List<Collinear> coll) { this.collinear = coll; }
        public void SetCircles(List<Circle> circs) { this.circles = circs; }
        public void SetWantedRegions(List<AtomicRegion> atoms) { this.goalRegions = atoms; }
        public void SetKnowns(Area_Based_Analyses.KnownMeasurementsAggregator k) { this.known = k; }
        public void SetGivens(List<GroundedClause> gs) { this.given = gs; }

        public void InvokeParser()
        {
            parser = new GeometryTutorLib.TutorParser.HardCodedParserMain(points, collinear, segments, circles, true);
        }

        public List<AtomicRegion> GetRemainingRegionsFromParser(FigSynthProblem problem)
        {
            // Acquire all the figures we are subtracting.
            // false indicates an implied addition at the beginning.
            List<Figure> figures = problem.CollectSubtractiveFigures(false);


            // Acquire the remaining atomic regions.
            List<AtomicRegion> atoms = parser.implied.GetAtomicRegionsNotByFigures(figures);

            if (Utilities.FIGURE_SYNTHESIZER_DEBUG)
            {
                if (atoms.Count == 1)
                {
                    System.Diagnostics.Debug.WriteLine("Remaining atom area: " + (atoms[0] as ShapeAtomicRegion).shape.CoordinatizedArea());
                }
            }

            return atoms;
        }

        public UIProblemDrawer.ProblemDescription MakeUIProblemDescription()
        {
            UIProblemDrawer.ProblemDescription desc = new UIProblemDrawer.ProblemDescription();

            desc.Points = this.points;
            desc.Segments = this.segments;
            desc.Circles = this.circles;
            desc.Regions = this.goalRegions;

            return desc;
        }
    }
}