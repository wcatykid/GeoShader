using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    class KiteProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new KiteProblem01(false, false));
            problems.Add(new KiteProblem02(false, false));

            return problems;
        }
    }
}
