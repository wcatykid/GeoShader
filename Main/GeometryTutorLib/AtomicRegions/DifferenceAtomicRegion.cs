using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class DifferenceAtomicRegion : AtomicRegion
    {
        public AtomicRegion outerShape { get; private set; }
        public List<AtomicRegion> innerShapes { get; private set; }

        public DifferenceAtomicRegion(AtomicRegion outer, AtomicRegion inner) : base()
        {
            outerShape = outer;
            innerShapes = new List<AtomicRegion>();
            innerShapes.Add(inner);
        }

        // Can the area of this region be calcualted?
        public override bool IsComputableArea()
        {
            if (!outerShape.IsComputableArea()) return false;
            foreach (AtomicRegion inner in innerShapes)
            {
                if (!inner.IsComputableArea()) return false;
            }
            return true;
        }

        public override double GetArea(KnownMeasurementsAggregator known)
        {
            double outerArea = outerShape.GetArea(known);
            if (outerArea < 0) return -1;

            double innerArea = 0;
            foreach (AtomicRegion inner in innerShapes)
            {
                double thisInnerArea = inner.GetArea(known);
                if (thisInnerArea < 0) return -1;

                innerArea += thisInnerArea;
            }

            return outerArea - innerArea;
        }

        public override bool PointLiesInside(Point pt)
        {
            if (!outerShape.PointLiesInside(pt)) return false;

            foreach (AtomicRegion inner in innerShapes)
            {
                if (inner.PointLiesInOrOn(pt)) return false;
            }

            return true;
        }

        //
        // Takes a shape turns it into an approximate polygon (if needed)
        // by converting all arcs into approximated arcs using many line segments.
        //
        public override Polygon GetPolygonalized()
        {
            throw new ArgumentException("Difference region is not a polygon.");

            // return null;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public bool HasInnerAtom(AtomicRegion that)
        {
            foreach (AtomicRegion inner in innerShapes)
            {
                if (inner.Equals(that)) return true;
            }

            return false;
        }

        public override bool Equals(Object obj)
        {
            DifferenceAtomicRegion thatAtom = obj as DifferenceAtomicRegion;
            if (thatAtom == null) return false;

            if (!outerShape.Equals(thatAtom.outerShape)) return false;

            if (this.innerShapes.Count != thatAtom.innerShapes.Count) return false;

            foreach (AtomicRegion inner in innerShapes)
            {
                if (!thatAtom.HasInnerAtom(inner)) return false;
            }

            return true;
        }

        public override string ToString()
        {
            String retString = "DifferenceAtom: (" + outerShape.ToString() + " - ";

            for (int i = 0; i < innerShapes.Count; i++)
            {
                retString += innerShapes[i].ToString();
                if (i < innerShapes.Count - 1) retString += " - ";
            }

            return retString;
        }
    }
}
