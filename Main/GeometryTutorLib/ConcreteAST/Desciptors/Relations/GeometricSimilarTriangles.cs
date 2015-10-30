using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class GeometricSimilarTriangles : SimilarTriangles
    {
        public GeometricSimilarTriangles(Triangle t1, Triangle t2) : base(t1, t2) { }

        public override bool IsAlgebraic() { return false; }
        public override bool IsGeometric() { return true; }

        public override bool Equals(Object obj)
        {
            GeometricSimilarTriangles gst = obj as GeometricSimilarTriangles;
            if (gst == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the objest is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString() { return "GeometricSimilar(" + st1.ToString() + ", " + st2.ToString() + ") " + justification; }

        public override string ToPrettyString() { return st1.ToPrettyString() + " is similar to " + st2.ToPrettyString() + "."; }
    }
}
