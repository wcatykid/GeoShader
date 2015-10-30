using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class ArcBisector : Bisector
    {
        public ArcBisector() : base() { }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            ArcBisector ab = obj as ArcBisector;
            if (ab == null) return false;
            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            ArcBisector ab = obj as ArcBisector;
            if (ab == null) return false;
            return base.Equals(obj);
        }
    }
}
