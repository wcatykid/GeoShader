using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class CoordinateRightIsoscelesTriangles : Definition
    {
        private readonly static string NAME = "Composing Right / Isosceles Triangles";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION);

        private static List<Strengthened> candidateIsosceles = new List<Strengthened>();
        private static List<Strengthened> candidateRight = new List<Strengthened>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateIsosceles.Clear();
            candidateRight.Clear();
        }

        public static void Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Instantiating FROM a right triangle
            //
            Strengthened streng = clause as Strengthened;
            if (streng == null) return;

            if (streng.strengthened is RightTriangle)
            {
                candidateRight.Add(streng);

                foreach (Strengthened iso in candidateIsosceles)
                {
                    InstantiateRightTriangle(streng, iso);
                }
            }
            else if (streng.strengthened is IsoscelesTriangle)
            {
                candidateIsosceles.Add(streng);

                foreach (Strengthened right in candidateRight)
                {
                    InstantiateRightTriangle(right, streng);
                }
            }
        }

        private static void InstantiateRightTriangle(Strengthened right, Strengthened iso)
        {
            Triangle rightTri = right.strengthened as Triangle;
            Triangle isoTri = iso.strengthened as Triangle;

            if (!rightTri.StructurallyEquals(isoTri)) return;

            rightTri.SetProvenToBeIsosceles();
            rightTri.SetProvenToBeRight();
            isoTri.SetProvenToBeRight();
            isoTri.SetProvenToBeIsosceles();
        }
    }
}