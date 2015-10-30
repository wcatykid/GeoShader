using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicProportionalAngles : ProportionalAngles
    {
        public AlgebraicProportionalAngles(Angle angle1, Angle angle2) : base(angle1, angle2) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override bool Equals(Object obj)
        {
            AlgebraicProportionalAngles aps = obj as AlgebraicProportionalAngles;
            if (aps == null) return false;
            return base.Equals(aps);
        }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "AlgebraicProportional(" + largerAngle.ToString() + " < " + dictatedProportion + " > " + smallerAngle.ToString() + ") " + justification;
        }
    }
}
