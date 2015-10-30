using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    /// <summary>
    /// Describes a figure being strengthened.
    /// </summary>
    public class Strengthened : Descriptor
    {
        public GroundedClause original { get; private set; }
        public GroundedClause strengthened { get; private set; }

        public Strengthened(GroundedClause orig, GroundedClause streng) : base()
        {
            original = orig;
            strengthened = streng;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool StructurallyEquals(Object obj)
        {
            Strengthened s = obj as Strengthened;
            if (s == null) return false;
            return this.original.StructurallyEquals(s.original) && this.strengthened.StructurallyEquals(s.strengthened);
        }

        public override bool Equals(Object obj)
        {
            Strengthened thatS = obj as Strengthened;
            if (thatS == null) return false;
            return this.original.Equals(thatS.original) && this.strengthened.GetType() == thatS.strengthened.GetType();
        }

        public override string ToString()
        {
            return "Strengthened(" + original.ToString() + " -> " + strengthened.ToString() + ") " + justification;
        }
    }
}
