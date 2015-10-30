using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class HypotenuseLeg : Theorem
    {

        private readonly static string NAME = "Hypotenuse Leg";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG);

        private static List<RightTriangle> candidateRightTriangles = new List<RightTriangle>();
        private static List<CongruentSegments> candidateSegments = new List<CongruentSegments>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateSegments.Clear();
            candidateStrengthened.Clear();
            candidateRightTriangles.Clear();
        }

        //    A
        //     |\
        //     | \
        //     |_ \
        //     |_|_\
        //    B     C
        // In order for two right triangles to be congruent, we require the following:
        //    RightTriangle(A, B, C), RightTriangle(D, E, F),
        //    Congruent(Segment(A, B), Segment(D, E)),
        //    Congruent(Segment(A, C), Segment(D, F)) -> Congruent(Triangle(A, B, C), Triangle(D, E, F)),
        //                                               Congruent(Segment(A, C), Angle(D, F)),
        //                                               Congruent(Angle(C, A, B), Angle(F, D, E)),
        //                                               Congruent(Angle(B, C, A), Angle(E, F, D)),
        //
        // Note: we need to figure out the proper order of the sides to guarantee congruence
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG;

            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is CongruentSegments)
            {
                CongruentSegments newCs = clause as CongruentSegments;

                // Check all combinations of strict right triangle objects
                for (int i = 0; i < candidateRightTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateRightTriangles.Count; j++)
                    {
                        foreach (CongruentSegments css in candidateSegments)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(candidateRightTriangles[i], candidateRightTriangles[j], newCs, css));
                        }
                    }
                }

                // Check all combinations of (1) strict right triangle and (2) a strengthened triangle
                foreach (RightTriangle rt in candidateRightTriangles)
                {
                    foreach (Strengthened streng in candidateStrengthened)
                    {
                        foreach (CongruentSegments css in candidateSegments)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(rt, streng, newCs, css));
                        }
                    }
                }

                // Check all combinations of strengthened triangles
                for (int i = 0; i < candidateStrengthened.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateStrengthened.Count; j++)
                    {
                        foreach (CongruentSegments css in candidateSegments)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(candidateStrengthened[i], candidateStrengthened[j], newCs, css));
                        }
                    }
                }

                // Add this segment to the list of possible clauses to unify later
                candidateSegments.Add(newCs);
            }
            //
            // A triangle is either strictly a right triangle object, or a triangle is strengthened to be a right triangle
            //
            else if (clause is RightTriangle)
            {
                RightTriangle newRt = clause as RightTriangle;

                // Check all combinations of strict right triangle objects
                foreach (RightTriangle oldRt in candidateRightTriangles)
                {
                    for (int i = 0; i < candidateSegments.Count - 1; i++)
                    {
                        for (int j = i + 1; j < candidateSegments.Count; j++)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(newRt, oldRt, candidateSegments[i], candidateSegments[j]));
                        }
                    }
                }

                // Check all combinations of (1) strict right triangle and (2) a strengthened triangle
                foreach (Strengthened streng in candidateStrengthened)
                {
                    for (int i = 0; i < candidateSegments.Count - 1; i++)
                    {
                        for (int j = i + 1; j < candidateSegments.Count; j++)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(newRt, streng, candidateSegments[i], candidateSegments[j]));
                        }
                    }
                }

                candidateRightTriangles.Add(newRt);
            }
            else if (clause is Strengthened)
            {
                Strengthened newStreng = clause as Strengthened;

                // Only interested in strengthened Right Triangles
                if (!(newStreng.strengthened is RightTriangle)) return newGrounded;

                // Check all combinations of strict right triangle objects
                foreach (RightTriangle oldRt in candidateRightTriangles)
                {
                    for (int i = 0; i < candidateSegments.Count - 1; i++)
                    {
                        for (int j = i + 1; j < candidateSegments.Count; j++)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(oldRt, newStreng, candidateSegments[i], candidateSegments[j]));
                        }
                    }
                }

                // Check all combinations of (1) strict right triangle and (2) a strengthened triangle
                foreach (Strengthened oldStreng in candidateStrengthened)
                {
                    for (int i = 0; i < candidateSegments.Count - 1; i++)
                    {
                        for (int j = i + 1; j < candidateSegments.Count; j++)
                        {
                            newGrounded.AddRange(ReconfigureAndCheck(newStreng, oldStreng, candidateSegments[i], candidateSegments[j]));
                        }
                    }
                }

                candidateStrengthened.Add(newStreng);
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments; then checks HL
        //
        private static List<EdgeAggregator> ReconfigureAndCheck(RightTriangle rt1, RightTriangle rt2, CongruentSegments css1, CongruentSegments css2)
        {
            return CollectAndCheckHL(rt1, rt2, css1, css2, rt1, rt2);
        }
        private static List<EdgeAggregator> ReconfigureAndCheck(RightTriangle rt1,  Strengthened streng, CongruentSegments css1, CongruentSegments css2)
        {
            return CollectAndCheckHL(rt1, streng.strengthened as RightTriangle, css1, css2, rt1, streng);
        }
        private static List<EdgeAggregator> ReconfigureAndCheck(Strengthened streng1,  Strengthened streng2, CongruentSegments css1, CongruentSegments css2)
        {
            return CollectAndCheckHL(streng1.strengthened as RightTriangle, streng2.strengthened as RightTriangle, css1, css2, streng1, streng2);
        }

        //
        // Acquires all of the applicable congruent segments; then checks HL
        //
        private static List<EdgeAggregator> CollectAndCheckHL(RightTriangle rt1, RightTriangle rt2, 
                                                                                                  CongruentSegments css1, CongruentSegments css2,
                                                                                                  GroundedClause original1, GroundedClause original2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The Congruence pairs must relate the two triangles
            if (!css1.LinksTriangles(rt1, rt2) || !css2.LinksTriangles(rt1, rt2)) return newGrounded;

            // One of the congruence pairs must relate the hypotenuses
            Segment hypotenuse1 = rt1.OtherSide(rt1.rightAngle);
            Segment hypotenuse2 = rt2.OtherSide(rt2.rightAngle);

            // Determine the specific congruence pair that relates the hypotenuses
            CongruentSegments hypotenuseCongruence = null;
            CongruentSegments nonHypotenuseCongruence = null;
            if (css1.HasSegment(hypotenuse1) && css1.HasSegment(hypotenuse2))
            {
                hypotenuseCongruence = css1;
                nonHypotenuseCongruence = css2;
            }
            else if (css2.HasSegment(hypotenuse1) && css2.HasSegment(hypotenuse2))
            {
                hypotenuseCongruence = css2;
                nonHypotenuseCongruence = css1;
            }
            else return newGrounded;

            // Sanity check that the non hypotenuse congruence pair does not contain hypotenuse
            if (nonHypotenuseCongruence.HasSegment(hypotenuse1) || nonHypotenuseCongruence.HasSegment(hypotenuse2)) return newGrounded;

            //
            // We now have a hypotenuse leg situation; acquire the point-to-point congruence mapping
            //
            List<Point> triangleOne = new List<Point>();
            List<Point> triangleTwo = new List<Point>();

            // Right angle vertices correspond
            triangleOne.Add(rt1.rightAngle.GetVertex());
            triangleTwo.Add(rt2.rightAngle.GetVertex());

            Point nonRightVertexRt1 = rt1.GetSegment(nonHypotenuseCongruence).SharedVertex(hypotenuse1);
            Point nonRightVertexRt2 = rt2.GetSegment(nonHypotenuseCongruence).SharedVertex(hypotenuse2);

            triangleOne.Add(nonRightVertexRt1);
            triangleTwo.Add(nonRightVertexRt2);

            triangleOne.Add(hypotenuse1.OtherPoint(nonRightVertexRt1));
            triangleTwo.Add(hypotenuse2.OtherPoint(nonRightVertexRt2));

            //
            // Construct the new deduced relationships
            //
            GeometricCongruentTriangles ccts = new GeometricCongruentTriangles(new Triangle(triangleOne),
                                                                               new Triangle(triangleTwo));

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original1);
            antecedent.Add(original2);
            antecedent.Add(css1);
            antecedent.Add(css2);

            newGrounded.Add(new EdgeAggregator(antecedent, ccts, annotation));

            // Add all the corresponding parts as new congruent clauses
            newGrounded.AddRange(CongruentTriangles.GenerateCPCTC(ccts, triangleOne, triangleTwo));

            return newGrounded;
        }
    }
}
