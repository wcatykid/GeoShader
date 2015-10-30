using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using LiveGeometry;
using GeometryTutorLib.ConcreteAST;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DirectComponentsFromUI
    {
        // The list of Live Geometry figures.
        List<IFigure> ifigs;
        Drawing drawing;

        // A database to check if we have already created a particular component of the figure.
        private Dictionary<IFigure, GroundedClause> uiToEngineMap;

        // The explicitly UI defined set of components.
        public List<Point> definedPoints { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Segment> definedSegments { get; private set; }
        public List<GeometryTutorLib.ConcreteAST.Circle> circles { get; private set; }

        public List<GeometryTutorLib.ConcreteAST.Polygon>[] polygons { get; private set; }

        /// <summary>
        /// Construct the parser that converts from the UI representation to the tutor back-end representation.
        /// </summary>
        /// <param name="figure">The figure to parse.</param>
        public DirectComponentsFromUI(Drawing d, List<IFigure> figs)
        {
            ifigs = figs;
            drawing = d;

            uiToEngineMap = new Dictionary<IFigure, GroundedClause>();
            definedPoints = new List<Point>();
            definedSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            circles = new List<GeometryTutorLib.ConcreteAST.Circle>();

            polygons = GeometryTutorLib.ConcreteAST.Polygon.ConstructPolygonContainer();
        }

        /// <summary>
        /// Parse the given figure.
        /// </summary>
        /// <param name="figure">The figure to parse.</param>
        public void Parse()
        {
            // Parse the figure.
            foreach (IFigure ifig in ifigs)
            {
                Parse(ifig);
            }

            // Remove the duplicate segments (if any).
            definedSegments = GeometryTutorLib.Utilities.RemoveDuplicates<GeometryTutorLib.ConcreteAST.Segment>(definedSegments);
        }

        /// <summary>
        /// Parse a component of the given figure.
        /// </summary>
        /// <param name="figure">The figure to parse.</param>
        private void Parse(IFigure figure)
        {
            if (uiToEngineMap.ContainsKey(figure)) return;
            else if (figure is IPoint) ParsePoint(figure as IPoint);
            else if (figure is ILine) ParseLine(figure as ILine);
            else if (figure is DynamicGeometry.Polygon) ParsePolygon(figure as PolygonBase);
            else if (figure is RegularPolygon) ParseRegularPolygon(figure as RegularPolygon);
            else if (figure is CircleBase) ParseCircle(figure as CircleBase);
        }


        /// <summary>
        /// Parse a point.
        /// </summary>
        /// <param name="pt">The point to parse.</param>
        private void ParsePoint(IPoint uiPt)
        {
            Point tutorPt = new Point(uiPt.Name, uiPt.Coordinates.X, uiPt.Coordinates.Y);

            definedPoints.Add(tutorPt);

            uiToEngineMap.Add(uiPt, tutorPt);
        }

        /// <summary>
        /// Parse a line.
        /// </summary>
        /// <param name="line">The line to parse.</param>
        private void ParseLine(ILine uiLine)
        {
            //
            // Parse the constituent points.
            //
            IPoint p1 = uiLine.Dependencies.FindPoint(uiLine.Coordinates.P1, 0);
            IPoint p2 = uiLine.Dependencies.FindPoint(uiLine.Coordinates.P2, 0);
            Parse(p1);
            Parse(p2);

            // Create the tutor segments
            GeometryTutorLib.ConcreteAST.Segment tutorSeg = new GeometryTutorLib.ConcreteAST.Segment(uiToEngineMap[p1] as Point, uiToEngineMap[p2] as Point);

            // TempSegs.Add(new TempSegment(s.Point1, s.Point2));

            uiToEngineMap.Add(uiLine, tutorSeg);

            definedSegments.Add(tutorSeg);
        }

        /// <summary>
        /// Parse a polygon. (Currently only triangles)
        /// </summary>
        /// <param name="pgon">The polygon to parse.</param>
        private void ParsePolygon(PolygonBase pgon)
        {
            //
            // Handle an n-sided polygon
            //
            int numSides = pgon.VertexCoordinates.Length;

            //
            // Find verticies
            //
            System.Windows.Point[] pts = pgon.VertexCoordinates;
            IPoint[] iPts = new IPoint[numSides];

            //
            // Parse all points
            //
            for (int i = 0; i < numSides; i++)
            {
                iPts[i] = pgon.Dependencies.FindPoint(pts[i], 0);
                Parse(iPts[i] as IFigure);
            }

            //
            // Generate sides
            //
            List<GeometryTutorLib.ConcreteAST.Segment> tutorSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            for (int i = 0; i < numSides; i++)
            {
                int j = (i + 1) % numSides;
                tutorSegments.Add(new GeometryTutorLib.ConcreteAST.Segment(uiToEngineMap[iPts[i]] as GeometryTutorLib.ConcreteAST.Point,
                                                                           uiToEngineMap[iPts[j]] as GeometryTutorLib.ConcreteAST.Point));
            }
            definedSegments.AddRange(tutorSegments);

            GeometryTutorLib.ConcreteAST.Polygon newPoly = null;
            switch(numSides)
            {
                case 3:
                    newPoly = new Triangle(tutorSegments);
                    polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX].Add(newPoly);

                    break;
                case 4:
                    newPoly = Quadrilateral.GenerateQuadrilateral(tutorSegments);
                    if (newPoly != null) polygons[GeometryTutorLib.ConcreteAST.Polygon.QUADRILATERAL_INDEX].Add(newPoly);
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    break;

                default:
                    break;
            }
            
            if (newPoly != null) uiToEngineMap.Add(pgon, newPoly);
        }

        /// <summary>
        /// Parse a regular polygon. (Currently only equilateral triangles)
        /// </summary>
        /// <param name="rgon">The regular polygon to parse.</param>
        private void ParseRegularPolygon(RegularPolygon rgon)
        {
            // Acquire the implicit center of the regular polygon
            IPoint center = rgon.Dependencies.FindPoint(rgon.Center, 0);
            IPoint vertex = rgon.Dependencies.FindPoint(rgon.Vertex, 0);

            int numSides = rgon.NumberOfSides;
            //
            // Genereate vertex points knowing that the polygon is regular
            //
            IPoint[] pts = new IPoint[numSides];

            double radius = Math.Distance(vertex.Coordinates.X, center.Coordinates.X, vertex.Coordinates.Y, center.Coordinates.Y);
            double initAngle = Math.GetAngle(new System.Windows.Point(center.Coordinates.X, center.Coordinates.Y),
                                             new System.Windows.Point(vertex.Coordinates.X, vertex.Coordinates.Y));
            double increment = Math.DOUBLEPI / numSides;
            // Vertex point generation and parsing.
            for (int i = 0; i < numSides - 1; i++)
            {
                double angle = initAngle + (i + 1) * increment;
                double X = center.Coordinates.X + radius * System.Math.Cos(angle);
                double Y = center.Coordinates.Y + radius * System.Math.Sin(angle);
                System.Windows.Point newPt = new System.Windows.Point(X, Y);
                pts[i] = rgon.Dependencies.FindPoint(newPt, 0);
                if (pts[i] == null) pts[i] = Factory.CreateFreePoint(drawing, newPt);
                Parse(pts[i] as IFigure);
            }
            pts[numSides - 1] = vertex;
            Parse(pts[numSides - 1] as IFigure);

            //
            // Generate sides from vertices
            //
            List<GeometryTutorLib.ConcreteAST.Segment> tutorSegments = new List<GeometryTutorLib.ConcreteAST.Segment>();
            for (int i = 0; i < numSides; i++)
            {
                int j = (i + 1) % 3;
                tutorSegments.Add(new GeometryTutorLib.ConcreteAST.Segment(uiToEngineMap[pts[i]] as Point, uiToEngineMap[pts[j]] as Point));
            }
            definedSegments.AddRange(tutorSegments);

            GeometryTutorLib.ConcreteAST.Polygon newPoly = null;
            switch(numSides)
            {
                case 3:
                    newPoly = new EquilateralTriangle(tutorSegments);
                    polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX].Add(newPoly);

                    break;
                case 4:
                    Quadrilateral newQuad = Quadrilateral.GenerateQuadrilateral(tutorSegments);
                    if (newQuad != null)
                    {
                        newPoly = new Rhombus(newQuad);
                        polygons[GeometryTutorLib.ConcreteAST.Polygon.QUADRILATERAL_INDEX].Add(newPoly);
                    }
                    break;
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    break;

                default:
                    break;
            }
            
            if (newPoly != null) uiToEngineMap.Add(rgon, newPoly);
        }

        /// <summary>
        /// Parse a CircleBase.
        /// </summary>
        /// <param name="c"> The circle to parse.</param>
        private void ParseCircle(CircleBase cb)
        {
            // Acquire the defining characteristics from the UI
            IPoint center = cb.Dependencies.FindPoint(cb.Center, 0);
            double radius = cb.Radius;

            // Parse the center of the circle.
            Parse(center as IFigure);

            // Create the Tutor version of the point
            GeometryTutorLib.ConcreteAST.Circle tutorCircle = new GeometryTutorLib.ConcreteAST.Circle(uiToEngineMap[center] as Point, radius);

            // Add to the list of known circles from the UI
            circles.Add(tutorCircle);

            uiToEngineMap.Add(cb, tutorCircle);
        }



        public override string ToString()
        {
            System.Text.StringBuilder str = new System.Text.StringBuilder();

            str.AppendLine("UI Points");
            foreach (Point p in definedPoints)
            {
                str.AppendLine("\t" + p.ToString());
            }

            str.AppendLine("UI Segments");
            foreach (GeometryTutorLib.ConcreteAST.Segment s in definedSegments)
            {
                str.AppendLine("\t" + s.ToString());
            }

            str.AppendLine("UI Polygons");
            for (int n = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX; n < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX; n++)
            {
                foreach (GeometryTutorLib.ConcreteAST.Polygon poly in polygons[n])
                {
                    str.AppendLine("\t" + poly.ToString());
                }
            }

            str.AppendLine("UI Circles");
            foreach (GeometryTutorLib.ConcreteAST.Circle c in circles)
            {
                str.AppendLine("\t" + c.ToString());
            }

            return str.ToString();
        }
    }
}