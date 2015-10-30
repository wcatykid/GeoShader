using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.EngineUIBridge
{
    public static class HardCodedProblemsToUI
    {
        private static List<ProblemInfo> problems = new List<ProblemInfo>();

        public static void AddProblem(string name, List<Point> pts, List<Circle> circs, List<Segment> segs)
        {
            System.Diagnostics.Debug.WriteLine(circs.Count());
            problems.Add(new ProblemInfo(name, pts, circs, segs));
        }

        public class ProblemInfo
        {
            public string problemName;

            public List<Point> points;
            public List<Circle> circles;
            public List<Segment> segments;

            public ProblemInfo(string name, List<Point> pts, List<Circle> circs, List<Segment> segs)
            {
                problemName = name;
                points = pts;
                circles = circs;
                segments = segs;
            }
        }


    }
}
