using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class ArcInMiddle : Descriptor
    {
        public Point point { get; private set; }
        public Arc arc { get; private set; }

        public ArcInMiddle(Point p, Arc a) : base()
        {
            this.point = p;
            this.arc = a;
        }

        ////
        //// Can this relationship can strengthened to a Midpoint?
        ////
        //public Strengthened CanBeStrengthened()
        //{
        //    if (Utilities.CompareValues(Point.calcDistance(point, Arc.Point1), Point.calcDistance(point, Arc.Point2)))
        //    {
        //        return new Strengthened(this, new Midpoint(this));
        //    }

        //    return null;
        //}

        //public override bool CanBeStrengthenedTo(GroundedClause gc)
        //{
        //    Midpoint midpoint = gc as Midpoint;
        //    if (midpoint == null) return false;

        //    return this.point.StructurallyEquals(midpoint.point) && this.Arc.StructurallyEquals(midpoint.Arc);
        //}


        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ArcInMiddle im = obj as ArcInMiddle;
            if (im == null) return false;

            return im.point.StructurallyEquals(this.point) && im.arc.StructurallyEquals(this.arc);
        }

        public override bool Equals(Object obj)
        {
            ArcInMiddle im = obj as ArcInMiddle;
            if (im == null) return false;

            return im.point.Equals(this.point) && im.arc.Equals(this.arc);
        }

        public override string ToString()
        {
            return "ArcInMiddle(" + this.point.ToString() + ", " + this.arc.ToString() + ") " + justification;
        }
    }
}
