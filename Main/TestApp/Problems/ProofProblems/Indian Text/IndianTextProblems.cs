using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class IndianTextProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            problems.Add(new IPage119Problem2(true, true));   // GTG
            problems.Add(new IPage119Problem3(true, false));   // GTG
            problems.Add(new IPage119Problem5(true, false));   // GTG
            problems.Add(new IPage123Example5(true, true));   // GTG
            problems.Add(new IPage123Example6(true, false));   // GTG
            problems.Add(new IPage128Problem01(true, false));  // GTG  // Good to analyze
            problems.Add(new IPage128Problem03(true, false));  // GTG
            //problems.Add(new IPage120Problem6(true, false));   // GTG  ONLY with Angle Addition Axiom
            problems.Add(new IPage120Problem7(true, true));   // GTG
            problems.Add(new IPage120Problem8(true, true));   // GTG  Overlapping Right Triangles
            problems.Add(new JPage140Problem9(true, false));   // GTG
            problems.Add(new JPage140Problem6(true, true));   // GTG: Interleaving Triangles
            problems.Add(new JPage140Problem7(true, false));   // GTG
            problems.Add(new JPage141Problem11(true, false));  // GTG
            problems.Add(new JPage135Example4(true, true));   // GTG


            //problems.Add(new IPage149TopExercise(true, false)); // Requires implementing a new theorem
            // May omit these from study
            // Proportionality problems.Add(new JPage152Problem01());
            // Problems with proportionality problems.Add(new JPage153Problem09());

            return problems;
        }
    }
}
