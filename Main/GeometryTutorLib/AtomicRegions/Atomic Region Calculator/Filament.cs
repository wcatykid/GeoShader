using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class Filament : Primitive
    {
        public List<Point> points;

        public Filament()
        {
            points = new List<Point>();
        }

        public void Add(Point pt)
        {
            points.Add(pt);
        }

        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.Append("Filament { ");
            for (int p = 0; p < points.Count; p++)
            {
                str.Append(points[p].ToString());
                if (p < points.Count - 1) str.Append(", ");
            }
            str.Append(" }");

            return str.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////
        //
        //                              Atomic Region Construction
        //
        ///////////////////////////////////////////////////////////////////////////////////////
        
        //
        // A matrix storing the memoized versions of atomic regions found: n x n where n is the number of points in the filament.
        //
        private Agg[,] memoized;

        //
        // Extract the atomic regions.
        //
        public List<AtomicRegion> ExtractAtomicRegions(UndirectedPlanarGraph.PlanarGraph graph, List<Circle> circles)
        {
            memoized = new Agg[points.Count, points.Count];

            // Recursively construct.
            return MakeRegions(graph, circles, 0, points.Count - 1).atoms;
        }

        private class Agg
        {
            public int beginPointIndex;
            public int endPointIndex;
            public int coveredPoints;
            public Circle outerCircle;
            public List<AtomicRegion> atoms;

            public Agg(int b, int e, int cov, Circle outCirc, List<AtomicRegion> ats)
            {
                beginPointIndex = b;
                endPointIndex = e;
                coveredPoints = cov;
                outerCircle = outCirc;
                atoms = ats;
            }
        }
        //
        // The order of the points in the filament were established by the algorithm
        // Recursively seek smaller and smaller circular regions.
        //
        // Returns the set of atomic regions characterized by the largest circle (containing all the other atoms).
        private Agg MakeRegions(UndirectedPlanarGraph.PlanarGraph graph, List<Circle> circles, int beginIndex, int endIndex)
        {
            if (memoized[beginIndex, endIndex] != null) return memoized[beginIndex, endIndex];

            //
            // Find the circle for these given points.
            //
            Circle outerCircle = null;
            foreach (Circle circle in circles)
            {
                if (circle.PointLiesOn(points[beginIndex]) && circle.PointLiesOn(points[endIndex])) outerCircle = circle;
            }

            //
            // Base Case: Gap between the given indices is 1.
            //
            if (endIndex - beginIndex == 1)
            {
                return new Agg(beginIndex, endIndex, -1, outerCircle, HandleConnection(graph, circles, points[beginIndex], points[endIndex]));

            }

            //
            // Look at all combinations of indices from beginIndex to endIndex; start with larger gaps between indices -> small gaps
            //
            Agg maxLeftCoveredAgg = null;
            Agg maxRightCoveredAgg = null;
            int maxCoveredNodes = 0;
            for (int gap = endIndex - beginIndex - 1; gap > 0; gap--)
            {
                for (int index = beginIndex; index < endIndex; index++)
                {
                    Agg left = MakeRegions(graph, circles, index, index + gap);
                    Agg right = MakeRegions(graph, circles, index + gap, endIndex);

                    // Check for new maxmimum coverage.
                    if (left.coveredPoints + right.coveredPoints > maxCoveredNodes)
                    {
                        maxLeftCoveredAgg = left;
                        maxRightCoveredAgg = right;
                    }

                    // Found complete coverage
                    if (left.coveredPoints + right.coveredPoints == endIndex - beginIndex + 1)
                    {
                        maxCoveredNodes = endIndex - beginIndex + 1;
                        break;
                    }
                }
            }

            //
            // We have the two maximal circles: create the new regions.
            //
            // The atoms from the left / right.
            List<AtomicRegion> atoms = new List<AtomicRegion>();
            atoms.AddRange(maxLeftCoveredAgg.atoms);
            atoms.AddRange(maxRightCoveredAgg.atoms);

            // New regions are based on this outer circle minus the left / right outer circles.
            AtomicRegion newAtomTop = new AtomicRegion();
            AtomicRegion newAtomBottom = new AtomicRegion();

            // The outer circle.
            newAtomTop.AddConnection(points[beginIndex], points[endIndex], ConnectionType.ARC, outerCircle);
            newAtomBottom.AddConnection(points[beginIndex], points[endIndex], ConnectionType.ARC, outerCircle);

            // The left / right maximal circles.
            newAtomTop.AddConnection(points[maxLeftCoveredAgg.beginPointIndex], points[maxLeftCoveredAgg.endPointIndex], ConnectionType.ARC, maxLeftCoveredAgg.outerCircle);
            newAtomBottom.AddConnection(points[maxLeftCoveredAgg.beginPointIndex], points[maxLeftCoveredAgg.endPointIndex], ConnectionType.ARC, maxLeftCoveredAgg.outerCircle);

            newAtomTop.AddConnection(points[maxRightCoveredAgg.beginPointIndex], points[maxRightCoveredAgg.endPointIndex], ConnectionType.ARC, maxRightCoveredAgg.outerCircle);
            newAtomBottom.AddConnection(points[maxRightCoveredAgg.beginPointIndex], points[maxRightCoveredAgg.endPointIndex], ConnectionType.ARC, maxRightCoveredAgg.outerCircle);

            atoms.Add(newAtomTop);
            atoms.Add(newAtomBottom);
            
            //
            // Make / return the new aggregator
            //
            return new Agg(beginIndex, endIndex, maxCoveredNodes, outerCircle, atoms);
        }



        //
        // Based on the two points, extract the circle which results in the connection (if the connection exists).
        //
        private List<AtomicRegion> HandleConnection(UndirectedPlanarGraph.PlanarGraph graph, List<Circle> circles, Point pt1, Point pt2)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            UndirectedPlanarGraph.PlanarGraphEdge edge = graph.GetEdge(pt1, pt2);

            if (edge == null) return atoms;

            //
            // Find the one circle that applies to this set of points.
            //
            Circle theCircle = null;
            foreach (Circle circle in circles)
            {
                if (circle.PointLiesOn(pt1) && circle.PointLiesOn(pt2)) theCircle = circle;
            }

            switch (edge.edgeType)
            {
                case UndirectedPlanarGraph.EdgeType.REAL_ARC:
                case UndirectedPlanarGraph.EdgeType.REAL_DUAL:
                    atoms.AddRange(CreateSectors(theCircle, pt1, pt2));
//                    atoms.AddRange(CreateSemiCircleRegions());
                    break;
            }

            return atoms;
        }

        private List<AtomicRegion> CreateSectors(Circle circle, Point pt1, Point pt2)
        {
            List<AtomicRegion> atoms = new List<AtomicRegion>();

            Segment diameter = new Segment(pt1, pt2);
            if (circle.DefinesDiameter(diameter))
            {
                Point midpt = circle.Midpoint(pt1, pt2);
                atoms.Add(new ShapeAtomicRegion(new Sector(new Semicircle(circle, pt1, pt2, midpt, diameter))));
                atoms.Add(new ShapeAtomicRegion(new Sector(new Semicircle(circle, pt1, pt2, circle.OppositePoint(midpt), diameter))));
            }
            else
            {
                atoms.Add(new ShapeAtomicRegion(new Sector(new MinorArc(circle, pt1, pt2))));
                atoms.Add(new ShapeAtomicRegion(new Sector(new MajorArc(circle, pt1, pt2))));
            }

            return atoms;
        }
    }
}