using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.TutorParser;

namespace GeometryTutorLib.AtomicRegionIdentifier
{
    /// <summary>
    /// Preprocesses figures into simple atomic regions. This mostly applies to circles where we generate necessary chords / radii.
    /// </summary>
    public static class ShapeAtomizer
    {
        public static List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> Atomize(List<GeometryTutorLib.ConcreteAST.Circle> circles)
        {
            List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> atoms = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();

            foreach (GeometryTutorLib.ConcreteAST.Circle circle in circles)
            {
                atoms.AddRange(Atomize(circle));
            }

            return new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
        }

        //
        // Atomize the circle by creating:
        //    (1) all radii connecting to other known polygon / circle intersection points. 
        //    (2) all chords connecting the radii
        //
        //    All constructed segments may intersect at imaginary points; these need to be calculated
        //
        public static List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> Atomize(GeometryTutorLib.ConcreteAST.Circle circle)
        {
            List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion> atoms = new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
            List<GeometryTutorLib.ConcreteAST.Point> interPts = circle.GetIntersectingPoints();

            //
            // Construct the radii
            //
            List<GeometryTutorLib.ConcreteAST.Segment> radii = new List<GeometryTutorLib.ConcreteAST.Segment>();
            foreach (GeometryTutorLib.ConcreteAST.Point interPt in interPts)
            {
                radii.Add(new GeometryTutorLib.ConcreteAST.Segment(circle.center, interPt));
            }

            //
            // Construct the chords
            //
            List<GeometryTutorLib.ConcreteAST.Segment> chords = new List<GeometryTutorLib.ConcreteAST.Segment>();
            for (int p1 = 0; p1 < interPts.Count - 1; p1++)
            {
                for (int p2 = p1 + 1; p2 < interPts.Count; p2++)
                {
                    chords.Add(new GeometryTutorLib.ConcreteAST.Segment(interPts[p1], interPts[p2]));
                }
            }

            //
            // Do any of the chords intersect the radii?
            //


            //
            // Do the chords intersect each other?
            //
            return new List<GeometryTutorLib.Area_Based_Analyses.Atomizer.AtomicRegion>();
        }
    }
}