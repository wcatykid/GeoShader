using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class ShapeAtomicRegion : AtomicRegion
    {
        public Figure shape { get; private set; }

        public ShapeAtomicRegion(Figure f) : base()
        {
            shape = f;
            connections = f.MakeAtomicConnections();
        }

        public void ReshapeForStrenghthening(Figure f)
        {
            shape = f;
        }

        public override bool CoordinateCongruent(Figure that)
        {
            return shape.CoordinateCongruent(that);
        }
        
        public override bool CoordinateCongruent(AtomicRegion that)
        {
            ShapeAtomicRegion shapeAtom = that as ShapeAtomicRegion;

            if (shapeAtom == null) return false;

            return this.shape.CoordinateCongruent(shapeAtom.shape);
        }

        // Can the area of this region be calcualted?
        public override bool IsComputableArea() { return true; }
        public override double GetArea(KnownMeasurementsAggregator known)
        {
            if (thisArea > 0) return thisArea;

            thisArea = shape.GetArea(known);

            return thisArea;
        }

        public override bool PointLiesOn(Point pt)
        {
            if (pt == null) return false;

            return shape.PointLiesOn(pt);
        }

        public override bool PointLiesInside(Point pt)
        {
            if (pt == null) return false;

            if (this.PointLiesOn(pt)) return false;

            return shape.PointLiesInside(pt);
        }
        //
        // Takes a shape turns it into an approximate polygon (if needed)
        // by converting all arcs into approximated arcs using many line segments.
        //
        public override Polygon GetPolygonalized()
        {
            if (polygonalized != null) return polygonalized;

            polygonalized = shape.GetPolygonalized();

            return polygonalized;
        }

        //public override void AddOwnedFigure(Figure f)
        //{
        //    // Ensure we don't say this figure contains itself.
        //    if (!this.shape.StructurallyEquals(f)) containingFigures.Add(f);
        //}

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            ShapeAtomicRegion thatAtom = obj as ShapeAtomicRegion;
            if (thatAtom == null) return false;

            return shape.StructurallyEquals(thatAtom.shape) && base.Equals(obj);
        }

        public override string ToString()
        {
            return "ShapeAtom: (" + shape.ToString() + ")";
        }

        public override string CheapPrettyString()
        {
            return shape.CheapPrettyString();
        }

        public override bool Contains(AtomicRegion that)
        {
            ShapeAtomicRegion thatAtom = that as ShapeAtomicRegion;

            if (thatAtom != null)
            {
                return this.shape.Contains(thatAtom.shape);
            }
            else
            {
                return base.Contains(that);
            }
        }
    }
}
