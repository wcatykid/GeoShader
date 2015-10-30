using System.Collections.Generic;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.EngineUIBridge
{
    /// <summary>
    /// A description of a problem for the back end to solve.
    /// This class is used to pass information from the front end to the back end.
    /// </summary>
    public class ProblemDescription
    {
        public List<Point> points;
        public List<GroundedClause> figure;
        public List<GroundedClause> givens;

        public ProblemDescription()
        {
            points = new List<Point>();
            figure = new List<GroundedClause>();
            givens = new List<GroundedClause>();
        }
    }
}
