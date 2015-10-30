using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class McDougallWorkbookProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page37Problem2(true, false));         // GTG
            problems.Add(new Page41Problem15(true, false));        // GTG
            problems.Add(new Page42Problem16(true, false));        // GTG
            problems.Add(new Page48Problem23To31(true, false));    // GTG
            problems.Add(new Page66Problem16(true, true));        // GTG : 1(5) Subset of givens required to prove same result
            problems.Add(new Page68Problem13(true, false));        // GTG
            problems.Add(new Page69Problem14(true, true));        // GTG
            problems.Add(new Page72Problem17(true, true));        // GTG
            problems.Add(new Page73Problem8(true, false));         // GTG
            problems.Add(new Page73Problem9(true, true));         // GTG
            problems.Add(new Page74Problem14To16(true, false));    // 3(9) Uses a subset of the givens to prove the same result
            problems.Add(new Page75Problem17(true, false));        // GTG
            problems.Add(new Page75Problem18(true, false));        // GTG
            problems.Add(new Page76Problem7(true, true));         // GTG
            problems.Add(new Page76Problem4(true, false));         // GTG
            problems.Add(new Page76Problem8(true, true));         // GTG
            problems.Add(new Page77Problem11(true, false));        // GTG
            problems.Add(new Page78Problem12(true, true));        // GTG
            problems.Add(new Page78Problem13(true, true));        // GTG
            problems.Add(new Page79Problem7(true, false));         // GTG
            problems.Add(new Page79Problem8(true, true));         // GTG
            problems.Add(new Page80Problem9(true, true));          // GTG
            problems.Add(new Page80Problem10(true, false));        // GTG
            problems.Add(new Page90Problem22(true, true));        // GTG
            problems.Add(new Page90Problem23(true, false));        // GTG    BACKWARD only works if <= 2 givens

            return problems;
        }
    }
}
