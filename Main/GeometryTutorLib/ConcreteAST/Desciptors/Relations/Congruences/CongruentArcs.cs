using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class CongruentArcs : Congruent
    {
        public Arc ca1 { get; protected set; }
        public Arc ca2 { get; protected set; }

        public CongruentArcs(Arc a1, Arc a2) : base()
        {
            ca1 = a1;
            ca2 = a2;

            if (!Utilities.CompareValues(a1.theCircle.radius, a2.theCircle.radius))
            {
                throw new ArgumentException("Arcs deduced congruent when radii differ " + this);
            }

            if (!Utilities.CompareValues(a1.minorMeasure, a2.minorMeasure))
            {
                throw new ArgumentException("Arcs deduced congruent when measure differ " + this);
            }
        }

        public override bool IsReflexive()
        {
            return ca1.StructurallyEquals(ca2);
        }

        public Arc OtherArc(Arc thatArc)
        {
            if (ca1.StructurallyEquals(thatArc)) return ca2;
            if (ca2.StructurallyEquals(thatArc)) return ca1;

            return null;
        }

        public override bool StructurallyEquals(Object clause)
        {
            CongruentArcs cts = clause as CongruentArcs;
            if (cts == null) return false;

            return this.ca1.StructurallyEquals(cts.ca1) || this.ca2.StructurallyEquals(cts.ca2) ||
                   this.ca2.StructurallyEquals(cts.ca1) || this.ca1.StructurallyEquals(cts.ca2);
        }

        public override bool Equals(Object clause)
        {
            CongruentArcs cts = clause as CongruentArcs;
            if (cts == null) return false;

            return this.ca1.Equals(cts.ca1) || this.ca2.Equals(cts.ca2) ||
                   this.ca2.Equals(cts.ca1) || this.ca1.Equals(cts.ca2);
        }

        public bool VerifyCongruentArcs()
        {
            return Utilities.CompareValues(ca1.minorMeasure, ca2.minorMeasure) && Utilities.CompareValues(ca1.theCircle.radius, ca2.theCircle.radius);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString() { return "Congruent(" + ca1.ToString() + ", " + ca2.ToString() + ") " + justification; }
    }
}