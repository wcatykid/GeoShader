using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class Collinear : Descriptor
    {
        public List<Point> points { get; private set; }

        //
        // We assume the points are ordered how they appear
        // But we verify just in case
        public Collinear(List<Point> pts) : base()
        {
            points = new List<Point>(pts);

            Verify();
        }

        //
        // We assume the points are ordered how they appear
        // But we verify just in case
        public Collinear() : base()
        {
            points = new List<Point>();
        }

        private void Verify()
        {
            if (points.Count < 2) throw new ArgumentException("A Collinear relationship requires at least 2 points: " + this.ToString());

            // Create a segment of the endpoints to compare all points for collinearity
            Segment line = new Segment(points[0], points[points.Count - 1]);

            foreach (Point pt in points)
            {
                if (!line.PointLiesOn(pt))
                {
                    throw new ArgumentException("Point " + pt + " is not collinear with line " + line.ToString());
                }

                if (!line.PointLiesOnAndBetweenEndpoints(pt))
                {
                    throw new ArgumentException("Point " + pt + " is not between the endpoints of segment " + line.ToString());
                }
            }
        }

        public void AddCollinearPoint(Point newPt)
        {
            // Traverse list to find where to insert the new point in the list in the proper order
            for (int p = 0; p < points.Count - 1; p++)
            {
                if (Segment.Between(newPt, points[p], points[p + 1]))
                {
                    points.Insert(p + 1, newPt);
                    return;
                }
            }
            points.Add(newPt);
        }

        public override bool Equals(Object obj)
        {
            Collinear collObj = obj as Collinear;
            if (collObj == null) return false;

            // Check all points
            foreach (Point pt in collObj.points)
            {
                if (!points.Contains(pt)) return false;
            }
            return true;
        }

        public override int GetHashCode() { return base.GetHashCode(); }

        public override string ToString()
        {
            List<String> strings = new List<String>();
            foreach (Point p in points) strings.Add(p.ToString());
            return "Collinear(" + string.Join(",", strings.ToArray()) + ")";
        }
    }
}
