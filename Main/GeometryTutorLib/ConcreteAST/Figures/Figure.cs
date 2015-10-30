using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class Figure : GroundedClause
    {
        protected Figure()
        {
            subFigures = new List<Figure>();
            superFigures = new List<Figure>();
            polygonalized = null;
            atoms = new List<AtomicRegion>();
            intersectingPoints = new List<Point>();
        }

        // Can we compute the area of this figure?
        public virtual bool IsComputableArea() { return false; }
        public virtual bool CanAreaBeComputed(Area_Based_Analyses.KnownMeasurementsAggregator known) { return false; }
        public virtual double GetArea(Area_Based_Analyses.KnownMeasurementsAggregator known) { return -1; }

        public const int NUM_SEGS_TO_APPROX_ARC = 72;
        public virtual bool PointLiesInside(Point pt) { return false; }
        public virtual bool PointLiesOn(Point pt) { return false; }
        public virtual bool PointLiesInOrOn(Point pt)
        {
            if (pt == null) return false;

            return PointLiesOn(pt) || PointLiesInside(pt);
        }
        public virtual List<Segment> Segmentize() { return new List<Segment>(); }
        public virtual List<Point> GetApproximatingPoints() { return new List<Point>(); }

        protected List<Point> intersectingPoints;
        public void AddIntersectingPoint(Point pt) { Utilities.AddStructurallyUnique<Point>(intersectingPoints, pt); }
        public void AddIntersectingPoints(List<Point> pts) { pts.ForEach(p => this.AddIntersectingPoint(p)); }
        public List<Point> GetIntersectingPoints() { return intersectingPoints; }
        public virtual void FindIntersection(Segment that, out Point inter1, out Point inter2) { inter1 = null; inter2 = null; }
        public virtual void FindIntersection(Arc that, out Point inter1, out Point inter2) { inter1 = null; inter2 = null; }
        public virtual List<Connection> MakeAtomicConnections() { return new List<Connection>(); }
        public virtual bool Covers(AtomicRegion a) { throw new NotImplementedException(); }

        // An ORDERED list of collinear points.
        public List<Point> collinear { get; protected set; }
        public virtual void AddCollinearPoint(Point newPt) { throw new ArgumentException("Only segments or arcs have 'collinearity'"); }
        public virtual void AddCollinearPoints(List<Point> pts) { pts.ForEach(p => this.AddCollinearPoint(p)); }
        public virtual void ClearCollinear() { throw new ArgumentException("Only segments or arcs have 'collinearity'"); }

        protected List<Figure> superFigures;
        protected List<Figure> subFigures;
        public void AddSuperFigure(Figure f) { if (!Utilities.HasStructurally<Figure>(superFigures, f)) superFigures.Add(f); }
        public void AddSubFigure(Figure f) { if (!Utilities.HasStructurally<Figure>(subFigures, f)) subFigures.Add(f); }
        public Polygon polygonalized { get; protected set; }
        public List<AtomicRegion> atoms { get; protected set; }
        public virtual Polygon GetPolygonalized() { return null; }
        public virtual string CheapPrettyString() { return "TBD"; }

        //
        // Shape hierarchy for shaded region solution / problem synthesis.
        //
        protected ShapeAtomicRegion thisAtomicRegion;
        public ShapeAtomicRegion GetFigureAsAtomicRegion()
        {
            if (thisAtomicRegion == null) thisAtomicRegion = new ShapeAtomicRegion(this);
            return thisAtomicRegion;
        }
        private Area_Based_Analyses.TreeNode<Figure> shapeHierarchy;
        public bool HierarchyEstablished() { return shapeHierarchy != null; }
        public Area_Based_Analyses.TreeNode<Figure> Hierarchy() { return shapeHierarchy; }
        public void MakeLeaf()
        {
            shapeHierarchy = new Area_Based_Analyses.TreeNode<Figure>(this);
        }
        public void SetChildren(List<Figure> children)
        {
            if (shapeHierarchy == null) shapeHierarchy = new Area_Based_Analyses.TreeNode<Figure>(this);

            foreach (Figure child in children)
            {
                shapeHierarchy.AddChild(child.Hierarchy());
            }
        }

        public void AddAtomicRegion(AtomicRegion atom)
        {
            // Avoid adding an atomic region which is itself
            //if (atom is ShapeAtomicRegion)
            //{
            //    if ((atom as ShapeAtomicRegion).shape.StructurallyEquals(this)) return;
            //}

            if (atoms.Contains(atom)) return;

            atoms.Add(atom);
        }

        public void AddAtomicRegions(List<AtomicRegion> atoms)
        {
            foreach (AtomicRegion atom in atoms)
            {
                AddAtomicRegion(atom);
            }
        }


        public bool isShared() { return superFigures.Count > 1; }
        public List<Figure> getSuperFigures() { return superFigures; }

        //
        // A shape within this shape?
        //
        public virtual bool Contains(Figure that)
        {
            return thisAtomicRegion.Contains(that.GetFigureAsAtomicRegion());
        }

        public virtual bool Contains(List<Point> figurePoints, AtomicRegion atom)
        {
            // A figure contains itself.
            ShapeAtomicRegion shapeAtom = atom as ShapeAtomicRegion;
            if (shapeAtom != null)
            {
                if (this.StructurallyEquals(shapeAtom.shape)) return true;
            }

            //
            // Do all vertices of that lie on the interior of this figure
            //
            List<Point> thatVertices = atom.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesInOrOn(vertex)) return false;
            }

            //
            // Check all midpoints of conenctions are on the interior.
            //
            foreach (Connection thatConn in atom.connections)
            {
                if (!this.PointLiesInOrOn(thatConn.Midpoint())) return false;
            }

            //
            // For any intersections between the atomic regions, the resultant points of intersection must be on the perimeter.
            //
            AtomicRegion thisFigureRegion = this.GetFigureAsAtomicRegion();
            List<AtomicRegion.IntersectionAgg> intersections = thisFigureRegion.GetIntersections(figurePoints, atom);
            foreach (AtomicRegion.IntersectionAgg agg in intersections)
            {
                if (agg.overlap)
                {
                    // No-Op
                }
                else
                {
                    // An approximation may result in an intersection inside the figure (although we would expect on)
                    if (!this.PointLiesInOrOn(agg.intersection1)) return false;
                    if (agg.intersection2 != null)
                    {
                        if (!this.PointLiesInOrOn(agg.intersection2)) return false;
                    }
                }
            }

            return true;
        }

        //
        // Is this figure (or atomic region approx by polygon) completely contained in the given figure?
        //
        //public bool Contains(Polygon thatPoly)
        //{
        //    //
        //    // Special Cases:
        //    //
        //    // Disambiguate Major Sector from Minor Sector
        //    if (this is Sector && (this as Sector).theArc is MajorArc)
        //    {
        //        // Not only do all the points need to be inside, the midpoints do as well.
        //        foreach(Segment side in thatPoly.orderedSides)
        //        {
        //            Point midpt = side.Midpoint();
        //            if (!this.PointLiesInOrOn(midpt)) return false;
        //        }
        //    }

        //    //
        //    // General Case
        //    //
        //    foreach (Point thatPt in thatPoly.points)
        //    {
        //        if (!this.PointLiesInOrOn(thatPt)) return false;
        //    }

        //    return true;
        //}
    }
}
