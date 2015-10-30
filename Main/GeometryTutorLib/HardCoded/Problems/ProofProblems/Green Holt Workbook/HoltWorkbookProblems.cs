using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class HoltWorkbookProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page13Problem10(true, false));  // GTG
            problems.Add(new Page23Problem9(true, true));   // GTG
            problems.Add(new Page24Problem7(true, true));   // GTG
            problems.Add(new Page25Problem8(true, false));   // GTG
            problems.Add(new Page26Problem2(true, true));   // GTG
            problems.Add(new Page17Problem9(true, false));   // GTG

            return problems;
        }
    }
}
