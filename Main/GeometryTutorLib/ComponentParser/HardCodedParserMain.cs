using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class HardCodedParserMain : AbstractParserMain
    {
        /// <summary>
        /// For hard-coded (old-style) tests of the system.
        /// </summary>
        public HardCodedParserMain(List<Point> points,
                                   List<GeometryTutorLib.ConcreteAST.Collinear> collinear,
                                   List<GeometryTutorLib.ConcreteAST.Segment> segments,
                                   List<GeometryTutorLib.ConcreteAST.Circle> circles,
                                   bool problemIsOn) : base()
        {
            //
            // Calculate all of the implied components of the figure.
            //
            implied = new ImpliedComponentCalculator(points, collinear, segments, circles);
            implied.ConstructAllImplied();
        }
    }
}