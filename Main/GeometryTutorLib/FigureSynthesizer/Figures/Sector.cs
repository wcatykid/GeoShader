using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public partial class Sector : Figure
    {
        public override double CoordinatizedArea()
        {
            double angle = -1;

            if (theArc is Semicircle) angle = 180;
            if (theArc is MinorArc) angle = theArc.GetMinorArcMeasureDegrees();
            if (theArc is MajorArc) angle = theArc.GetMajorArcMeasureDegrees();

            return Math.Pow(theArc.theCircle.radius, 2) * Math.PI * (angle / 360);
        }

        public override bool CoordinateCongruent(Figure that)
        {
            Sector thatSector = that as Sector;
            if (thatSector == null) return false;

            return this.theArc.CoordinateCongruent(thatSector.theArc);
        }

        public static List<FigSynthProblem> SubtractShape(Figure outerShape, List<Connection> conns, List<Point> points)
        {
            throw new NotImplementedException();
        }

        public static List<FigSynthProblem> AppendShape(Figure outerShape, List<Segment> segments)
        {
            throw new NotImplementedException();
        }

        public static Sector ConstructDefaultSector()
        {
            return null;
        }
    }
}