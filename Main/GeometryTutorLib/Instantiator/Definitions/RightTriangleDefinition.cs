using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RightTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Right Triangle";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION);

        private static List<RightAngle> candidateRightAngles = new List<RightAngle>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Triangle> candidateTriangles = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateStrengthened.Clear();
            candidateRightAngles.Clear();
            candidateTriangles.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION;

            //
            // Instantiating FROM a right triangle
            //
            Strengthened streng = clause as Strengthened;
            if (clause is RightTriangle) return InstantiateFromRightTriangle(clause as RightTriangle, clause);
            if (streng != null && streng.strengthened is RightTriangle) return InstantiateFromRightTriangle(streng.strengthened as RightTriangle, clause);

            //
            // Instantiating TO a right triangle
            //
            if (clause is RightAngle || clause is Strengthened || clause is Triangle)
            {
                return InstantiateToRightTriangle(clause);
            }

            return new List<EdgeAggregator>();
        }

        private static List<EdgeAggregator> InstantiateFromRightTriangle(RightTriangle rightTri, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            
            // Strengthen the old triangle to a right triangle
            Strengthened newStrengthened = new Strengthened(rightTri.rightAngle, new RightAngle(rightTri.rightAngle));

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newStrengthened, annotation));

            return newGrounded;
        }

        //   A
        //   |\
        //   | \
        //   |  \
        //   |   \
        //   |_   \
        //   |_|___\
        //   B      C
        //
        // Triangle(A, B, C), RightAngle(A, B, C) -> RightTriangle(A, B, C)
        //
        //
        public static List<EdgeAggregator> InstantiateToRightTriangle(GroundedClause clause)
        {
            // The list of new grounded clauses if they are deduced
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Triangle)
            {
                Triangle newTri = clause as Triangle;

                foreach (RightAngle ra in candidateRightAngles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(newTri, ra, ra));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(newTri, streng.strengthened as RightAngle, streng));
                }

                candidateTriangles.Add(clause as Triangle);
            }
            else if (clause is RightAngle)
            {
                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(tri, clause as RightAngle, clause));
                }

                candidateRightAngles.Add(clause as RightAngle);
            }
            if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is RightAngle)) return newGrounded;

                foreach (Triangle tri in candidateTriangles)
                {
                    newGrounded.AddRange(StrengthenToRightTriangle(tri, streng.strengthened as RightAngle, clause));
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        //
        // DO NOT generate a new clause, instead, report the result and generate all applicable
        //
        private static List<EdgeAggregator> StrengthenToRightTriangle(Triangle tri, RightAngle ra, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // This angle must belong to this triangle.
            if (!tri.HasAngle(ra)) return newGrounded;
            
            // Strengthen the old triangle to a right triangle
            Strengthened newStrengthened = new Strengthened(tri, new RightTriangle(tri));
            tri.SetProvenToBeRight();

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(tri);
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newStrengthened, annotation));

            return newGrounded;
        }
    }
}