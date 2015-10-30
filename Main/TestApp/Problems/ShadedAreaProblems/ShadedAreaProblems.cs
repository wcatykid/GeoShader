using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTestbed
{
    public static class ShadedAreaProblems
    {
        public static List<ActualShadedAreaProblem> GetProblems()
        {
            List<ActualShadedAreaProblem> problems = new List<ActualShadedAreaProblem>();

            //
            // Testing
            //
            //problems.Add(new ThreeCircleTwoOverlapTester(true, false)); // GTG
            //problems.Add(new ThreeCirclePathologicalTester(true, false)); GTG
            //problems.Add(new RegularPolygonCrushingTester(true, false));
            //problems.Add(new BasicInteriorPolygonTester2(true, false));
            //problems.Add(new BasicInteriorPolygonTester(true, false));
            //problems.Add(new BasicPolygonTester(true, false));
            //problems.Add(new ContainmentTester(true, false));
            //problems.Add(new CircCircRegionTester(true, false));
            //problems.Add(new CircCircCircRegionTester(true, false));
            //problems.Add(new FilamentTester(true, false));
          
            //
            // Class X
            //
            // problems.Add(new Page1Col1Prob1(true, false));         // Can't find solution.
            // problems.Add(new Page1Col1Prob2(true, false));         // Arc equations....

            // problems.Add(new Page1Col1Prob3(true, false));        // Arc Equations

            // problems.Add(new Page1Col1Prob5(true, false));        // GTG: Pathological
            // problems.Add(new Page1Col2Prob1(true, false));        // GTG
            // problems.Add(new Page1Col2Prob2(true, false));        // GTG
            // problems.Add(new Page1Col2Prob3(true, false));        // GTG
            // problems.Add(new Page2Col1Prob1(true, false));        // GTG
            // problems.Add(new Page2Col1Prob2(true, false));        // GTG
            // problems.Add(new Page2Col1Prob3(true, false));        // GTG
            // problems.Add(new Page2Col2Prob1(true, false));        // GTG
            // problems.Add(new Page2Col2Prob2(true, false));        // GTG
            // problems.Add(new TesterForCircleTriangle(true, false));


            //
            // Jurgensen
            //
            // problems.Add(new Page2Prob15(true, false));    // GTG
            // problems.Add(new Page2Prob17(true, false));    // GTG
            // problems.Add(new Page2Prob18(true, false));    // Arc equation
            // problems.Add(new Page2Prob19(true, false));    // GTG
            // problems.Add(new Page3Prob21(true, false));    // GTG: Takes a while.
            // problems.Add(new Page3Prob22(true, false));    // GTG
            // problems.Add(new Page3Prob23(true, false));    // OMIT: Circular filaments.
            // problems.Add(new Page2Prob28(true, false));    // Arc equations needed (as well as work to calculate trapezoid area))  
            // problems.Add(new Page4Prob7(true, false));     // GTG
            // problems.Add(new Page4Prob8(true, false));     // GTG
            // problems.Add(new Page4Prob13(true, false));    // GTG
            // problems.Add(new Page4Prob14(true, false));    // Encoding: Points
            // problems.Add(new Page4Prob19(true, false));    // Encoding

            //
            // McDougall
            //
            // problems.Add(new Page5Row1Prob24(true, false));   // GTG
            // problems.Add(new Page5Row1Prob25(true, false));   // GTG
            // problems.Add(new Page5Row1Prob26(true, false));   // GTG
            // problems.Add(new Page5Row2Prob27(true, false));   // GTG
            // problems.Add(new Page5Row2Prob28(true, false));   // GTG
            // problems.Add(new Page5Row3Prob2(true, false));    // GTG
            // problems.Add(new Page5Row3Prob3(true, false));    // GTG
            // problems.Add(new Page5Row3Prob4(true, false));    // GTG
            // problems.Add(new Page5Row5Prob17(true, false));   // Unchecked

            //
            // Singapore
            //
            // problems.Add(new Page205(true, false));  // GTG
            // problems.Add(new Page207(true, false));        // Takes a long time; eliminate supplementary from parallelograms to speed up ; Angle equation problem.
            // problems.Add(new Page208(true, false));  // GTG
            // problems.Add(new Page209(true, false));  // GTG (Long deduction engine)
            // problems.Add(new Page210(true, false));        // Incomputable ; LONG execution need angle addition axiom ; Don't include.
            // problems.Add(new Page199(true, false));        // FUBAR

            return problems;
        }
    }
}
