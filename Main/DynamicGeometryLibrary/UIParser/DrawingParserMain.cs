using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;
using DynamicGeometry;
using GeometryTutorLib.ConcreteAST;
using GeometryTutorLib.TutorParser;

namespace LiveGeometry.TutorParser
{
    /// <summary>
    /// Provides various functions related to converting LiveGeometry Figure into Grounded Clauses suitable for input into the proof engine.
    /// </summary>
    public class DrawingParserMain
    {
        private Drawing drawing;
        public HardCodedParserMain backendParser { get; protected set; }

        /// <summary>
        /// Create a new Drawing Parser.
        /// </summary>
        /// <param name="drawing">The drawing to parse.</param>
        /// <param name="parseController">The parseController, used to add disambiguation dialogs.</param>
        public DrawingParserMain(Drawing drawing) : base()
        {
            this.drawing = drawing;
        }

        public void Parse()
        {
            //
            // From the Live Geometry UI, we parse those components of the figures
            //
            List<IFigure> ifigs = new List<IFigure>();
            drawing.Figures.ForEach<IFigure>(f => ifigs.Add(f));

            // Parsing of the UI-based components results in populated lists for:
            // (a) Named points
            // (b) Segments
            // (c) Polygons (includes triangles and quadrilaterals)
            // (d) Regular Polygons (in polygon structure)
            // (e) Circles
            DirectComponentsFromUI uiParser = new DirectComponentsFromUI(drawing, ifigs);
            uiParser.Parse();

            backendParser = new HardCodedParserMain(uiParser.definedPoints, new List<GeometryTutorLib.ConcreteAST.Collinear>(),
                                                    uiParser.definedSegments, uiParser.circles, true);
        }
    }
}