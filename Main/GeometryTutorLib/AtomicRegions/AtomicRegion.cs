using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses.Atomizer
{
    public class AtomicRegion
    {
        private bool ordered; // Are the connections ordered?
        public List<Connection> connections { get; protected set; }
        public List<Figure> owners { get; protected set; }
        private Figure topOwner;

        //
        // <------- The following are for processing atomic regions.
        //
        private bool knownAtomic;
        public void SetKnownAtomic() { knownAtomic = true; }
        public bool IsKnownAtomic() { return knownAtomic; }
        public void Clear()
        {
            knownAtomic = false;
            containedAtoms = new List<AtomicRegion>();
        }

        private List<AtomicRegion> containedAtoms;
        public void SetContained(List<AtomicRegion> contained) { containedAtoms = contained; }
        public bool IsKnownNonAtomic() { return containedAtoms.Any(); }
        public List<AtomicRegion> GetContainedAtoms() { return containedAtoms; }
        public bool UnknownAtomicStatus() { return !IsKnownAtomic() && !IsKnownNonAtomic(); }
        //
        // <------- End processing atomic region members
        //

        //
        // <------- Figure Synthesis (begin) --------------------------------------------------------------------------
        //
        //
        // Is this shape congruent to the other shape based purely on coordinates.
        //
        public virtual bool CoordinateCongruent(Figure that)
        {
            throw new NotImplementedException();
        }

        private static void SplitConnections(AtomicRegion atom, out List<Segment> segs, out List<Arc> arcs)
        {
            segs = new List<Segment>();
            arcs = new List<Arc>();

            foreach (Connection conn in atom.connections)
            {
                if (conn.segmentOrArc != null)
                {
                    if (conn.type == ConnectionType.SEGMENT) segs.Add(conn.segmentOrArc as Segment);
                    if (conn.type == ConnectionType.ARC) arcs.Add(conn.segmentOrArc as Arc);
                }
            }
        }

        public virtual bool CoordinateCongruent(AtomicRegion that)
        {
            //
            // Collect segment and arc connections.
            //
            List<Segment> thisSegs = new List<Segment>();
            List<Arc> thisArcs = new List<Arc>();
            List<Segment> thatSegs = new List<Segment>();
            List<Arc> thatArcs = new List<Arc>();

            SplitConnections(this, out thisSegs, out thisArcs);
            SplitConnections(that, out thatSegs, out thatArcs);

            //
            // We must have the same number of each type of connections
            //
            if (thisSegs.Count != thatSegs.Count) return false;
            if (thisArcs.Count != thatArcs.Count) return false;

            //
            // This is a naive approach since a more formal approach should follow order of connections; should generally work
            //
            //
            // An atomic region must have the same number of segments, each of the same length.
            //
            bool[] marked = new bool[thisSegs.Count];
            foreach (Segment thisSeg in thisSegs)
            {
                bool found = false;
                for (int c = 0; c < thatSegs.Count; c++)
                {
                    if (!marked[c])
                    {
                        if (thisSeg.CoordinateCongruent(thatSegs[c]))
                        {
                            marked[c] = true;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) return false;
            }
            // Redundant:
            // if (marked.Contains(false)) return false;

            // Exit early if no arcs.
            if (!thisArcs.Any()) return true;

            //
            // An atomic region must have the same number of arcs, each of the same length (using the radius of the circle).
            //
            marked = new bool[thisArcs.Count];
            foreach (Arc thisArc in thisArcs)
            {
                bool found = false;
                for (int c = 0; c < thatArcs.Count; c++)
                {
                    if (!marked[c])
                    {
                        if (thisArc.CoordinateCongruent(thatArcs[c]))
                        {
                            marked[c] = true;
                            found = true;
                            break;
                        }
                    }
                }
                if (!found) return false;
            }
            // Redundant:
            // if (marked.Contains(false)) return false;

            return true;
        }
        //
        // <------- Figure Synthesis (end) --------------------------------------------------------------------------
        //

        // A version of this region that is an approximate polygon.
        public Polygon polygonalized { get; protected set; }

        public List<Point> GetApproximatingPoints() { return GetPolygonalized().points; }

        public AtomicRegion()
        {
            ordered = false;
            connections = new List<Connection>();
            owners = new List<Figure>();
            topOwner = null;
            knownAtomic = false;
            polygonalized = null;
            thisArea = -1;
        }

        public Figure GetTopMostShape() { return topOwner; }

        // Add a shape to the list which contains this region.
        public virtual void AddOwner(Figure f)
        {
            if (!Utilities.AddStructurallyUnique<Figure>(owners, f)) return;

            // Check if this new atomic region is the outermost owner.
            if (topOwner == null || f.Contains(topOwner)) topOwner = f;
        }

        public virtual void AddOwners(List<Figure> fs)
        {
            foreach (Figure f in fs) AddOwner(f);
        }

        public void ClearOwners()
        {
            owners.Clear();
            topOwner = null;
        }

        //public virtual void AddContained(AtomicRegion atom)
        //{
        //    if (!contained.Contains(atom)) contained.Add(atom);
        //}
        //public virtual void AddContained(List<AtomicRegion> atoms)
        //{
        //    foreach (AtomicRegion atom in atoms)
        //    {
        //        AddContained(atom);
        //    }
        //}

        //// Compose this atomic region with another atomic region (resulting in a set of regions).
        //public List<AtomicRegion> Compose(AtomicRegion that)
        //{
        //    return new List<AtomicRegion>();
        //}

        //// Compose this atomic region with a segment (resulting in a set of regions).
        //public List<AtomicRegion> Compose(Segment that);

        // Convert the region to a polygon and then use the normal detection technique to determine if it is in the interior.
        public virtual bool PointLiesInside(Point pt)
        {
            if (pt == null) return false;

            if (PointLiesOn(pt)) return false;

            return GetPolygonalized().IsInPolygon(pt);
        }

        public virtual bool PointLiesInOrOn(Point pt)
        {
            if (pt == null) return false;

            return PointLiesOn(pt) || PointLiesInside(pt);
        }

        //
        // Can the area of this region be calculated?
        //
        public virtual double GetArea(KnownMeasurementsAggregator known) { return thisArea; }
        protected double thisArea;

        // Can the area of this region be calculated?
        public virtual bool IsComputableArea() { return false; }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override bool Equals(Object obj)
        {
            AtomicRegion thatAtom = obj as AtomicRegion;
            if (thatAtom == null) return false;

            if (this.connections.Count != thatAtom.connections.Count) return false;

            foreach (Connection conn in this.connections)
            {
                if (!thatAtom.HasConnection(conn)) return false;
            }

            return true;
        }

        public void AddConnection(Point e1, Point e2, ConnectionType t, Figure owner)
        {
            connections.Add(new Connection(e1, e2, t, owner));
        }

        public void AddConnection(Connection conn)
        {
            connections.Add(conn);
        }

        public bool HasPoint(Point that)
        {
            foreach (Connection conn in connections)
            {
                if (conn.HasPoint(that)) return true;
            }

            return false;
        }

        //  |)
        //  | )
        //  |  )
        //  | )
        //  |)
        public bool IsDefinedByChordCircle()
        {
            if (connections.Count != 2) return false;

            if (!HasSegmentConnection() || !HasArcConnection()) return false;

            return AllConnectionsHaveSameEndpoints();
        }

        //  ---\
        //   )  \
        //    )  \
        //     )  |
        //    )  /
        //   )  /
        //  ---/
        public bool IsDefinedByCircleCircle()
        {
            if (connections.Count != 2) return false;

            if (!HasSegmentConnection() && HasArcConnection()) return true;

            return AllConnectionsHaveSameEndpoints();
        }

        public bool DefinesAPolygon()
        {
            return !HasArcConnection();
        }

        private int NumSegmentConnections()
        {
            int count = 0;
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.SEGMENT) count++;
            }

            return count;
        }

        private int NumArcConnections()
        {
            int count = 0;
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.ARC) count++;
            }

            return count;
        }

        private bool HasSegmentConnection()
        {
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.SEGMENT) return true;
            }

            return false;
        }

        private bool HasArcConnection()
        {
            foreach (Connection conn in connections)
            {
                if (conn.type == ConnectionType.ARC) return true;
            }

            return false;
        }

        private bool AllConnectionsHaveSameEndpoints()
        {
            Point e1 = connections[0].endpoint1;
            Point e2 = connections[1].endpoint2;

            for (int c = 1; c < connections.Count; c++)
            {
                if (!connections[c].HasPoint(e1) || !connections[c].HasPoint(e2)) return false;
            }

            return true;
        }

        public bool HasConnection(Connection that)
        {
            foreach (Connection conn in this.connections)
            {
                if (conn.StructurallyEquals(that)) return true;
            }

            return false;
        }

        public virtual bool PointLiesOn(Point pt)
        {
            if (pt == null) return false;

            foreach (Connection conn in connections)
            {
                if (conn.PointLiesOn(pt)) return true;
            }

            return false;
        }

        public bool PointLiesOnOrInside(Point pt)
        {
            if (pt == null) return false;

            return PointLiesOn(pt) || PointLiesInside(pt);
        }

        public bool PointLiesExterior(Point pt)
        {
            if (pt == null) return false;

            return !PointLiesOnOrInside(pt);
        }

        //
        // Do all the endpoints in that region lie within this region?
        // And, are all intersection points, if any, on this perimeter?
        //
        public virtual bool Contains(AtomicRegion that)
        {
            //
            // Do all vertices of that lie on the interior of this atomic region?
            //
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesOnOrInside(vertex)) return false;
            }

            //
            // Check all midpoints of conenctions are on the interior.
            //
            foreach (Connection thatConn in that.connections)
            {
                if (!this.PointLiesOnOrInside(thatConn.Midpoint())) return false;
            }

            //
            // For any intersections between the atomic regions, the resultant points of intersection must be on the perimeter.
            //
            List<IntersectionAgg> intersections = this.GetIntersections(that);
            foreach (IntersectionAgg agg in intersections)
            {
                if (agg.overlap)
                {
                    // No-Op
                }
                else
                {
                    if (!this.PointLiesOn(agg.intersection1)) return false;
                    if (agg.intersection2 != null)
                    {
                        if (!this.PointLiesOn(agg.intersection2)) return false;
                    }
                }
            }

            return true;
        }

        //
        // Do all the endpoints in that region lie within this region?
        // There should be no intersection points.
        //
        public bool StrictlyContains(AtomicRegion that)
        {
            //
            // Do all vertices of that lie on the interior of this atomic region?
            //
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesInside(vertex)) return false;
            }

            // There should be no intersections
            return !this.GetIntersections(that).Any();
        }

        //
        // A region (that) lies inside this with one intersection.
        //
        public bool ContainsWithOneInscription(AtomicRegion that)
        {
            //
            // Do all vertices of that lie on the interior of this atomic region?
            //
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesOnOrInside(vertex)) return false;
            }

            // There should be only ONE intersection
            return this.GetIntersections(that).Count == 1;
        }

        //
        // A region (that) lies inside this with one intersection.
        //
        public bool ContainsWithGreaterOneInscription(AtomicRegion that)
        {
            //
            // Do all vertices of that lie on the interior of this atomic region?
            //
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesOnOrInside(vertex)) return false;
            }

            // There should be only ONE intersection
            return this.GetIntersections(that).Count > 1;
        }

        //
        // All vertices of that region on the perimeter of this.
        // Number of intersections must equate to the number of vertices.
        //
        public bool Inscribed(AtomicRegion that)
        {
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (!this.PointLiesOn(vertex)) return false;
            }

            return this.GetIntersections(that).Count == thatVertices.Count;
        }

        public bool Circumscribed(AtomicRegion that)
        {
            return that.Inscribed(this);
        }

        public bool HasVertexExteriorTo(AtomicRegion that)
        {
            List<IntersectionAgg> intersections = this.GetIntersections(that);

            foreach (IntersectionAgg agg in intersections)
            {
                if (!agg.overlap)
                {
                    if (!this.PointLiesOnOrInside(agg.intersection1)) return true;
                }
            }

            return false;
        }

        //
        // If there is no interaction between these atomic regions OR just touching
        //
        public bool OnExteriorOf(AtomicRegion that)
        {
            List<IntersectionAgg> intersections = this.GetIntersections(that);

            // All vertices cannot be interior to the region.
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (this.PointLiesInside(vertex)) return false;
            }

            // All intersections must be overlap; only point-based intersections which are on the perimeter.
            foreach (IntersectionAgg agg in intersections)
            {
                if (!agg.overlap)
                {
                    if (agg.intersection2 != null) return false;
                    if (!this.PointLiesOn(agg.intersection1)) return false;
                }
                else // agg.overlap
                {
                    // No-Op
                }
            }

            return true;
        }

        //
        // If there is no interaction between these atomic regions OR just touching
        //
        public bool InteriorOfWithTouching(AtomicRegion that)
        {
            List<IntersectionAgg> intersections = this.GetIntersections(that);

            // All vertices cannot be interior to the region.
            List<Point> thatVertices = that.GetVertices();
            foreach (Point vertex in thatVertices)
            {
                if (this.PointLiesOnOrInside(vertex)) return false;
            }

            // All intersections must overlap; only point-based intersections which are on the perimeter.
            foreach (IntersectionAgg agg in intersections)
            {
                if (agg.overlap)
                {
                    // No-Op
                }
                else
                {
                    if (PointLiesExterior(agg.intersection1)) return false;
                    if (agg.intersection2 != null)
                    {
                        if (PointLiesExterior(agg.intersection2)) return false;
                    }
                }
            }

            return true;
        }

        //
        // Imagine 2 equilateral triangles to make the Star of David: all 
        //
        public bool OverlapWithSingleConnections(AtomicRegion that)
        {
            List<IntersectionAgg> intersections = this.GetIntersections(that);

            foreach (IntersectionAgg agg in intersections)
            {
                if (agg.overlap)
                {
                    // No-Op
                }
                else
                {
                    if (agg.intersection1 == null || agg.intersection2 == null) return false;
                }
            }

            return true;
        }

        //
        // Does this region overlap the other based on points (not intersections).
        //
        private bool Overlap(List<Point> points)
        {
            bool interiorPoint = false;
            bool exteriorPoint = false;
            foreach (Point pt in points)
            {
                if (this.PointLiesInside(pt)) interiorPoint = true;
                else if (!this.PointLiesOn(pt)) exteriorPoint = true;
            }
            return interiorPoint && exteriorPoint;
        }

        public bool OverlapsWith(AtomicRegion that)
        {
            // Point based overlapping.
            if (Overlap(that.GetApproximatingPoints())) return true;

            // Crossing-based overlap.
            List<IntersectionAgg> intersections = this.GetIntersections(that);
            foreach (IntersectionAgg agg in intersections)
            {
                if (agg.thisConn.Crosses(agg.thatConn)) return true;
            }
            
            return false;
        }

        //
        // Takes a non-shape atomic region and turns it into an approximate polygon,
        // by converting all arcs into approximated arcs using many line segments.
        //
        public virtual Polygon GetPolygonalized()
        {
            if (polygonalized != null) return polygonalized;

            List<Segment> sides = new List<Segment>();
            foreach (Connection conn in connections)
            {
                sides.AddRange(conn.Segmentize());
            }

            polygonalized = Polygon.MakePolygon(sides);

            return polygonalized;
        }

        //
        // Does the given connection pass through the atomic region? Or is, it completely outside of the region?
        //
        public bool NotInteriorTo(Connection that)
        {
            if (this.PointLiesInside(that.endpoint1)) return false;
            if (this.PointLiesInside(that.endpoint2)) return false;

            int standOnCount = 0;
            foreach (Connection thisConn in this.connections)
            {
                if (thisConn.Crosses(that)) return false;
                if (thisConn.StandsOnNotEndpoint(that)) standOnCount++;
            }

            return standOnCount <= 1;
        }

        //
        // Acquire the two connections that have the given point.
        //
        private void GetConnections(Point pt, out Connection conn1, out Connection conn2)
        {
            List<Connection> conns = new List<Connection>();

            foreach (Connection conn in connections)
            {
                if (conn.HasPoint(pt)) conns.Add(conn);
            }

            conn1 = conns[0];
            conn2 = conns[1];
        }

        //
        // Order the connections so that one endpoint leads to the next (sequentially).
        //
        public void OrderConnections()
        {
            if (!connections.Any()) return;

            List<Connection> orderedConns = new List<Connection>();

            // Add the arbitrary first connection and choose a point to follow 
            orderedConns.Add(connections[0]);
            Point currPoint = connections[0].endpoint1;
            Connection currConn = connections[0];

            for (int i = 1; i < connections.Count; i++)
            {
                Connection c1 = null;
                Connection c2 = null;
                GetConnections(currPoint, out c1, out c2);
                if (currConn.Equals(c1))
                {
                    orderedConns.Add(c2);
                    currConn = c2;
                    currPoint = c2.OtherEndpoint(currPoint);
                }
                else if (currConn.Equals(c2))
                {
                    orderedConns.Add(c1);
                    currConn = c1;
                    currPoint = c1.OtherEndpoint(currPoint);
                }
            }

            // Update the actual connections with the ordered set.
            connections = orderedConns;
            ordered = true;
        }

        //
        // Acquire the set of point making up the vertices of this region.
        //
        public List<Point> GetVertices()
        {
            // Ensure ordered connections for simple acquisition.
            if (!ordered) OrderConnections();

            List<Point> vertices = new List<Point>();

            Point currPoint = connections[0].endpoint1;
            vertices.Add(currPoint);

            for (int i = 1; i < connections.Count; i++)
            {
                currPoint = connections[i].OtherEndpoint(currPoint);
                vertices.Add(currPoint);
            }

            return vertices;
        }

        public class IntersectionAgg
        {
            public Connection thisConn;
            public Connection thatConn;
            public Point intersection1;
            public Point intersection2;
            public bool overlap;

            public bool MixedTypes() { return thisConn.type != thatConn.type; }
        }

        ////
        //// Determine the intersection points / connections between this atomic region and a segment.
        ////
        //public List<IntersectionAgg> GetIntersections(Segment that)
        //{
        //    List<IntersectionAgg> intersections = new List<IntersectionAgg>();
        //    Point pt1 = null;
        //    Point pt2 = null;

        //    foreach (Connection conn in connections)
        //    {
        //        conn.FindIntersection(that, out pt1, out pt2);
        //        AddIntersection(intersections, pt1, conn);
        //        AddIntersection(intersections, pt2, conn);
        //    }

        //    return intersections;
        //}

        ////
        //// Determine the intersection points / connections between this atomic region and a connection.
        ////
        //private List<IntersectionAgg> GetIntersections(Connection that)
        //{
        //    if (that.type == ConnectionType.SEGMENT) return GetIntersections(that.segmentOwner as Segment);

        //    return new List<IntersectionAgg>();

        //    //return GetIntersections(that.segmentOwner as Circle, that.endpoint1, that.endpoint2);
        //}

        //
        // Get the index of the intersection with the same point of intersection.
        //
        private int IntersectionIndex(List<IntersectionAgg> intersections, Point pt)
        {
            if (pt == null) return -1;

            for (int a = 0; a < intersections.Count; a++)
            {
                if (pt.StructurallyEquals(intersections[a].intersection1)) return a;
                if (pt.StructurallyEquals(intersections[a].intersection2)) return a;
            }

            return -1;
        }

        //
        // Add to the list of intersections only if the intersection point is not already in the list (avoids duplicates due to endpoints).
        // We have as many as 4 intersections since a segment may intersect the endpoint of twe connections.
        // This prevents against 4 intersections, ensure 2 intersections only.
        //
        private void AddIntersection(List<IntersectionAgg> intersections, IntersectionAgg newAgg)
        {
            //
            // Favor an intersection that intersects twice over an intersection that intersects once.
            //
            int index1 = IntersectionIndex(intersections, newAgg.intersection1);
            int index2 = IntersectionIndex(intersections, newAgg.intersection2);

            // Not found, so add this new intersection.
            if (index1 == -1 && index2 == -1)
            {
                intersections.Add(newAgg);
                return;
            }

            IntersectionAgg agg1 = index1 != -1 ? intersections[index1] : null;
            IntersectionAgg agg2 = index2 != -1 ? intersections[index2] : null;

            // This, by default, means newAgg has 2 intersections.
            if (index1 != -1 && index2 != -1)
            {
                intersections.Remove(agg1);
                intersections.Remove(agg2);
                intersections.Add(newAgg);
                return;
            }

            // Favor two intersections, if applicable.
            if (index1 != -1 && index2 == -1)
            {
                // Two intersections favored.
                if (newAgg.intersection1 != null && newAgg.intersection2 != null)
                {
                    intersections.Remove(agg1);
                    intersections.Add(newAgg);
                }
                else
                {
                    // We already have 1 intersection.
                }
                return;
            }

            // Two intersections implied.
            if (index1 == -1 && index2 != -1)
            {
                intersections.Remove(agg2);
                intersections.Add(newAgg);
                return;
            }
        }



        //
        // Determine the intersection points / connections between this atomic region and another region.
        //
        public List<IntersectionAgg> GetIntersections(List<Point> figurePoints, AtomicRegion thatAtom)
        {
            List<IntersectionAgg> intersections = new List<IntersectionAgg>();

            foreach (Connection thisConn in this.connections)
            {
                foreach (Connection thatConn in thatAtom.connections)
                {
                    Point inter1 = null;
                    Point inter2 = null;
                    thisConn.FindIntersection(figurePoints, thatConn, out inter1, out inter2);

                    if (inter1 != null)
                    {
                        IntersectionAgg newAgg = new IntersectionAgg();
                        newAgg.thisConn = thisConn;
                        newAgg.thatConn = thatConn;
                        newAgg.intersection1 = inter1;
                        newAgg.intersection2 = inter2;
                        newAgg.overlap = thisConn.Overlap(thatConn);
                        AddIntersection(intersections, newAgg);
                    }
                    else if (thisConn.Overlap(thatConn))
                    {
                        IntersectionAgg newAgg = new IntersectionAgg();
                        newAgg.thisConn = thisConn;
                        newAgg.thatConn = thatConn;
                        newAgg.intersection1 = null;
                        newAgg.intersection2 = null;
                        newAgg.overlap = true;
                        AddIntersection(intersections, newAgg);
                    }
                }
            }

            return intersections;
        }

        private List<IntersectionAgg> GetIntersections(AtomicRegion thatAtom)
        {
            List<IntersectionAgg> intersections = new List<IntersectionAgg>();

            foreach (Connection thisConn in this.connections)
            {
                foreach (Connection thatConn in thatAtom.connections)
                {
                    Point inter1 = null;
                    Point inter2 = null;
                    thisConn.FindIntersection(thatConn, out inter1, out inter2);

                    if (thisConn.Overlap(thatConn))
                    {
                        IntersectionAgg newAgg = new IntersectionAgg();
                        newAgg.thisConn = thisConn;
                        newAgg.thatConn = thatConn;
                        newAgg.intersection1 = null;
                        newAgg.intersection2 = null;
                        newAgg.overlap = true;
                        AddIntersection(intersections, newAgg);
                    }
                    else if (inter1 != null)
                    {
                        IntersectionAgg newAgg = new IntersectionAgg();
                        newAgg.thisConn = thisConn;
                        newAgg.thatConn = thatConn;
                        newAgg.intersection1 = inter1;
                        newAgg.intersection2 = inter2;
                        newAgg.overlap = thisConn.Overlap(thatConn);
                        AddIntersection(intersections, newAgg);
                    }

                }
            }

            return intersections;
        }

        ////
        //// Determine the number of intersection points between this atomic region and a segment.
        ////
        //public List<KeyValuePair<Point, Connection>> GetIntersections(Circle circle, Point endpt1, Point endp12)
        //{
        //    List<KeyValuePair<Point, Connection>> intersections = new List<KeyValuePair<Point, Connection>>();
        //    Point pt1 = null;
        //    Point pt2 = null;

        //    foreach (Connection conn in connections)
        //    {
        //        conn.FindIntersection(that, out pt1, out pt2);
        //        if (pt1 != null) intersections.Add(new KeyValuePair<Point, Connection>(pt1, conn));
        //        if (pt2 != null) intersections.Add(new KeyValuePair<Point, Connection>(pt2, conn));

        //        // A segment should only intersect an atomic region in 2 places.
        //        if (intersections.Count >= 2) break;
        //    }

        //    return intersections;
        //}

        //
        // Convert an atomic region to a planar graph.
        //
        public UndirectedPlanarGraph.PlanarGraph ConvertToPlanarGraph()
        {
            UndirectedPlanarGraph.PlanarGraph graph = new UndirectedPlanarGraph.PlanarGraph();

            if (!ordered) OrderConnections();

            //
            // Traverse the connections and add vertices / edges 
            //
            foreach (Connection conn in connections)
            {
                graph.AddNode(conn.endpoint1);
                graph.AddNode(conn.endpoint2);

                graph.AddUndirectedEdge(conn.endpoint1, conn.endpoint2,
                                        new Segment(conn.endpoint1, conn.endpoint2).Length,
                                        conn.type == ConnectionType.ARC ? UndirectedPlanarGraph.EdgeType.REAL_ARC : UndirectedPlanarGraph.EdgeType.REAL_SEGMENT);
            }

            return graph;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("Atom: {");

            for (int c = 0; c < connections.Count; c++)
            {
                str.Append(connections[c].ToString());
                if (c < connections.Count - 1) str.Append(", ");
            }

            str.Append(" }");

            return str.ToString();
        }

        public virtual string CheapPrettyString()
        {
            StringBuilder str = new StringBuilder();

            str.Append("{");

            for (int c = 0; c < connections.Count; c++)
            {
                str.Append(connections[c].CheapPrettyString());
                if (c < connections.Count - 1) str.Append(", ");
            }

            str.Append("}");

            return str.ToString();
        }
    }
}
