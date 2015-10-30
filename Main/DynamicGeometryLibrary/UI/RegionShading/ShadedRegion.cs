using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace DynamicGeometry.UI.RegionShading
{
    public class ShadedRegion
    {
        /// <summary>
        /// Some predefined hatch brushes to shade the regions with.
        /// </summary>
        public static readonly Brush[] BRUSHES = 
        {
            MakeHatchBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), Color.FromArgb(0xFF, 0x80, 0x00, 0x00)), // red & maroon
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0x00), Color.FromArgb(0xFF, 0x00, 0x80, 0x00)), // lime & green
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), Color.FromArgb(0xFF, 0x00, 0x00, 0x80)), // blue & navy
            MakeHatchBrush(Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF), Color.FromArgb(0xFF, 0x00, 0x80, 0x80)), // aqua & teal
            MakeHatchBrush(Color.FromArgb(0xFF, 0xFF, 0x00, 0xFF), Color.FromArgb(0xFF, 0x80, 0x00, 0x80))  // fuschia & purple
        };


        public AtomicRegion Region { get; private set; }
        public System.Windows.Shapes.Polygon Shading { get; private set; }

        /// <summary>
        /// Create a new ShadedRegion
        /// </summary>
        /// <param name="region">The region to shade</param>
        public ShadedRegion(AtomicRegion region)
        {
            Region = region;
        }

        /// <summary>
        /// Convert a physical point (pixel) to a logical point.
        /// </summary>
        /// <param name="cs">The current coordiante system</param>
        /// <param name="physical">The point to convert</param>
        /// <returns>The converted point</returns>
        public static Point ToLogical(CoordinateSystem cs, Point physical)
        {
            return new Point(cs.ToLogical(physical.X - cs.Origin.X), cs.ToLogical(cs.Origin.Y - physical.Y));
        }

        /// <summary>
        /// Convert a logical point to a nearby physical point (pixel).
        /// </summary>
        /// <param name="cs">The current coordinate system</param>
        /// <param name="logical">The point to convert</param>
        /// <returns>The converted point</returns>
        public static Point ToPhysical(CoordinateSystem cs, Point logical)
        {
            return cs.ToPhysical(logical);
        }

        /// <summary>
        /// Draw the shaded region as an approximated polygon and return the polygon
        /// The Zindex, Top, and Left attributes will also be set and the shading will be added to the drawing.
        /// </summary>
        /// <param name="drawing">The current drawing</param>
        /// <param name="shadingBrush">The brush to shade the region with</param>
        /// <returns>The graphical representation of the region.</returns>
        public UIElement Draw(Drawing drawing, Brush shadingBrush)
        {
            //Create the polygon with the correct physical points
            Shading = new System.Windows.Shapes.Polygon();
            SetPolygonPoints(Shading, drawing.CoordinateSystem);

            //Color the polygon
            Shading.Fill = shadingBrush;
            Shading.Stroke = new SolidColorBrush(Colors.DarkGray);
            Shading.StrokeThickness = 1;

            //Orient the polygon
            Canvas.SetTop(Shading, 0);
            Canvas.SetLeft(Shading, 0);
            Canvas.SetZIndex(Shading, (int)ZOrder.Shading);

            //Add to the drawing
            drawing.AddRegionShading(this);

            return Shading;
        }

        /// <summary>
        /// Redraw the region polygon after a zoom.
        /// </summary>
        /// <param name="drawing">The drawing the region is on.</param>
        public void ZoomRedraw(Drawing drawing)
        {
            SetPolygonPoints(Shading, drawing.CoordinateSystem);
        }

        /// <summary>
        /// Transform the polygonal approximation of the region created in the back end into
        /// the point collection for the UI Polygon.
        /// </summary>
        /// <param name="shading">The UI polygon for the region. Will have its points set.</param>
        /// <param name="cs">The coordinate system for the UI drawing.</param>
        private void SetPolygonPoints(System.Windows.Shapes.Polygon shading, CoordinateSystem cs)
        {
            //Create the points collection for the polygon
            PointCollection points = new PointCollection();
            foreach (GeometryTutorLib.ConcreteAST.Point p in Region.GetPolygonalized().points)
            {
                points.Add(ToPhysical(cs, new Point(p.X, p.Y)));
            }
            shading.Points = points;
        }

        /// <summary>
        /// Make a brush with a diagonal hatching pattern using the two given colors.
        /// </summary>
        /// <param name="c1">The first color of the hatch</param>
        /// <param name="c2">The second color of the hatch</param>
        /// <returns>The hatch brush</returns>
        public static Brush MakeHatchBrush(Color c1, Color c2)
        {
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.MappingMode = BrushMappingMode.Absolute;
            brush.SpreadMethod = GradientSpreadMethod.Repeat;
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(6, 6);
            GradientStop gs = new GradientStop();
            gs.Color = c1;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c1;
            gs.Offset = 0.5;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c2;
            gs.Offset = .5;
            brush.GradientStops.Add(gs);
            gs = new GradientStop();
            gs.Color = c2;
            gs.Offset = 1;
            brush.GradientStops.Add(gs);
            return brush;
        }
    }
}
