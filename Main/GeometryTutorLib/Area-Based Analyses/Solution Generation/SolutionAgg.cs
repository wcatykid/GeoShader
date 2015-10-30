using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.Area_Based_Analyses
{
    //
    // Lightweight class used for memoizing shaded area solutions.
    //
    public class SolutionAgg
    {
        public enum SolutionType { UNKNOWN, INCOMPUTABLE, COMPUTABLE };

        public IndexList atomIndices;
        public ComplexRegionEquation solEq;
        public double solArea;
        public SolutionType solType;

        public SolutionAgg()
        {
            atomIndices = new IndexList();
            solEq = null;
            solArea = -1;
            solType = SolutionType.UNKNOWN;
        }

        public bool IsDirectArea()
        {
            return solEq.IsIdentity() && solType == SolutionType.COMPUTABLE;
        }

        public override bool Equals(object obj)
        {
            SolutionAgg that = obj as SolutionAgg;
            if (that == null) return false;

            return Utilities.EqualOrderedSets(this.atomIndices.orderedIndices, that.atomIndices.orderedIndices);
        }

        public override string ToString()
        {
            return atomIndices.ToString() + " Area(" + solArea + ")";
        }

        public override int GetHashCode() { return base.GetHashCode(); }
    }
}
