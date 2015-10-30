using GeometryTutorLib.ConcreteAST.Figures;

namespace GeometryTutorLib.ConcreteAST.Desciptors
{
    public class CircleIntersection : Descriptor
    {
        public Point intersect { get; private set; }
        public Circle circle1 { get; private set; }
        public Circle circle2 { get; private set; }

        /// <summary>
        /// Create a new intersection between two circles.
        /// </summary>
        /// <param name="i">The point of intersection.</param>
        /// <param name="c1">A circle.</param>
        /// <param name="c2">An intersecting circle to c1.</param>
        public CircleIntersection(Point i, Circle c1, Circle c2)
        {
            intersect = i;
            circle1 = c1;
            circle2 = c2;
        }

        /// <summary>
        /// Tests if the two intersections are structurally equal.
        /// </summary>
        /// <param name="obj">The other object to test.</param>
        /// <returns>True if the parameter is structurally equal to this intersection.</returns>
        public override bool StructurallyEquals(object obj)
        {
            CircleIntersection ci = obj as CircleIntersection;
            if (ci == null) return false;

            return intersect.StructurallyEquals(ci.intersect) &&
                ((circle1.StructurallyEquals(ci.circle1) && circle2.StructurallyEquals(ci.circle2)) ||
                (circle1.StructurallyEquals(ci.circle2) && circle2.StructurallyEquals(ci.circle1)));
        }

        public override string ToString()
        {
            return "CircleIntersection(" + intersect.ToString() + ", " + circle1.ToString() + ", " + circle2.ToString() + ") " + justification;
        }
    }
}
