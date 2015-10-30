using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Circle : Figure
    {
        public override double CoordinatizedArea() { return radius * radius * Math.PI; }

        public override bool CoordinateCongruent(Figure that)
        {
            Circle thatCirc = that as Circle;
            if (thatCirc == null) return false;

            return Utilities.CompareValues(this.radius, thatCirc.radius);
        }

        //
        // Outer minus circle; the circle will only be situation if it touches all sides of the outer figure.
        //
        public static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            List<FigSynthProblem> composed = new List<FigSynthProblem>();
            List<Midpoint> midpoints = outerShape.GetMidpointClauses();

            if (midpoints.Count < 3) return composed;

            //
            // Construct the circle based on the first 3 points.
            //
            Circle circ = Circle.ConstructCircle(midpoints[0].point, midpoints[1].point, midpoints[2].point);

            //
            // Verify that the remaining points lie on the circle.
            //
            for (int p = 4; p < midpoints.Count; p++)
            {
                if (!circ.PointLiesOn(midpoints[p].point)) return composed;
            }

            SubtractionSynth subSynth = new SubtractionSynth(outerShape, circ);

            composed.Add(subSynth);

            return composed;
        }

        public static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            throw new NotImplementedException();
        }

        public static Circle ConstructDefaultCircle()
        {
            int radius = Figure.DefaultSideLength() / 2;

            return new Circle(origin, radius);
        }

        //
        // Construct and return all constraints.
        //
        public override List<Constraint> GetConstraints()
        {
            List<Equation> eqs = new List<Equation>();
            //List<Congruent> congs = new List<Congruent>();

            ////
            //// Acquire the 'midpoint' equations from the polygon class.
            ////
            //GetGranularMidpointEquations(out eqs, out congs);

            ////
            //// Create all relationship equations among the sides of the square.
            ////
            //for (int s1 = 0; s1 < orderedSides.Count - 1; s1++)
            //{
            //    for (int s2 = s1 + 1; s2 < orderedSides.Count; s2++)
            //    {
            //        congs.Add(new GeometricCongruentSegments(orderedSides[s1], orderedSides[s2]));
            //    }
            //}

            ////
            //// Create all relationship equations among the angles.
            ////
            //// Make simple equations where the angles are 90 degrees?
            ////
            //for (int a1 = 0; a1 < angles.Count - 1; a1++)
            //{
            //    for (int a2 = a1 + 1; a2 < angles.Count; a2++)
            //    {
            //        eqs.Add(new GeometricAngleEquation(angles[a1], angles[a2]));
            //    }
            //}

            List<Constraint> constraints = Constraint.MakeEquationsIntoConstraints(eqs);
            //constraints.AddRange(Constraint.MakeCongruencesIntoConstraints(congs));

            return constraints;
        }


        //
        // Specify how many points are required to atomize each circle.
        //
        public static Dictionary<Circle, int> AcquireCircleGranularity(List<Circle> circles)
        {
            Dictionary<Circle, int> granularity = new Dictionary<Circle, int>();

            if (!circles.Any()) return granularity;

            circles = new List<Circle>(circles.OrderBy(c => c.radius));

            // Construct the granularity for when we construct arcs.
            double currRadius = circles[0].radius;
            int gran = 1;

            foreach (Circle circle in circles)
            {
                if (circle.radius > currRadius) gran++;
                granularity[circle] = gran;
            }

            return granularity;
        }

        //
        // Get the actual area formula for the given figure.
        //
        public override List<Segment> GetAreaVariables() { throw new NotImplementedException(); }
        private bool SIXTEEN_PT_UNIT_CIRCLE = true;
        private bool CARDINAL_PTS = false;

        //
        // Constructor for figure synthesis: constructs the unit circle.
        //
        protected void FigureSynthesizerConstructor()
        {
#if !FIGURE_SYNTHESIZER
            return;
#else
            //
            // Construct the set of all snapping points
            //
            allComposingPoints = new List<Point>();

            ConstructOriginSnapPoint();
            if (SIXTEEN_PT_UNIT_CIRCLE) Construct16PointSnapPoints();
            else if (CARDINAL_PTS) ConstructCardinalDirections();
#endif
        }


        //
        // Add the center as a snapping point.
        //
        private void ConstructOriginSnapPoint()
        {
            allComposingPoints.Add(this.center);
        }

        private int[] ANGLE_MEASURES = { 0, 30, 45, 60, 90, 120, 135, 150, 180, 210, 225, 240, 270, 300, 315, 330 };

        //
        // Add the 16-point unit circle to the set of snapping points
        //
        private void Construct16PointSnapPoints()
        {
            // Add all points to the unit circle
            foreach (int angle in ANGLE_MEASURES)
            {
                double x = this.radius * Math.Cos(Angle.toRadians(angle));
                double y = this.radius * Math.Sin(Angle.toRadians(angle));

                allComposingPoints.Add(new Point(angle.ToString() + "^o", x, y));
            }
        }

        //
        // Add the 16-point unit circle to the set of snapping points
        //
        private void ConstructCardinalDirections()
        {
            // Add all points to the unit circle
            foreach (int angle in ANGLE_MEASURES)
            {
                if (angle % 90 == 0)
                {
                    double x = this.radius * Math.Cos(Angle.toRadians(angle));
                    double y = this.radius * Math.Sin(Angle.toRadians(angle));

                    allComposingPoints.Add(new Point(angle.ToString() + "^o", x, y));
                }
            }
        }

        public static Circle ConstructCircle(Point p1, Point p2, Point p3)
        {
            //
            // Find the center of the circle.
            //
            Segment chord1 = new Segment(p1, p2);
            Segment chord2 = new Segment(p2, p3);

            Segment perpBis1 = chord1.GetPerpendicular(chord1.Midpoint());
            Segment perpBis2 = chord2.GetPerpendicular(chord2.Midpoint());

            Point center = perpBis1.FindIntersection(perpBis2);

            //
            // Radius is the distance between the circle and any of the original points.
            //
            return new Circle(center, Point.calcDistance(center, p1));
        }
    }
}