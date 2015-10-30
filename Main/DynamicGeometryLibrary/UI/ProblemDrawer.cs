
using DynamicGeometry.UI.RegionShading;
using GeometryTutorLib.GeometryTestbed;
using System.Collections.Generic;
using System.Diagnostics;
using GeometryTutorLib;
using System.Windows.Threading;
namespace DynamicGeometry.UI
{
    /// <summary>
    /// Draws hard-coded problems to the canvas.
    /// Provides an implementation for UIProblemDrawer. No instance of this class can be publically obtained.
    /// </summary>
    public class ProblemDrawer
    {
        private static ProblemDrawer instance = null;

        private DrawingHost drawingHost;

        private Dictionary<GeometryTutorLib.ConcreteAST.Point, IPoint> points; //Keep track of logical to graphical points
        private Dictionary<GeometryTutorLib.ConcreteAST.Segment, Segment> segments; //Keep track of logical to graphical segments
        private Dictionary<GeometryTutorLib.ConcreteAST.Circle, Circle> circles; //Keep track of logical to graphical circles

        /// <summary>
        /// Create a new problem drawer.
        /// </summary>
        /// <param name="drawing">The drawingHost.</param>
        private ProblemDrawer(DrawingHost drawingHost)
        {
            this.drawingHost = drawingHost;
        }

        public static void create(DrawingHost drawingHost)
        {
            Debug.Assert(instance == null, "create() should only be called once.");
            instance = new ProblemDrawer(drawingHost);
            UIProblemDrawer.create(instance.invokeDraw, instance.invokeClear, instance.invokeReset);
        }

        /// <summary>
        /// Async invocation of draw on the UI thread.
        /// </summary>
        /// <param name="desc">The problem to draw.</param>
        public void invokeDraw(UIProblemDrawer.ProblemDescription desc)
        {
            SmartDispatcher.BeginInvoke(() => draw(desc));
        }

        /// <summary>
        /// Async invocation of clear on the UI thread.
        /// </summary>
        public void invokeClear()
        {
            SmartDispatcher.BeginInvoke(clear);
        }

        /// <summary>
        /// Async invocation of reset on the UI thread.
        /// </summary>
        public void invokeReset()
        {
            SmartDispatcher.BeginInvoke(reset);
        }

        /// <summary>
        /// Draw the given problem to the drawing.
        /// </summary>
        /// <param name="problem">The problem to draw.</param>
        public void draw(UIProblemDrawer.ProblemDescription problem)
        {
            // Reset to create the dictionaries.
            reset();

            //Draw the hard-coded problem.
            try
            {
                drawPoints(problem);
                drawSegments(problem);
                drawCircles(problem);

                shadeProblem(problem);
            } catch (System.Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message + " " + e.StackTrace);
            }
        }

        /// <summary>
        /// Clear all shadings and figures on the UI.
        /// </summary>
        public void clear()
        {
            drawingHost.CurrentDrawing.ClearRegionShadings();
            var figures = new List<IFigure>();
            drawingHost.CurrentDrawing.Figures.ForEach(figure => figures.Add(figure));
            figures.ForEach(figure => RemoveFigure(figure));
        }

        /// <summary>
        /// Reset the problem drawer. That is, reset all the UI lookup information.
        /// </summary>
        public void reset()
        {
            points = new Dictionary<GeometryTutorLib.ConcreteAST.Point, IPoint>();
            segments = new Dictionary<GeometryTutorLib.ConcreteAST.Segment, Segment>();
            circles = new Dictionary<GeometryTutorLib.ConcreteAST.Circle, Circle>();
        }

        /// <summary>
        /// Draw each point in the problem and save it to the lookup dictionary.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawPoints(UIProblemDrawer.ProblemDescription problem)
        {
            //Add each point to the drawing
            foreach (var pt in problem.Points)
            {
                //Create and add the point
                var point = Factory.CreateFreePoint(drawingHost.CurrentDrawing, new System.Windows.Point(pt.X, pt.Y));
                point.Name = pt.name;
                Actions.Add(drawingHost.CurrentDrawing, point);

                //Save to lookup dictionary
                points.Add(pt, point);
            }
        }

        /// <summary>
        /// Draw each segment in the problem and save it to the lookup dictionary.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawSegments(UIProblemDrawer.ProblemDescription problem)
        {
            //Add each segment to the drawing
            foreach (var seg in problem.Segments)
            {
                if (seg != null)
                {
                    //Look already drawn points
                    var pt1 = points[seg.Point1];
                    var pt2 = points[seg.Point2];

                    //Create and add the segment
                    var segment = Factory.CreateSegment(drawingHost.CurrentDrawing, pt1, pt2);
                    Actions.Add(drawingHost.CurrentDrawing, segment);

                    //Save to lookup dictionary
                    segments.Add(seg, segment);
                }
            }
        }

        /// <summary>
        /// Draw each circle in the problem and save it to the lookup dictionary.
        /// Circles in LiveGeometry need two points. We have the center, but may need a point on the circle itself.
        /// If a point exists on the circle we will use that, if not a point will be created on the circle to the direct
        /// right of the center.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void drawCircles(UIProblemDrawer.ProblemDescription problem)
        {
            //Add circles to the drawing
            foreach (var circ in problem.Circles)
            {
                //Lookup center point
                var center = points[circ.center];
                IPoint pointOnCircle;

                if (circ.pointsOnCircle.Count > 0) //Point on circle exists
                {
                    pointOnCircle = points[circ.pointsOnCircle[0]];
                }
                else //Does not exist, we need to make one
                {
                    pointOnCircle = Factory.CreateFreePoint(
                        drawingHost.CurrentDrawing,
                        new System.Windows.Point(circ.center.X + circ.radius, circ.center.Y));
                }

                //Create circle and add to drawing
                IPoint[] dependencies = { center, pointOnCircle };
                Circle circle = Factory.CreateCircle(drawingHost.CurrentDrawing, new List<IFigure>(dependencies));
                Actions.Add(drawingHost.CurrentDrawing, circle);

                //Save to lookup dictionary
                circles.Add(circ, circle);
            }
        }

        /// <summary>
        /// If the problem is a shaded area problem, shade the goal regions.
        /// </summary>
        /// <param name="problem">The problem being drawn.</param>
        private void shadeProblem(UIProblemDrawer.ProblemDescription problem)
        {
            if (problem.Regions != null)
            {
                //Shade each region
                foreach (var region in problem.Regions)
                {
                    ShadedRegion sr = new ShadedRegion(region);
                    sr.Draw(drawingHost.CurrentDrawing, ShadedRegion.BRUSHES[1]);
                }
            }
        }

        /// <summary>
        /// Helper method used to remove figures from the drawing. 
        /// Will remove any figure that is not the coordinate grid.
        /// </summary>
        /// <param name="figure">The figure to be removed.</param>
        private void RemoveFigure(IFigure figure)
        {
            if (figure is CartesianGrid) { }
            else
            {
                Actions.Remove(figure);
            }
        }
    }
}
