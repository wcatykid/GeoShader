using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a point that lies on a segmant.
    /// </summary>
    public class InMiddle : Descriptor
    {
        public Point point { get; private set; }
        public Segment segment { get; private set; }

        /// <summary>
        /// Create a new InMiddle statement
        /// </summary>
        /// <param name="p">A point that lies on the segment</param>
        /// <param name="segment">A segment</param>
        public InMiddle(Point p, Segment segment) : base()
        {
            this.point = p;
            this.segment = segment;
        }

        public override void DumpXML(Action<string, List<GroundedClause>> write)
        {
            GroundedClause[] children = { point, segment };
            write("InMiddle", new List<GroundedClause>(children));
        }

        //
        // Can this relationship can strengthened to a Midpoint?
        //
        public Strengthened CanBeStrengthened()
        {
            if (Utilities.CompareValues(Point.calcDistance(point, segment.Point1), Point.calcDistance(point, segment.Point2)))
            {
                return new Strengthened(this, new Midpoint(this));
            }

            return null;
        }

        public override bool CanBeStrengthenedTo(GroundedClause gc)
        {
            Midpoint midpoint = gc as Midpoint;
            if (midpoint == null) return false;

            return this.point.StructurallyEquals(midpoint.point) && this.segment.StructurallyEquals(midpoint.segment);
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            if (obj is Midpoint) return (obj as Midpoint).StructurallyEquals(this);

            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.point.StructurallyEquals(point) && im.segment.StructurallyEquals(segment);
        }

        public override bool Equals(Object obj)
        {
            //Causes infinite recursion -> if (obj is Midpoint) return (obj as Midpoint).Equals(this);

            InMiddle im = obj as InMiddle;
            if (im == null) return false;
            return im.point.Equals(point) && im.segment.Equals(segment);
        }

        public override string ToString()
        {
            return "InMiddle(" + point.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}
