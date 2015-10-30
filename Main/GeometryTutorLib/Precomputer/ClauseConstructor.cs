using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;
using System;

namespace GeometryTutorLib.Precomputer
{
    public static class ClauseConstructor
    {


        ////
        //// Generate all Triangle clauses based on segments
        ////
        //public static List<Triangle> GenerateTriangleClauses(List<GroundedClause> clauses, List<Segment> segments)
        //{
        //    List<Triangle> newTriangles = new List<Triangle>();
        //    for (int s1 = 0; s1 < segments.Count - 2; s1++)
        //    {
        //        for (int s2 = s1 + 1; s2 < segments.Count - 1; s2++)
        //        {
        //            Point vertex1 = segments[s1].SharedVertex(segments[s2]);
        //            if (vertex1 != null)
        //            {
        //                for (int s3 = s2 + 1; s3 < segments.Count; s3++)
        //                {
        //                    Point vertex2 = segments[s3].SharedVertex(segments[s1]);
        //                    Point vertex3 = segments[s3].SharedVertex(segments[s2]);
        //                    if (vertex2 != null && vertex3 != null)
        //                    {
        //                        // Vertices must be distinct
        //                        if (!vertex1.Equals(vertex2) && !vertex1.Equals(vertex3) && !vertex2.Equals(vertex3))
        //                        {
        //                            // Vertices must be non-collinear
        //                            Segment side1 = new Segment(vertex1, vertex2);
        //                            Segment side2 = new Segment(vertex2, vertex3);
        //                            Segment side3 = new Segment(vertex1, vertex3);
        //                            if (!side1.IsCollinearWith(side2))
        //                            {
        //                                // Construct the triangle based on the sides to ensure reflexivity clauses are generated
        //                                newTriangles.Add(new Triangle(ClauseConstructor.GetProblemSegment(clauses, side1), ClauseConstructor.GetProblemSegment(clauses, side2), ClauseConstructor.GetProblemSegment(clauses, side3)));
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
        //// Generate all Quadrilateral clauses based on segments
        ////
        //public static List<Quadrilateral> GenerateQuadrilateralClauses(List<GroundedClause> clauses, List<Segment> segments)
        //{
        //    List<Quadrilateral> newQuads = new List<Quadrilateral>();

        //    if (segments.Count < 4) return newQuads;

        //    for (int s1 = 0; s1 < segments.Count - 3; s1++)
        //    {
        //        for (int s2 = s1 + 1; s2 < segments.Count - 2; s2++)
        //        {
        //            for (int s3 = s2 + 1; s3 < segments.Count - 1; s3++)
        //            {
        //                for (int s4 = s3 + 1; s4 < segments.Count; s4++)
        //                {
        //                    Quadrilateral quad = Quadrilateral.GenerateQuadrilateral(segments[s1], segments[s2], segments[s3], segments[s4]);
        //                    if (quad != null) Utilities.AddUnique<Quadrilateral>(newQuads, quad);
        //                }
        //            }
        //        }
        //    }

        //    return newQuads;
        //}
    }
}