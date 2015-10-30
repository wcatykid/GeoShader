using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAbstractSyntax
{
    public class ConcreteSupplementaryAngles : Descriptor
    {
        public ConcreteAngle ca1 { get; private set; }
        public ConcreteAngle ca2 { get; private set; }

        public ConcreteAngle GetFirstAngle() { return ca1; }
        public ConcreteAngle GetSecondAngle() { return ca2; }

        //
        // Deduced Node Information
        //
        private List<ConcreteCongruentSegments> facts; // CTA: needed?
        public ConcreteSupplementaryAngles(ConcreteAngle a1, ConcreteAngle a2, string just) : base()
        {
            ca1 = a1;
            ca2 = a2;
            justification = just;
        }

        public override bool Equals(Object c)
        {
            if (!(c is ConcreteSupplementaryAngles)) return false;

            ConcreteSupplementaryAngles cca = (ConcreteSupplementaryAngles)c;

            //If this checks that the set of angles is equal to the set in question, I think it should work for supplementary sets the same as congruent sets
            return ca1.Equals(cca.ca1) && ca2.Equals(cca.ca2) || ca1.Equals(cca.ca2) && ca2.Equals(cca.ca1);
        }

        
        /*  Return both angles here?  Not sure what this was for in congruent angles
        // Return the shared angle in both congruences
        public ConcreteAngle SegmentShared(ConcreteCongruentAngles thatCC)
        {
            if (SharesNumClauses(thatCC) != 1) return null;

            return ca1.Equals(thatCC.ca1) || ca1.Equals(thatCC.ca2) ? ca1 : ca2;
        }
        */

        // Given one of the angles in the pair, return the other
        public ConcreteAngle OtherAngle(ConcreteAngle cs)
        {
            if (cs.Equals(ca1)) return ca2;
            if (cs.Equals(ca2)) return ca1;
            return null;
        }

        public void BuildUnparse(StringBuilder sb, int tabDepth)
        {
            //Console.WriteLine("To Be Implemented");
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "Supplementary(" + ca1.ToString() + ", " + ca2.ToString() + "): " + justification;
        }
    }
}
