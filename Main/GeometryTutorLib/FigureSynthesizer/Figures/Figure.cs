using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract partial class Figure
    {
#if FIGURE_SYNTHESIZER
        protected List<Midpoint> midpoints = new List<Midpoint>();
        public List<Midpoint> GetMidpointClauses() { return midpoints; }
#endif
        public virtual bool CoordinateCongruent(Figure that) { return false; }
        public virtual List<Constraint> GetConstraints() { return new List<Constraint>(); }

        public List<Point> snapToPoints { get; protected set; }
        public List<Point> allComposingPoints { get; protected set; }
        // Subsegments of each side (parallel with the orderedsides)
        public List<Segment>[] sideSubsegments { get; protected set; }
        protected List<Segment>[] completeSideSegments;
        public List<Segment> GetCompleteSideSegments()
        {
            List<Segment> segs = new List<Segment>();
            foreach (List<Segment> segList in completeSideSegments)
            {
                segList.ForEach(s => segs.Add(s));
            }
            return segs;
        }

        public virtual double CoordinatizedArea() { throw new NotImplementedException(); }

        //
        //
        // Random number Generation (begin)
        //
        //
        private const int LOWER_BOUND = 0;
        private const int UPPER_BOUND = 24;

        public static readonly Point origin = new Point("Origin", 0, 0);

        private static Random rand = new Random();

        private static readonly int[] SMALL_VALUES = { 1, 2, 3, 4, 5 };
        public static int SmallIntegerValue()
        {
            int index = rand.Next(0, SMALL_VALUES.Length);

            return SMALL_VALUES[SMALL_VALUES.Length - 1 /* index */ ];
        }
        //
        // Return a default length of a side; this is chosen randomly, but is one of the following: 
        //
        private static readonly int[] SIDE_LENGTHS = { 2, 4, 8, 12, 16, 20, 24, 30, 36, 48 };
        public static int DefaultSideLength()
        {
            int index = rand.Next(0, SIDE_LENGTHS.Length);

            return SIDE_LENGTHS[/* SIDE_LENGTHS.Length - 1 */ index ];
        }

        public static int GenerateIntValue()
        {
            return SIDE_LENGTHS[(int)rand.Next(0, SIDE_LENGTHS.Length)];
        }

        private static readonly int[] ANGLE_MEASURES = { 30, 45, 60, 90 };
        public static int DefaultFirstQuadrantNonRightAngle()
        {
            int index = rand.Next(0, ANGLE_MEASURES.Length);

            return ANGLE_MEASURES[ANGLE_MEASURES.Length - 2 /* index */ ];
        }

        public static int DefaultFirstQuadrantAngle()
        {
            int index = rand.Next(0, ANGLE_MEASURES.Length);

            return ANGLE_MEASURES[ANGLE_MEASURES.Length - 1 /* index */ ];
        }
        //
        //
        // Random number Generation (end)
        //
        //

        public static Point GetPointByLengthAndAngleInStandardPosition(int length, double measure)
        {
            return new Point("", length * Math.Cos(Angle.toRadians(measure)), length * Math.Sin(Angle.toRadians(measure)));
        }

        public static Point GetPointByLengthAndAngleInThirdQuadrant(int length, int measure)
        {
            return new Point("", length * Math.Cos(Angle.toRadians(180 - measure)), length * Math.Sin(Angle.toRadians(180 - measure)));
        }

        public static Figure ConstructDefaultShape(ShapeType type)
        {
            switch(type)
            {
                case ShapeType.TRIANGLE:               return Triangle.ConstructDefaultTriangle();
                case ShapeType.ISOSCELES_TRIANGLE:     return IsoscelesTriangle.ConstructDefaultIsoscelesTriangle();
                case ShapeType.RIGHT_TRIANGLE:         return RightTriangle.ConstructDefaultRightTriangle();
                case ShapeType.EQUILATERAL_TRIANGLE:   return EquilateralTriangle.ConstructDefaultEquilateralTriangle();
                case ShapeType.KITE:                   return Kite.ConstructDefaultKite();
                case ShapeType.QUADRILATERAL:          return Quadrilateral.ConstructDefaultQuadrilateral();
                case ShapeType.TRAPEZOID:              return Trapezoid.ConstructDefaultTrapezoid();
                case ShapeType.ISO_TRAPEZOID:          return IsoscelesTrapezoid.ConstructDefaultIsoscelesTrapezoid();
                case ShapeType.PARALLELOGRAM:          return Parallelogram.ConstructDefaultParallelogram();
                case ShapeType.RECTANGLE:              return Rectangle.ConstructDefaultRectangle();
                case ShapeType.RHOMBUS:                return Rhombus.ConstructDefaultRhombus();
                case ShapeType.SQUARE:                 return Square.ConstructDefaultSquare();
                case ShapeType.CIRCLE:                 return Circle.ConstructDefaultCircle();
                case ShapeType.SECTOR:                 return Sector.ConstructDefaultSector();
            }

            return null;
        }

        // Get the variables required for the given figure.
        public virtual List<Segment> GetAreaVariables() { return new List<Segment>(); }

        public bool Overlaps(Figure that)
        {
            return this.GetFigureAsAtomicRegion().OverlapsWith(that.GetFigureAsAtomicRegion());
        }

        //
        // Used by Square and Rectangle
        //
        public static FigSynthProblem MakeAdditionProblem(Figure outerShape, Figure appended)
        {
            if (outerShape.Contains(appended) || appended.Contains(outerShape) || outerShape.Overlaps(appended)) return null;

            AdditionSynth addSynth = new AdditionSynth(outerShape, appended);

            List<AtomicRegion> atoms = new List<AtomicRegion>();
            atoms.Add(outerShape.GetFigureAsAtomicRegion());
            atoms.Add(appended.GetFigureAsAtomicRegion());

            addSynth.SetOpenRegions(atoms);

            return addSynth;
        }
    }
}
