using System.Collections.Generic;

namespace GeometryTutorLib.EngineUIBridge
{
    /// <summary>
    /// Represents a relationship that can be a given or goal problem characteristic
    /// </summary>
    public class Relationship
    {
        public static Relationship CongruentTriangles = new Relationship("Congruent Triangles");
        public static Relationship SimilarTriangles = new Relationship("Similar Triangles");
        public static Relationship SegmentRatio = new Relationship("Proportional Segments");
        public static Relationship CongruentAngles = new Relationship("Congruent Angles");
        public static Relationship CongruentSegments = new Relationship("Congruent Segments");
        public static Relationship Midpoint = new Relationship("Midpoint");
        public static Relationship IsoscelesTriangle = new Relationship("Isosceles Triangle");
        public static Relationship EquilateralTriangle = new Relationship("Equilateral Triangle");
        public static Relationship RightTriangle = new Relationship("Right Triangle");
        public static Relationship Parallel = new Relationship("Parallel");
        public static Relationship Perpendicular = new Relationship("Perpendicular");

        public string Name { get; private set; }
        public bool isGiven { get; set; }
        public bool isGoal { get; set; }

        /// <summary>
        /// Create a new relationship
        /// </summary>
        /// <param name="name">The string representation of the relationship</param>
        private Relationship(string name)
        {
            this.Name = name;
            this.isGiven = false;
            this.isGoal = false;
        }

        /// <summary>
        /// Get a list of all defined relationships.
        /// </summary>
        /// <returns>A list of all defined relationships.</returns>
        public static List<Relationship> GetRelationships()
        {
            List<Relationship> rv = new List<Relationship>();
            rv.Add(CongruentTriangles);
            rv.Add(SimilarTriangles);
            rv.Add(SegmentRatio);
            rv.Add(CongruentAngles);
            rv.Add(CongruentSegments);
            rv.Add(Midpoint);
            rv.Add(IsoscelesTriangle);
            rv.Add(EquilateralTriangle);
            rv.Add(RightTriangle);
            rv.Add(Parallel);
            rv.Add(Perpendicular);
            return rv;
        }

        public override string ToString()
        {
            return Name + ": Given=" + isGiven + ", Goal=" + isGoal;
        }
    }
}
