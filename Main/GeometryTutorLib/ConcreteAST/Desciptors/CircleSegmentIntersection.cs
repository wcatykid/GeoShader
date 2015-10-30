using GeometryTutorLib.ConcreteAST.Figures;

namespace GeometryTutorLib.ConcreteAST.Desciptors
{
    public class CircleSegmentIntersection : Descriptor
    {
        public Point intersect { get; private set; }
        public Circle circle { get; private set; }
        public Segment segment { get; private set; }

        /// <summary>
        /// Create a new intersection between a segment and a circle.
        /// </summary>
        /// <param name="i">The point of intersection.</param>
        /// <param name="c">The circle.</param>
        /// <param name="s">The segment.</param>
        public CircleSegmentIntersection(Point i, Circle c, Segment s)
            : base()
        {
            intersect = i;
            circle = c;
            segment = s;
        }

        /// <summary>
        /// Tests if the two intersections are structurally equal.
        /// </summary>
        /// <param name="obj">The other object to test.</param>
        /// <returns>True if the parameter is structurally equal to this intersection.</returns>
        public override bool StructurallyEquals(object obj)
        {
            CircleSegmentIntersection csi = obj as CircleSegmentIntersection;
            if (csi == null) return false;

            return intersect.StructurallyEquals(csi.intersect) &&
                circle.StructurallyEquals(csi.circle) &&
                segment.StructurallyEquals(csi.segment);
        }

        public override string ToString()
        {
            return "CircleSegmentIntersection(" + intersect.ToString() + ", " + circle.ToString() + ", " + segment.ToString() + ") " + justification;
        }
    }
}
