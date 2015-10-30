using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class McDougallProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new Page168Problem34(true, false));   // GTG
            problems.Add(new Page159Problem37(true, false));   // GTG
            problems.Add(new Page159Problem41(true, false));   // GTG
            problems.Add(new Page160Problem42(true, false));   // GTG
            problems.Add(new Page166Problem25(true, false));   // GTG on limited givens
            problems.Add(new Page168Problem35(true, false));   // GTG
            problems.Add(new Page284Problem17(true, true));   // GTG 
            problems.Add(new Page284Problem18(true, false));   // GTG 
            problems.Add(new Page284Example44(true, true));   // GTG
            problems.Add(new Page284Exameple45(true, false));  // GTG
            problems.Add(new Page284Example46(true, true));   // GTG
            problems.Add(new Page285Problem21(true, true));   // GTG
            problems.Add(new Page285Problem22(true, true));   // GTG
            problems.Add(new Page285Problem23(true, false));   // GTG
            problems.Add(new Page286Problem8(true, false));    // GTG; multiple solutions possible
            problems.Add(new Page286Problem9(true, false));    // GTG
            problems.Add(new Page301Problem50(true, false));   // GTG
            problems.Add(new Page301Problem51(true, true));   // GTG
            problems.Add(new Page301Problem52(true, true));   // GTG
            problems.Add(new Page316Problem42(true, false));   // GTG
            problems.Add(new Page316Problem43(true, false));   // GTG
            problems.Add(new Page316Problem44(true, false));   // GTG





            //problems.Add(new Page168Problem36(true, false));  // Endocing  
            //problems.Add(new Page168Problem37(true, false));  // Endocing  
            //problems.Add(new Page197Problem37(true, false));  // Quite algebraic
            //problems.Add(new Page160Problem43(true, false));  // Encoding; not for this implementation round
            // problems.Add(new Page301Problem42(true, false));  Can't deduce geometrically; coordinate-based needed

            return problems;
        }
    }
}
