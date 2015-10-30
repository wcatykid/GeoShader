using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class InscribedQuadrilateralOppositeSupplementary : Theorem
    {
        private readonly static string NAME = "If a quadrilateral is inscribed in a circle, then its opposite angles are supplementary.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY);

        //         B ____________________ C
        //          /                   /
        //         /                   /
        //        / Circle (center)   /
        //       /        O          /
        //      /                   /
        //   A /___________________/ D
        //
        // Circle(O), Quad(A, B, C, D) -> Supplementary(Angle(ABC), Angle(ADC)), Supplementary(Angle(BAD), Angle(BCD))
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            Circle circle = clause as Circle;
            
            if (circle == null) return newGrounded;

            //
            // For each inscribed quadrilateral, generate accordingly.
            //
            foreach (Quadrilateral quad in circle.inscribedPolys[Polygon.QUADRILATERAL_INDEX])
            {
                Supplementary supp1 = new Supplementary(quad.topLeftAngle, quad.bottomRightAngle);
                Supplementary supp2 = new Supplementary(quad.bottomLeftAngle, quad.topRightAngle);

                // For hypergraph
                List<GroundedClause> antecedent = new List<GroundedClause>();
                antecedent.Add(circle);
                antecedent.Add(quad);

                newGrounded.Add(new EdgeAggregator(antecedent, supp1, annotation));
                newGrounded.Add(new EdgeAggregator(antecedent, supp2, annotation));
            }

            return newGrounded;
        }
    }
}