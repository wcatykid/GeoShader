using GeometryTutorLib.ConcreteAST;
using System.Collections.Generic;

namespace GeometryTestbed
{
    public abstract class ActualProblem
    {
        // Hard-coded intrinsic problem characteristics
        protected List<GroundedClause> intrinsic;

        // Boolean facts
        protected List<GroundedClause> given;

        // Boolean the book problem is attempting to prove; use in validation of figures / generated problems
        // One <figure, given> pair may contain multiple goals
        protected List<GroundedClause> goals;

        // Formatted Labeling of the Problem
        protected string problemName;

        public bool problemIsOn { get; protected set; }

        //
        // For constructing clauses consistent with the UI.
        //
        protected LiveGeometry.TutorParser.HardCodedParserMain parser;

        protected List<Point> points;
        protected List<Collinear> collinear;
        protected List<Segment> segments;
        protected List<Circle> circles;
        protected List<Semicircle> semicircles;

        public const bool INCOMPLETE = false;
        public const bool COMPLETE = true;
        public bool isComplete;

        // Main routine to run a problem through the system.
        public abstract void Run();

        public ActualProblem(bool runOrNot, bool comp)
        {
            intrinsic = new List<GroundedClause>();
            given = new List<GroundedClause>();
            goals = new List<GroundedClause>();

            points = new List<Point>();
            collinear = new List<Collinear>();
            segments = new List<Segment>();
            circles = new List<Circle>();
            semicircles = new List<Semicircle>();

            problemName = "TODO: NAME ME" + this.GetType();
            problemIsOn = runOrNot;

            isComplete = comp;
        }

        protected void ConstructIntrinsicSet()
        {
            parser.implied.allFigurePoints.ForEach(pt => intrinsic.Add(pt));
            parser.implied.segments.ForEach(seg => intrinsic.Add(seg));
            parser.implied.inMiddles.ForEach(im => intrinsic.Add(im));
            parser.implied.angles.ForEach(angle => intrinsic.Add(angle));
            parser.implied.ssIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.minorArcs.ForEach(arc => intrinsic.Add(arc));
            parser.implied.majorArcs.ForEach(arc => intrinsic.Add(arc));
            parser.implied.semiCircles.ForEach(arc => intrinsic.Add(arc));
            parser.implied.circles.ForEach(circ => intrinsic.Add(circ));
            parser.implied.csIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.ccIntersections.ForEach(inter => intrinsic.Add(inter));
            parser.implied.arcInMiddle.ForEach(im => intrinsic.Add(im));

            foreach (List<Polygon> polyList in parser.implied.polygons)
            {
                polyList.ForEach(poly => intrinsic.Add(poly));
            }
        }
    }
}