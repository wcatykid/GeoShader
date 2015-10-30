using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class AlgebraicSimilarTriangles : SimilarTriangles
    {
        public AlgebraicSimilarTriangles(Triangle t1, Triangle t2) : base(t1, t2) { }

        public override bool IsAlgebraic() { return true; }
        public override bool IsGeometric() { return false; }

        public override bool Equals(Object obj)
        {
            AlgebraicSimilarTriangles gst = obj as AlgebraicSimilarTriangles;
            if (gst == null) return false;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            //Change this if the objest is no longer immutable!!!
            return base.GetHashCode();
        }

        public override string ToString() { return "AlgebraicSimilar(" + st1.ToString() + ", " + st2.ToString() + ") " + justification; }
    }
}
