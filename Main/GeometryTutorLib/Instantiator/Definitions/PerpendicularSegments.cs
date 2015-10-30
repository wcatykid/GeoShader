using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAbstractSyntax;

namespace GeometryTutorLib.GenericInstantiator
{
    public class PerpendicularSegments : Definition
    {
        private readonly static string NAME = "Perpendicular segments from intersection";

        public PerpendicularSegments() { }

        //
        // Collinear(A, B, C, D, ...) -> Angle(A, B, C), Angle(A, B, D), Angle(A, C, D), Angle(B, C, D),...
        // All angles will have measure 180^o
        // There will be nC3 resulting clauses.
        //
        public static List<KeyValuePair<List<GroundedClause>, GroundedClause>> Instantiate(GroundedClause c)
        {
            if (!(c is Intersection)) return null;

            Intersection pCand = (Intersection)c;
            List<KeyValuePair<List<GroundedClause>, GroundedClause>> newGrounded = new List<KeyValuePair<List<GroundedClause>, GroundedClause>>();

            if (pCand.isPerpendicular == true)
            {
                Perpendicular newPerpendicular = new Perpendicular(pCand.lhs,pCand.rhs, NAME);
                List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(pCand);
                newGrounded.Add(new KeyValuePair<List<GroundedClause>, GroundedClause>(antecedent, newPerpendicular));
             }
                        
                    
                
           

            return newGrounded;
        }
    }
}