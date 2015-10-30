using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class CongruentCircles : Congruent
    {
        public Circle cc1 { get; protected set; }
        public Circle cc2 { get; protected set; }

        public CongruentCircles(Circle c1, Circle c2) : base()
        {
            cc1 = c1;
            cc2 = c2;

            if (!Utilities.CompareValues(c1.radius, c2.radius))
            {
                throw new ArgumentException("Circles deduced congruent when radii differ " + this);
            }
        }

        public override bool IsReflexive()
        {
            return cc1.center.Equals(cc2.center);
        }

        public Circle OtherCircle(Circle thatCircle)
        {
            if (cc1.StructurallyEquals(thatCircle)) return cc2;
            if (cc2.StructurallyEquals(thatCircle)) return cc1;

            return null;
        }

        public override bool StructurallyEquals(Object clause)
        {
            CongruentCircles cts = clause as CongruentCircles;
            if (cts == null) return false;

            return this.cc1.StructurallyEquals(cts.cc1) || this.cc2.StructurallyEquals(cts.cc2) ||
                   this.cc2.StructurallyEquals(cts.cc1) || this.cc1.StructurallyEquals(cts.cc2);
        }

        public override bool Equals(Object clause)
        {
            CongruentCircles cts = clause as CongruentCircles;
            if (cts == null) return false;

            return this.cc1.Equals(cts.cc1) || this.cc2.Equals(cts.cc2) ||
                   this.cc2.Equals(cts.cc1) || this.cc1.Equals(cts.cc2);
        }

        public bool VerifyCongruentCircles()
        {
            return Utilities.CompareValues(cc1.radius, cc2.radius);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "Congruent(" + cc1.ToString() + ", " + cc2.ToString() + ") " + justification; }
    }
}