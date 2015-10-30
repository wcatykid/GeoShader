using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.TutorParser;

namespace DynamicGeometry.UI.RegionShading
{
    [Category(BehaviorCategories.Regions)]
    [Order(1)]
    public class ShadedRegionCreator : Behavior
    {
        public static List<AtomicRegion> Atoms;

        public override string Name
        {
            get { return "Mark Region"; }
        }

        public override string HintText
        {
            get { return "Click a region to mark it."; }
        }

        public override bool EnabledByDefault
        {
            get { return false; }
        }

        public override FrameworkElement CreateIcon()
        {
            var uri = new Uri("LiveGeometry;component/MarkRegion.png", UriKind.Relative);
            var streamInfo = Application.GetResourceStream(uri);
            BitmapImage source = new BitmapImage();
            source.SetSource(streamInfo.Stream);
            return new Image() { Source = source, Stretch = Stretch.None };
        }

        // private System.Collections.Generic.List<AtomicRegion> atoms = null;
        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.Assert(Atoms != null, "Atomic Regions not set for ShadedRegionCreator");

            CoordinateSystem cs = Drawing.CoordinateSystem;
            Point pt = new Point(e.GetPosition(Drawing.Canvas).X, e.GetPosition(Drawing.Canvas).Y);
            Point logicalPt = new Point(cs.ToLogical(pt.X - cs.Origin.X), cs.ToLogical(cs.Origin.Y - pt.Y));

            foreach (AtomicRegion ar in Atoms)
            {
                if (ar.PointLiesInside(new GeometryTutorLib.ConcreteAST.Point("shadingtest", logicalPt.X, logicalPt.Y)))
                {
                    ShadedRegion sr = new ShadedRegion(ar);
                    sr.Draw(Drawing, ShadedRegion.BRUSHES[0]);
                }
            }
        }
    }
}
