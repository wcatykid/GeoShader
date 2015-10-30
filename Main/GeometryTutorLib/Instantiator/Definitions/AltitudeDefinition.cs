using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class AltitudeDefinition : Definition
    {
        private readonly static string NAME = "Definition of Altitude";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ALTITUDE_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateAltitude.Clear();
            candidateIntersection.Clear();
            candidatePerpendicular.Clear();
            candidateStrengthened.Clear();
            candidateTriangle.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ALTITUDE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Triangle || clause is Perpendicular || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateToAltitude(clause));
            }

            else if (clause is Altitude || clause is Intersection)
            {
                newGrounded.AddRange(InstantiateFromAltitude(clause));
            }

            return newGrounded;
        }

        //
        //       A
        //      /|\
        //     / | \
        //    /  |  \
        //   /   |_  \
        //  /____|_|__\
        // B     M     C
        //
        // Altitude(Segment(A, M), Triangle(A, B, C)), Intersection(M, Segment(A, M), Segment(B, C)) -> Perpendicular(M, Segment(A, M), Segment(B, C))
        //
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Altitude> candidateAltitude = new List<Altitude>();
        private static List<EdgeAggregator> InstantiateFromAltitude(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Intersection)
            {
                Intersection inter = clause as Intersection;

                // We are only interested in straight-angle type intersections
                if (inter.StandsOnEndpoint()) return newGrounded;
                if (!inter.IsPerpendicular()) return newGrounded;

                foreach (Altitude altitude in candidateAltitude)
                {
                    newGrounded.AddRange(InstantiateFromAltitude(inter, altitude));
                }

                candidateIntersection.Add(inter);
            }
            else if (clause is Altitude)
            {
                Altitude altitude  = clause as Altitude;

                foreach (Intersection inter in candidateIntersection)
                {
                    newGrounded.AddRange(InstantiateFromAltitude(inter, altitude));
                }

                candidateAltitude.Add(altitude);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromAltitude(Intersection inter, Altitude altitude)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The intersection should contain the altitude segment
            if (!inter.HasSegment(altitude.segment)) return newGrounded;

            // The triangle should contain the other segment in the intersection
            Segment triangleSide = altitude.triangle.CoincidesWithASide(inter.OtherSegment(altitude.segment));
            if (triangleSide == null) return newGrounded;
            if (!inter.OtherSegment(altitude.segment).HasSubSegment(triangleSide)) return newGrounded;

            //
            // Create the Perpendicular relationship
            //
            Strengthened streng = new Strengthened(inter, new Perpendicular(inter));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(inter);
            antecedent.Add(altitude);

            newGrounded.Add(new EdgeAggregator(antecedent, streng, annotation));

            return newGrounded;
        }

        //
        //       A
        //      /|\
        //     / | \
        //    /  |  \
        //   /   |_  \
        //  /____|_|__\
        // B     M     C
        //
        // Triangle(A, B, C), Perpendicular(M, Segment(A, M), Segment(B, C)) -> Altitude(Segment(A, M), Triangle(A, B, C))
        //
        private static List<Triangle> candidateTriangle = new List<Triangle>();
        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<EdgeAggregator> InstantiateToAltitude(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Triangle)
            {
                Triangle tri = clause as Triangle;

                foreach (Perpendicular perp in candidatePerpendicular)
                {
                    newGrounded.AddRange(InstantiateToAltitude(tri, perp, perp));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToAltitude(tri, streng.strengthened as Perpendicular, streng));
                }

                candidateTriangle.Add(tri);
            }
            else if (clause is Perpendicular)
            {
                Perpendicular perp = clause as Perpendicular;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToAltitude(tri, perp, perp));
                }

                candidatePerpendicular.Add(perp);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                // Only interested in strenghthened intersection -> perpendicular or -> perpendicular bisector
                if (!(streng.strengthened is Perpendicular)) return newGrounded;

                foreach (Triangle tri in candidateTriangle)
                {
                    newGrounded.AddRange(InstantiateToAltitude(tri, streng.strengthened as Perpendicular, streng));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToAltitude(Triangle triangle, Perpendicular perp, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Acquire the side of the triangle containing the intersection point
            // This point may or may not be directly on the triangle side
            Segment baseSegment = triangle.GetSegmentWithPointOnOrExtends(perp.intersect);
            if (baseSegment == null) return newGrounded;

            // The altitude must pass through the intersection point as well as the opposing vertex
            Point oppositeVertex = triangle.OtherPoint(baseSegment);

            Segment altitude = new Segment(perp.intersect, oppositeVertex);

            // The alitude must alig with the intersection
            if (!perp.ImpliesRay(altitude)) return newGrounded;

            // The opposing side must align with the intersection
            if (!perp.OtherSegment(altitude).IsCollinearWith(baseSegment)) return newGrounded;


            //
            // Create the new Altitude object
            //
            Altitude newAltitude = new Altitude(triangle, altitude);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(triangle);
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newAltitude, annotation));

            //
            // Check if this induces a second altitude for a right triangle (although we don't know this is a strengthened triangle)
            // The intersection must be on the vertex of the triangle
            if (triangle.HasPoint(perp.intersect))
            {
                Angle possRightAngle = new Angle(triangle.OtherPoint(new Segment(perp.intersect, oppositeVertex)), perp.intersect, oppositeVertex);

                if (triangle.HasAngle(possRightAngle))
                {
                    Altitude secondAltitude = new Altitude(triangle, new Segment(perp.intersect, oppositeVertex));
                    newGrounded.Add(new EdgeAggregator(antecedent, secondAltitude, annotation));
                }
            }

            return newGrounded;
        }
    }
}