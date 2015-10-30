using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Represents a general polygon (which consists of n >= 3 points)
    /// </summary>
    public partial class Polygon : Figure
    {
        //
        // Requires strict counter-clockwise ordering of points...so may not work.
        //
        public override double CoordinatizedArea()
        {
            List<Point> localPoints = new List<Point>(this.points);

            localPoints.Add(points[0]);
            var area = Math.Abs(localPoints.Take(points.Count - 1)
               .Select((p, i) => (localPoints[i + 1].X - p.X) * (localPoints[i + 1].Y + p.Y)).Sum() / 2);

            return area;
        }

        public static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            return new List<FigSynthProblem>();
        }

        public static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            return new List<FigSynthProblem>();
        }

        //
        // Is this shape congruent to the other shape based purely on coordinates.
        //
        public override bool CoordinateCongruent(Figure that)
        {
            Polygon thatPoly = that as Polygon;
            if (thatPoly == null) return false;

            if (this.orderedSides.Count != thatPoly.orderedSides.Count) return false;

            // Utility variable.
            int numSides = orderedSides.Count;

            // Find all sides which match in length to the first side of this polygon.
            List<int> possIndices = new List<int>();

            for (int s = 0; s < numSides; s++)
            {
                if (Utilities.CompareValues(this.orderedSides[0].Length, thatPoly.orderedSides[s].Length))
                {
                    possIndices.Add(s);
                }
            }

            // No shared segment lengths
            if (!possIndices.Any()) return false;

            //
            // Using the possible indices as starting points, walk around the polygons to see if all segments / angles coordinate.
            // This is the FORWARD comparison direction.
            //
            foreach (int startIndex in possIndices)
            {
                bool congruent = true;
                for (int thisIndex = 0; thisIndex < numSides; thisIndex++)
                {
                    // Check the same side length
                    if (Utilities.CompareValues(this.orderedSides[thisIndex].Length, thatPoly.orderedSides[(startIndex + thisIndex) % numSides].Length))
                    {
                        // Angles must measure the same as well.
                        Angle thisTemp = new Angle(this.orderedSides[thisIndex], this.orderedSides[(thisIndex + 1) % numSides]);
                        Angle thatTemp = new Angle(thatPoly.orderedSides[(startIndex + thisIndex) % numSides],
                                                   thatPoly.orderedSides[(startIndex + thisIndex + 1) % numSides]);

                        if (!Utilities.CompareValues(thisTemp.measure, thatTemp.measure))
                        {
                            congruent = false;
                            break;
                        }
                    }
                }

                if (congruent) return true;
            }

            //
            // Using the possible indices as starting points, walk around the polygons to see if all segments / angles coordinate.
            // This is the REVERSE comparison direction.
            //
            foreach (int startIndex in possIndices)
            {
                bool congruent = true;
                for (int counter = 0, downCounter = 0; counter < numSides; counter++, downCounter = Utilities.Modulus(downCounter - 1, this.orderedSides.Count))
                {
                    // Check the same side length
                    if (Utilities.CompareValues(this.orderedSides[downCounter].Length, thatPoly.orderedSides[(startIndex + counter) % numSides].Length))
                    {
                        // Angles must measure the same as well.
                        Angle thisTemp = new Angle(this.orderedSides[downCounter], this.orderedSides[Utilities.Modulus(downCounter - 1, this.orderedSides.Count)]);
                        Angle thatTemp = new Angle(thatPoly.orderedSides[(startIndex + counter) % numSides],
                                                   thatPoly.orderedSides[(startIndex + counter + 1) % numSides]);

                        if (!Utilities.CompareValues(thisTemp.measure, thatTemp.measure))
                        {
                            congruent = false;
                            break;
                        }
                    }
                }

                if (congruent) return true;
            }

            return false;
        }

        // Number of recursive midpoints to add to as snap-to points: A---B with 1 means A--Mid--B; A---B with 2 means A-m1-Mid-m3-B
        public const int NUM_MID_SEGMENT_POINTS = 1;

        protected void FigureSynthesizerConstructor()
        {
#if !FIGURE_SYNTHESIZER
            return;
#else
            snapToPoints = ConstructAllMidpoints(this.points);

            //
            // Construct and populate the subsegments aligned with the ordered set of sides.
            //
            sideSubsegments = new List<Segment>[orderedSides.Count];
            completeSideSegments = new List<Segment>[orderedSides.Count];
            for (int i = 0; i < sideSubsegments.Length; i++)
            {
                sideSubsegments[i] = new List<Segment>();
                completeSideSegments[i] = new List<Segment>();
            }

            // How many points are between the side endpoints?
            int numMidPointsPerSide = (int)Math.Pow(2, NUM_MID_SEGMENT_POINTS) - 1;
            int numPointsPerSide = numMidPointsPerSide + 2;

            List<Point> sidePoints = new List<Point>();
            for (int p = 0, currSidePt = 0, side = 0; p < allComposingPoints.Count; p++)
            {
                sideSubsegments[side].Add(new Segment(allComposingPoints[p], allComposingPoints[(p+1) % allComposingPoints.Count]));
                sidePoints.Add(allComposingPoints[p]);

                currSidePt++;
                if (currSidePt == numPointsPerSide-1) // Num sub-segments per side is 1 less than the number of points.
                {
                    // Append the last point.
                    sidePoints.Add(allComposingPoints[(p + 1) % allComposingPoints.Count]);

                    //
                    // Construct all segments for this side.
                    //
                    for (int s1 = 0; s1 < sidePoints.Count; s1++)
                    {
                        for (int s2 = s1 + 1; s2 < sidePoints.Count; s2++)
                        {
                            completeSideSegments[side].Add(new Segment(sidePoints[s1], sidePoints[s2]));
                        }
                    }

                    sidePoints.Clear();

                    // Advance to the next side.
                    side++;
                    // Reset the current side counter
                    currSidePt = 0;
                }
            }
#endif
        }

        /// <summary>
        /// Constructs all midpoints for all segments of the polygon.
        /// </summary>
        private List<Point> ConstructAllMidpoints(List<Point> vertices)
        {
            allComposingPoints = new List<Point>();

            List<Point> middlePoints = new List<Point>();
            for (int s = 0; s < orderedSides.Count; s++)
            {
                Point start = orderedSides[s].SharedVertex(orderedSides[Utilities.Modulus(s - 1, orderedSides.Count)]);
                Point end = orderedSides[s].OtherPoint(start);

                // Construct all the necessary midpoints.
                List<Point> currMiddlePoints = ConstructMidpoints(start, end, 0);

                // Add to the overall list.
                allComposingPoints.Add(start);
                allComposingPoints.AddRange(currMiddlePoints);

                // Add to the distinct snapping set of midpoints.
                middlePoints.AddRange(currMiddlePoints);
            }

            return middlePoints;
        }

        private List<Point> ConstructMidpoints(Point end1, Point end2, int level)
        {
            List<Point> middlePoints = new List<Point>();

            // Recursive Base Case
            if (level == NUM_MID_SEGMENT_POINTS) return middlePoints;

            Point midpoint = PointFactory.GeneratePoint(end1.Midpoint(end2));

            //
            // Recursively construct all points between
            //
            middlePoints.AddRange(ConstructMidpoints(end1, midpoint, level + 1));

            middlePoints.Add(midpoint);

            // Add to the list of midpoints clauses (for instantiation during figure synthesis).
            midpoints.Add(new Midpoint(new InMiddle(midpoint, new Segment(end1, end2))));

            middlePoints.AddRange(ConstructMidpoints(midpoint, end2, level + 1));

            return middlePoints;
        }

        //
        // Since we are cutting segments into sections, there is a direct relationship between the split side lengths for each side.
        //
        protected void GetGranularMidpointEquations(out List<Equation> eqs, out List<Congruent> congs)
        {
            eqs = new List<Equation>();
            congs = new List<Congruent>();

            //
            // Each sub-segment is equal to each other on each side.
            //
            for (int side = 0; side < sideSubsegments.Length; side++)
            {
                for (int s1 = 0; s1 < sideSubsegments[side].Count - 1; s1++)
                {
                    for (int s2 = s1 + 1; s2 < sideSubsegments[side].Count; s2++)
                    {
                        congs.Add(new GeometricCongruentSegments(sideSubsegments[side][s1], sideSubsegments[side][s2]));
                    }
                }
            }

            //
            // Each sub-segment is equation to a fraction of the side length.
            //
            int factor = (int)Math.Pow(2, NUM_MID_SEGMENT_POINTS);
            NumericValue factorVal = new NumericValue(factor);

            if (factor != sideSubsegments[0].Count) throw new Exception("Disagreement with the number of sub-segments in polygon");

            for (int side = 0; side < sideSubsegments.Length; side++)
            {
                foreach (Segment subSeg in sideSubsegments[side])
                {
                                                                                // Factor * sub-segment = whole side
                    eqs.Add(new GeometricSegmentEquation(new Multiplication(factorVal, subSeg), orderedSides[side]));
                }
            }
        }
    }
}
