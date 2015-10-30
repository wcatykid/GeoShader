using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Live Geometry does not define ALL components of the figure, we must acquire those implied components.
    /// </summary>
    public class PolygonCalculator
    {
        private List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons;
        private List<GeometryTutorLib.ConcreteAST.Segment> segments;

        public PolygonCalculator(List<GeometryTutorLib.ConcreteAST.Segment> segs)
        {
            polygons = null;
            segments = segs;
        }

        public List<GeometryTutorLib.ConcreteAST.Polygon>[] GetPolygons()
        {
            if (polygons == null)
            {
                polygons = Polygon.ConstructPolygonContainer();
                CalculateImpliedPolygons();
            }

            return polygons;
        }

        //
        // Not all shapes are explicitly stated by the user; find all the implied shapes.
        // This populates the polygon array with any such shapes (concave or convex)
        //
        // Using a restricted powerset construction (from the bottom-up), construct all polygons.
        // Specifically: 
        //    (1) begin with all nC3 sets of segments.
        //    (2) If the set of 3 segments makes a triangle, create polygon, stop.
        //    (3) If the set of 3 segments does NOT make a triangle, construct all possible sets of size 4.
        //    (4) Inductively repeat for sets of size up MAX_POLY
        // No set of segments are collinear.
        //
        // This construction must be done in a breadth first manner (triangles then quads then pentagons...)
        //
        private void CalculateImpliedPolygons()
        {
            bool[,] eligible = DetermineEligibleCombinations();
            List<List<int>> constructedPolygonSets = new List<List<int>>();
            List<List<int>> failedPolygonSets = new List<List<int>>();

            //
            // Base case: construct all triangles.
            // For all non-triangle set of 3 segments, inductively look for polygons with more sides.
            //
            for (int s1 = 0; s1 < segments.Count - 2; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count - 1; s2++)
                {
                    if (eligible[s1, s2])
                    {
                        for (int s3 = s2 + 1; s3 < segments.Count - 0; s3++)
                        {
                            // Does this set create a triangle?
                            if (eligible[s1, s3] && eligible[s2, s3])
                            {
                                List<int> indices = new List<int>();
                                indices.Add(s1);
                                indices.Add(s2);
                                indices.Add(s3);

                                List<GeometryTutorLib.ConcreteAST.Segment> segs = MakeSegmentsList(indices);
                                GeometryTutorLib.ConcreteAST.Polygon poly = GeometryTutorLib.ConcreteAST.Polygon.MakePolygon(segs);
                                if (poly == null)
                                {
                                    failedPolygonSets.Add(indices);
                                }
                                else
                                {
                                    polygons[GeometryTutorLib.ConcreteAST.Polygon.GetPolygonIndex(indices.Count)].Add(poly);

                                    // Keep track of all existent sets of segments which created polygons.
                                    constructedPolygonSets.Add(indices);
                                }
                            }
                        }
                    }
                }
            }

            //
            // Inductively look for polygons with more than 3 sides.
            //
            InductivelyConstructPolygon(failedPolygonSets, eligible, constructedPolygonSets);
        }

        //
        // For each given set, add 1 new side (at a time) to the list of sides in order to construct polygons.
        //
        private void InductivelyConstructPolygon(List<List<int>> openPolygonSets, bool[,] eligible, List<List<int>> constructedPolygonSets)
        {
            // Stop if no sets to consider and grow from.
            if (!openPolygonSets.Any()) return;

            // Stop at a maximum number of sides;  we say n is the number of sides
            if (openPolygonSets[0].Count == GeometryTutorLib.ConcreteAST.Polygon.MAX_POLYGON_SIDES) return;

            int matrixLength = eligible.GetLength(0);

            // The set of sets that contains n+1 elements that do not make a polygon (sent to the next round)
            List<List<int>> failedPolygonSets = new List<List<int>>();

            // Breadth first consruction / traversal
            foreach (List<int> currentOpenSet in openPolygonSets)
            {
                // Since indices will be ordered least to greatest, we start looking at the largest index (last place in the list).
                for (int s = currentOpenSet[currentOpenSet.Count - 1]; s < matrixLength; s++)
                {
                    if (IsEligible(currentOpenSet, eligible, s))
                    {
                        List<int> newIndices = new List<int>(currentOpenSet);
                        newIndices.Add(s);

                        // Did we already create a polygon with a subset of these indices?
                        if (!GeometryTutorLib.Utilities.ListHasSubsetOfSet<int>(constructedPolygonSets, newIndices))
                        {
                            List<GeometryTutorLib.ConcreteAST.Segment> segs = MakeSegmentsList(newIndices);
                            GeometryTutorLib.ConcreteAST.Polygon poly = GeometryTutorLib.ConcreteAST.Polygon.MakePolygon(segs);
                            if (poly == null)
                            {
                                failedPolygonSets.Add(newIndices);
                            }
                            else
                            {
                                polygons[GeometryTutorLib.ConcreteAST.Polygon.GetPolygonIndex(segs.Count)].Add(poly);

                                // Keep track of all existent sets of segments which created polygons.
                                constructedPolygonSets.Add(newIndices);
                            }
                        }
                    }
                }
            }

            InductivelyConstructPolygon(failedPolygonSets, eligible, constructedPolygonSets);
        }

        // Make a list of segments based on indices; a helper function.
        private List<GeometryTutorLib.ConcreteAST.Segment> MakeSegmentsList(List<int> indices)
        {
            List<GeometryTutorLib.ConcreteAST.Segment> segs = new List<GeometryTutorLib.ConcreteAST.Segment>();

            foreach (int index in indices)
            {
                segs.Add(segments[index]);
            }

            return segs;
        }

        //
        // Given the set, is the given single term eligible?
        //
        private bool IsEligible(List<int> indices, bool[,] eligible, int newElement)
        {
            foreach (int index in indices)
            {
                if (!eligible[index, newElement]) return false;
            }

            return true;
        }

        //
        // Eligibility means that each pair of segment combinations does not:
        //   (1) Cross the other segment through the middle (creating an X or |-)
        //   (2) Coincide with overlap (or share a vertex)
        //
        private bool[,] DetermineEligibleCombinations()
        {
            bool[,] eligible = new bool[segments.Count, segments.Count]; // defaults to false

            for (int s1 = 0; s1 < segments.Count - 1; s1++)
            {
                for (int s2 = s1 + 1; s2 < segments.Count; s2++)
                {
                    // Crossing
                    if (!segments[s1].Crosses(segments[s2]))
                    {
                        if (!segments[s1].IsCollinearWith(segments[s2]))
                        {
                            eligible[s1, s2] = true;
                            eligible[s2, s1] = true;
                        }
                        else
                        {
                            //
                            // Coinciding and sharing a vertex, is disallowed by default.
                            //
                            //                                           __    __
                            // Coinciding ; Can have something like :   |  |__|  |
                            //                                          |________|
                            //
                            if (segments[s1].SharedVertex(segments[s2]) == null)
                            {
                                if (segments[s1].CoincidingWithoutOverlap(segments[s2]))
                                {
                                    eligible[s1, s2] = true;
                                    eligible[s2, s1] = true;
                                }
                            }
                        }
                    }
                }
            }

            return eligible;
        }
    }
}