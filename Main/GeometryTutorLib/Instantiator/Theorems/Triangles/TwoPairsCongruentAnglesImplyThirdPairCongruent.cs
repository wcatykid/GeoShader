using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TwoPairsCongruentAnglesImplyThirdPairCongruent : Theorem
    {
        private readonly static string NAME = "Two pairs of Congruent Angles in two triangles imply third pair of triangles are congruent.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT);

        private static List<Triangle> candidateTriangles = new List<Triangle>();
        private static List<CongruentAngles> candidateCongruent = new List<CongruentAngles>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateTriangles.Clear();
        }

        //       A              D
        //      /\             /\
        //     /  \           /  \
        //    /    \         /    \
        //   /______\       /______\
        //  B        C      E       F
        //
        // In order for two triangles to be congruent, we require the following:
        //    Triangle(A, B, C), Triangle(D, E, F),
        //    Congruent(Angle(A, B, C), Angle(D, E, F)),
        //    Congruent(Angle(A, C, B), Angle(D, F, E)) -> Congruent(Angle(B, A, C), Angle(E, D, F)),
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause c)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have a segment or triangle?
            if (!(c is CongruentAngles) && !(c is Triangle)) return newGrounded;

            if (c is CongruentAngles)
            {
                CongruentAngles newCas = c as CongruentAngles;

                // Reflexive relationships will not relate two distinct triangles
                if (newCas.IsReflexive()) return newGrounded;

                // Check all combinations of triangles to see if they have congruent pairs
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateTriangles.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateTriangles.Count; j++)
                    {
                        foreach (CongruentAngles oldCas in candidateCongruent)
                        {
                            newGrounded.AddRange(CheckAndGenerateThirdCongruentPair(candidateTriangles[i], candidateTriangles[j], newCas, oldCas));
                        }
                    }
                }

                candidateCongruent.Add(newCas);
            }
            else if (c is Triangle)
            {
                Triangle newTri = c as Triangle;

                // Check all combinations of triangles to see if they have congruent pairs
                // This congruence must include the new segment congruence
                for (int i = 0; i < candidateCongruent.Count - 1; i++)
                {
                    for (int j = i + 1; j < candidateCongruent.Count; j++)
                    {
                        foreach (Triangle oldTri in candidateTriangles)
                        {
                            newGrounded.AddRange(CheckAndGenerateThirdCongruentPair(newTri, oldTri, candidateCongruent[i], candidateCongruent[j]));
                        }
                    }
                }

                candidateTriangles.Add(newTri);
            }

            return newGrounded;
        }

        //
        // Acquires all of the applicable congruent segments as well as congruent angles. Then checks for SAS
        //
        private static List<EdgeAggregator> CheckAndGenerateThirdCongruentPair(Triangle tri1, Triangle tri2, CongruentAngles cas1, CongruentAngles cas2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // We have eliminated all reflexive relationships at this point

            // The congruent relations should not share any angles
            if (cas1.AngleShared(cas2) != null) return newGrounded;

            // Both congruent pairs of angles must relate both triangles
            if (!cas1.LinksTriangles(tri1, tri2)) return newGrounded;
            if (!cas2.LinksTriangles(tri1, tri2)) return newGrounded;

            Angle firstAngleTri1 = tri1.AngleBelongs(cas1);
            Angle secondAngleTri1 = tri1.AngleBelongs(cas2);

            Angle firstAngleTri2 = tri2.AngleBelongs(cas1);
            Angle secondAngleTri2 = tri2.AngleBelongs(cas2);

            Angle thirdAngle1 = tri1.OtherAngle(firstAngleTri1, secondAngleTri1);
            Angle thirdAngle2 = tri2.OtherAngle(firstAngleTri2, secondAngleTri2);

            CongruentAngles newCas = new CongruentAngles(thirdAngle1, thirdAngle2);

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri1);
            antecedent.Add(tri2);
            antecedent.Add(cas1);
            antecedent.Add(cas2);

            newGrounded.Add(new EdgeAggregator(antecedent, newCas, annotation));

            return newGrounded;
        }
    }
}