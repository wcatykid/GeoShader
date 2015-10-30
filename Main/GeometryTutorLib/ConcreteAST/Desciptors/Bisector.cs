using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Bisector : Descriptor
    {
        public Bisector() : base() { }

        public override int GetHashCode()
        {
            //Change this if the object is no longer immutable!!!
            return base.GetHashCode();
        }

        // This should be overriden
        public override bool StructurallyEquals(Object obj)
        {
            Bisector b = obj as Bisector;
            if (b == null) return false;
            return base.StructurallyEquals(obj);
        }

        public override bool Equals(Object obj)
        {
            Bisector b = obj as Bisector;
            if (b == null) return false;
            return base.Equals(obj);
        }
    }
}
