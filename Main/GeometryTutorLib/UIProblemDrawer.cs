using System;
using System.Collections.Generic;
using System.Diagnostics;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    /// <summary>
    /// A class that can be used tell the UI to draw problems.
    /// Follows a variation on the singleton pattern - must be explicitly created once.
    /// </summary>
    public class UIProblemDrawer
    {
        private static UIProblemDrawer instance = null;

        private Action<ProblemDescription> invokeDraw;
        private Action invokeClear;
        private Action invokeReset;

        /// <summary>
        /// Create a new UIProblemDrawer.
        /// </summary>
        /// <param name="draw">The Action that handles a UI draw.</param>
        /// <param name="clear">The Action that handles a UI clear.</param>
        /// <param name="reset">The Action that handles a ProblemDrawer reset.</param>
        private UIProblemDrawer(Action<ProblemDescription> draw, Action clear, Action reset)
        {
            invokeDraw = draw;
            invokeClear = clear;
            invokeReset = reset;
        }

        /// <summary>
        /// Create the UIProblemDrawer.
        /// This method should only be called once, and should be called before any call to getInstance().
        /// </summary>
        /// <param name="draw">The Action that handles a UI draw.</param>
        /// <param name="clear">The Action that handles a UI clear.</param>
        /// <param name="reset">The Action that handles a ProblemDrawer reset.</param>
        public static void create(Action<ProblemDescription> draw, Action clear, Action reset)
        {
            Debug.Assert(instance == null, "create() should only be called once.");
            instance = new UIProblemDrawer(draw, clear, reset);
        }

        /// <summary>
        /// Get an instance of the UIProblemDrawer.
        /// Should only be called after create has been called in the UI.
        /// </summary>
        /// <returns></returns>
        public static UIProblemDrawer getInstance()
        {
            Debug.Assert(instance != null, "create() should be called before getInstance().");
            return instance;
        }

        /// <summary>
        /// Draw the given problem to the UI.
        /// </summary>
        /// <param name="desc"></param>
        public void draw(ProblemDescription desc)
        {
            invokeDraw(desc);
        }

        /// <summary>
        /// Clear all input on the UI.
        /// </summary>
        public void clear()
        {
            invokeClear();
        }

        /// <summary>
        /// Reset the ProblemDrawer (ie clear all cached UI elements).
        /// </summary>
        public void reset()
        {
            invokeReset();
        }

        /// <summary>
        /// A simplistic description of the problem to be drawn.
        /// </summary>
        public class ProblemDescription
        {
            public List<Point> Points { get; set; } //Points to be drawn
            public List<Segment> Segments { get; set; } //Segments to be drawn
            public List<Circle> Circles { get; set; } //Circles to be drawn
            public List<AtomicRegion> Regions { get; set; } //Regions to be shaded
        }
    }
}
