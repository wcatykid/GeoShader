using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Precomputer
{
    public class CoordinatePrecomputer
    {
        private List<Circle> circles;
        private List<Quadrilateral> quadrilaterals;
        private List<Triangle> triangles;
        private List<Segment> segments;
        private List<Angle> angles;
        public List<Collinear> collinear;

        private List<InMiddle> inMiddles;
        private List<Intersection> intersections;
        private List<Perpendicular> perpendiculars;
        private List<Parallel> parallels;

        private List<MinorArc> minorArcs;
        private List<MajorArc> majorArcs;
        private List<Semicircle> semiCircles;
        private List<Sector> sectors;
        private List<ArcInMiddle> arcInMiddle;

        private List<CircleSegmentIntersection> csIntersections;
        private List<CircleCircleIntersection> ccIntersections;

        public CoordinatePrecomputer(List<GroundedClause> figure)
        {
            circles = new List<Circle>();
            quadrilaterals = new List<Quadrilateral>();
            triangles = new List<Triangle>();
            segments = new List<Segment>();
            angles = new List<Angle>();
            collinear = new List<Collinear>();

            inMiddles = new List<InMiddle>();
            intersections = new List<Intersection>();
            perpendiculars = new List<Perpendicular>();
            parallels = new List<Parallel>();

            minorArcs = new List<MinorArc>();
            majorArcs = new List<MajorArc>();
            semiCircles = new List<Semicircle>();
            sectors = new List<Sector>();
            arcInMiddle = new List<ArcInMiddle>();

            FilterClauses(figure);
        }

        //
        // Split the figure into the constituent clauses
        //
        private void FilterClauses(List<GroundedClause> figure)
        {
            foreach (GroundedClause clause in figure)
            {
                if (clause is Circle)
                {
                    circles.Add(clause as Circle);
                }
                if (clause is Quadrilateral)
                {
                    quadrilaterals.Add(clause as Quadrilateral);
                }
                if (clause is Triangle)
                {
                    triangles.Add(clause as Triangle);
                }
                else if (clause is Angle)
                {
                    angles.Add(clause as Angle);
                }
                else if (clause is Segment)
                {
                    segments.Add(clause as Segment);
                }
                else if (clause is InMiddle)
                {
                    inMiddles.Add(clause as InMiddle);
                }
                else if (clause is Collinear)
                {
                    collinear.Add(clause as Collinear);
                }
                else if (clause is Parallel)
                {
                    parallels.Add(clause as Parallel);
                }
                else if (clause is Perpendicular)
                {
                    perpendiculars.Add(clause as Perpendicular);
                }
                else if (clause is Intersection)
                {
                    intersections.Add(clause as Intersection);
                }
                else if (clause is Collinear)
                {
                    collinear.Add(clause as Collinear);
                }
                else if (clause is MinorArc)
                {
                    minorArcs.Add(clause as MinorArc);
                }
                else if (clause is MajorArc)
                {
                    majorArcs.Add(clause as MajorArc);
                }
                else if (clause is Semicircle)
                {
                    semiCircles.Add(clause as Semicircle);
                }
                else if (clause is Sector)
                {
                    sectors.Add(clause as Sector);
                }
                else if (clause is ArcInMiddle)
                {
                    arcInMiddle.Add(clause as ArcInMiddle);
                }
            }
        }

        //
        // Numerically (via the coordinates from the UI), calculate the relationships among all figures
        //    1) Congruences (angles, segments, triangles, arcs)
        //    2) Parallel
        //    3) Equalities
        //    4) Perpendicular
        //    5) Similarity
        //
        List<Descriptor> descriptors = new List<Descriptor>();
        public List<Descriptor> GetPrecomputedRelations() { return descriptors; }

        public void CalculateRelations()
        {
            //
            // Segment, Parallel, and Perpendicular, and Congruences
            //
            for (int s1 = 0; s1 < segments.Count; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    //
                    // Congruence
                    //
                    if (segments[s1].CoordinateCongruent(segments[s2]))
                    {
                        descriptors.Add(new CongruentSegments(segments[s1], segments[s2]));
                    }

                    //
                    // Parallel amd Perpendicular Lines
                    //
                    if (segments[s1].CoordinateParallel(segments[s2]))
                    {
                        descriptors.Add(new Parallel(segments[s1], segments[s2]));
                    }
                    //
                    // Perpendicular, bisector, perpendicular bisector
                    //
                    else
                    {
                        // These are the general intersection points between the endpoints or on the endpoints of the segments (in some cases)
                        Point intersectionPerp = segments[s1].CoordinatePerpendicular(segments[s2]);
                        //                         is segment[s2] a bisector of segment[s1]?
                        Point intersectionBisec = segments[s1].CoordinateBisector(segments[s2]); // returns the actual intersection point
                        if (intersectionPerp != null && intersectionBisec != null)
                        {
                            descriptors.Add(new PerpendicularBisector(new Intersection(intersectionPerp, segments[s1], segments[s2]), segments[s2]));
                        }
                        else if (intersectionPerp != null)
                        {
                            descriptors.Add(new Perpendicular(new Intersection(intersectionPerp, segments[s1], segments[s2])));
                        }
                        else if (intersectionBisec != null)
                        {
                            descriptors.Add(new SegmentBisector(new Intersection(intersectionBisec, segments[s1], segments[s2]), segments[s2]));
                        }

                        // We may have a bisector in the other direction
                        intersectionBisec = segments[s2].CoordinateBisector(segments[s1]);
                        if (intersectionPerp != null && intersectionBisec != null)
                        {
                            descriptors.Add(new PerpendicularBisector(new Intersection(intersectionPerp, segments[s1], segments[s2]), segments[s1]));
                        }
                        else if (intersectionBisec != null)
                        {
                            descriptors.Add(new SegmentBisector(new Intersection(intersectionBisec, segments[s2], segments[s1]), segments[s1]));
                        }
                    }

                    //
                    // Proportional Line Segments
                    //
                    // Just generate if the ratio is really an integer or half-step
                    KeyValuePair<int, int> proportion = segments[s1].CoordinateProportional(segments[s2]);
                    if (proportion.Value != -1)
                    {
                        if (proportion.Value <= 2 || proportion.Key <= 2)
                        {
                            if (GeometryTutorLib.Utilities.DEBUG)
                            {
                                System.Diagnostics.Debug.WriteLine("< " + proportion.Key + ", " + proportion.Value + " >: " + segments[s1] + " : " + segments[s2]);
                            }
                            descriptors.Add(new SegmentRatio(segments[s1], segments[s2]));
                        }
                    }
                }
            }

            //
            // Angle congruences; complementary and supplementary
            //
            for (int a1 = 0; a1 < angles.Count; a1++)
            {
                for (int a2 = a1 + 1; a2 < angles.Count; a2++)
                {
                    if (angles[a1].CoordinateCongruent(angles[a2]) && !Utilities.CompareValues(angles[a1].measure, 180))
                    {
                        descriptors.Add(new CongruentAngles(angles[a1], angles[a2]));
                    }

                    if (angles[a1].IsComplementaryTo(angles[a2]))
                    {
                        descriptors.Add(new Complementary(angles[a1], angles[a2]));
                    }
                    else if (angles[a1].IsSupplementaryTo(angles[a2]))
                    {
                        descriptors.Add(new Supplementary(angles[a1], angles[a2]));
                    }

                    //
                    // Proportional Angle Measures
                    //
                    // Just generate if the ratio is really an integer or half-step
                    KeyValuePair<int, int> proportion = angles[a1].CoordinateProportional(angles[a2]);
                    if (proportion.Value != -1)
                    {
                        if (proportion.Value <= 2 || proportion.Key <= 2)
                        {
                            if (Utilities.DEBUG)
                            {
                                System.Diagnostics.Debug.WriteLine("< " + proportion.Key + ", " + proportion.Value + " >: " + angles[a1] + " : " + angles[a2]);
                            }
                            descriptors.Add(new ProportionalAngles(angles[a1], angles[a2]));
                        }
                    }
                }
            }

            //
            // Triangle congruences OR similarity (congruence is a stronger relationship than similarity)
            //
            for (int t1 = 0; t1 < triangles.Count; t1++)
            {
                for (int t2 = t1 + 1; t2 < triangles.Count; t2++)
                {
                    KeyValuePair<Triangle, Triangle> corresponding = triangles[t1].CoordinateCongruent(triangles[t2]);
                    if (corresponding.Key != null && corresponding.Value != null)
                    {
                        descriptors.Add(new CongruentTriangles(corresponding.Key, corresponding.Value));
                    }
                    else if (triangles[t1].CoordinateSimilar(triangles[t2]))
                    {
                        descriptors.Add(new SimilarTriangles(triangles[t1], triangles[t2]));
                    }
                }
            }

            //
            // Arc congruences
            //
            CalculateArcCongruences<MinorArc>(minorArcs);
            CalculateArcCongruences<MajorArc>(majorArcs);
            CalculateArcCongruences<Semicircle>(semiCircles);

            //
            // Calculate all segment relations to triangles: bisector, median, altitude, perpendicular bisector
            //
            foreach (Triangle tri in triangles)
            {
                foreach (Segment segment in segments)
                {
                    // Medians
                    if (tri.CoordinateMedian(segment))
                    {
                        descriptors.Add(new Median(segment, tri));
                    }

                    // Altitude
                    if (tri.CoordinateAltitude(segment))
                    {
                        descriptors.Add(new Altitude(tri, segment));
                    }
                }
            }

            // Calculate angle bisectors
            foreach (Angle angle in angles)
            {
                if (!Utilities.CompareValues(angle.measure, 180))
                {
                    foreach (Segment segment in segments)
                    {
                        // Angle Bisector
                        if (angle.CoordinateAngleBisector(segment))
                        {
                            descriptors.Add(new AngleBisector(angle, segment));
                        }
                    }
                }
            }

            //
            // Dumping the Relations
            //
            if (Utilities.DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Precomputed Relations");
                foreach (ConcreteAST.Descriptor descriptor in descriptors)
                {
                    System.Diagnostics.Debug.WriteLine(descriptor.ToString());
                }
            }
        }

        private void CalculateArcCongruences<T>(List<T> arcs) where T : Arc
        {
            for (int a1 = 0; a1 < arcs.Count; a1++)
            {
                for (int a2 = a1 + 1; a2 < arcs.Count; a2++)
                {
                    if (arcs[a1].CoordinateCongruent(arcs[a2]))
                    {
                        descriptors.Add(new CongruentArcs(arcs[a1], arcs[a2]));
                    }
                }
            }
        }

        //
        // Can we determine any stregnthening in the figure class (scalene to equilateral, etc)
        //
        List<Strengthened> strengthened = new List<Strengthened>();
        public List<Strengthened> GetStrengthenedClauses() { return strengthened; }

        public void CalculateStrengthening()
        {
            //
            // Can a quadrilateral be strenghtened? Quad -> trapezoid, Quad -> Parallelogram?, etc.
            //
            foreach (Quadrilateral quad in quadrilaterals)
            {
                strengthened.AddRange(Quadrilateral.CanBeStrengthened(quad));
            }

            //
            // Can a triangle be strenghtened? Scalene -> Isosceles -> Equilateral?
            //
            foreach (Triangle t in triangles)
            {
                strengthened.AddRange(Triangle.CanBeStrengthened(t));
            }

            //
            // Can an inMiddle relationship be classified as a Midpoint?
            //
            foreach (InMiddle im in inMiddles)
            {
                Strengthened s = im.CanBeStrengthened();
                if (s != null) strengthened.Add(s);
            }

            //
            // Right Angles
            //
            foreach (Angle angle in angles)
            {
                if (Utilities.CompareValues(angle.measure, 90))
                {
                    strengthened.Add(new Strengthened(angle, new RightAngle(angle)));
                }
            }

            //
            // Dumping the Strengthening
            //
            if (Utilities.DEBUG)
            {
                System.Diagnostics.Debug.WriteLine("Precomputed Strengthening");
                foreach (ConcreteAST.Strengthened s in strengthened)
                {
                    System.Diagnostics.Debug.WriteLine(s.ToString());
                }
            }
        }
    }
}
