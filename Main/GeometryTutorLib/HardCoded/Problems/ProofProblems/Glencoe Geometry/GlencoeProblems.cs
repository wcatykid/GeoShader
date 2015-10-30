using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class GlencoeProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page156Problem34(false, false));  // GTG
            problems.Add(new Page156Problem35(false, true));  // GTG
            problems.Add(new Page156Problem36(false, false));  // GTG
            problems.Add(new Page164Problem36(false, false));  // GTG
            problems.Add(new Page164Problem44(false, false));  // GTG
            problems.Add(new Page156Problem37(false, false));  // GTG
            problems.Add(new Page226Problem41(false, false));  // GTG
            problems.Add(new Page226Problem42(false, true));  // GTG
            problems.Add(new Page226Problem43(false, true));  // GTG
            problems.Add(new Page226Problem44(false, false));  // GTG
            problems.Add(new Page231Problem14(false, true));  // GTG
            problems.Add(new Page273Problem39(false, false));  // GTG
            problems.Add(new Page273Problem40(false, true));  // GTG


//            problems.Add(new Page231Picture(false, false));  // Encoding of a point

            return problems;
        }
    }
}
