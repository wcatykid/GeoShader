using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class AngleArcProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            //problems.Add(new Test01(true, false));
            //problems.Add(new Test02(true, false));
            //problems.Add(new Test03(true, false));
            //problems.Add(new Test04(true, false));
            //problems.Add(new Test05(true, false));
            //problems.Add(new Test06(true, false));
            //problems.Add(new Test07(true, false));
            //problems.Add(new Test08(true, false));
            //problems.Add(new Test09(true, false));
            //problems.Add(new Test10(true, false));
            problems.Add(new Test11(true, false));

            return problems;
        }
    }
}
