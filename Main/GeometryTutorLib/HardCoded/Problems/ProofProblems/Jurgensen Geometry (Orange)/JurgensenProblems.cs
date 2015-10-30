using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.GeometryTestbed
{
    public static class JurgensenProblems
    {
        public static List<ActualProofProblem> GetProblems()
        {
            List<ActualProofProblem> problems = new List<ActualProofProblem>();

            // Transversals
            problems.Add(new Page23Theorem11(false, true));                  // GTG
            problems.Add(new Page32ClassroomProblem11To14(false, false));     // GTG

            // Parallel
            problems.Add(new Page60Theorem22(false, false));                  // GTG
            problems.Add(new Page62Problem1(false, false));                   // GTG
            problems.Add(new Page62Problem2(false, false));                   // GTG
            problems.Add(new Page62Problems3To4(false, false));               // GTG if no AngleAdditionAxiom

            // Congruent Triangles
            problems.Add(new Page113Problem7(false, true));                  // GTG
            problems.Add(new Page134Problem6(false, true));                  // GTG
            problems.Add(new Page134Problem7(false, true));                  // GTG
            problems.Add(new Page135Problem21(false, false));                 // GTG
            problems.Add(new Page144ClassroomExercise01(false, false));       // GTG
            problems.Add(new Page144ClassroomExercise02(false, true));       // GTG
            problems.Add(new Page144ClassroomExercise03(false, false));       // GTG
            problems.Add(new Page144ClassroomExercise04(false, false));       // GTG 1:38 execute
            problems.Add(new Page144Problem01(false, true));                 // GTG
            problems.Add(new Page144Problem02(false, false));                 // GTG
            problems.Add(new Page145Problem03(false, false));                 // GTG
            problems.Add(new Page145Problem04(false, false));                 // GTG // Intersection
            problems.Add(new Page145Problem07(false, true));                 // GTG // Intersection
            problems.Add(new Page145Problem08(false, false));                 // GTG // Intersection
            problems.Add(new Page145Problem09(false, true));                 // GTG
            problems.Add(new Page145Problem10(false, false));                 // GTG
            problems.Add(new Page146Problem14(false, false));                 // GTG // Intersection
            problems.Add(new Page146Problem15(false, false));                 // GTG
            //problems.Add(new Page146Problem18(false, false));                 // GTG 1:20 execute
            problems.Add(new Page155Problem14(false, true));                 // GTG
            problems.Add(new Page175ClassroomExercise12(false, true));       // GTG
            problems.Add(new Page223Problem22(false, false));                 // GTG
            problems.Add(new Page223Problem23(false, false));                 // GTG
            problems.Add(new Page242Problem17(false, false));                 // GTG



            ////
            //// Quadrilaterals
            ////
            problems.Add(new Page162Problem20(false, false));          //parallelogram
            problems.Add(new Page166Problem01(false, false));          //parallelogram
            problems.Add(new Page166Problem02(false, false));          //parallelogram
            problems.Add(new Page166Problem03(false, false));          //parallelogram
            problems.Add(new Page166Problem04(false, false));          //parallelogram
            problems.Add(new Page166Problem05(false, false));          //parallelogram
            problems.Add(new Page166Problem13(false, false));          //parallelogram
            problems.Add(new Page170ClassroomExercise02(false, false)); //rhombus
            problems.Add(new Page172Problem19(false, false));          //rhombus
            problems.Add(new Page169Theorem413(false, false));          //rectangle
            problems.Add(new Page171Problem17(false, false));           //rectangle
            //problems.Add(new Page178SelfTest07(false, false));          //rectangle
            problems.Add(new Page173Theorem415(false, false));          //trapezoid
            problems.Add(new Page174Theorem416_1(false, false));          //trapezoid
            problems.Add(new Page174Theorem416_2(false, false));           //trapezoid
            problems.Add(new Page170ClassroomExercise05(false, false));      //square
            //problems.Add(new ExtraSquareProblem(false, false));            //square - very large hypergraph

            //
            //Circles
            //
            problems.Add(new Page296Theorem7_1(false, false));
            problems.Add(new Page296Theorem7_1_Test2(false, false));
            problems.Add(new Page296Theorem7_1_Test3(false, false));
            problems.Add(new Page306Theorem7_4_1(false, false));
            problems.Add(new Page306Theorem7_4_1_Semicircle(false, false));
            problems.Add(new Page309Problem09(false, false));
            problems.Add(new Page307Theorem7_5(false, false));

            //These problems will work, but since they involve both angles and arcs,any theorem which tries to create an equation relating angle measure and 
            //arc measure must first be commented out in the instantiator until the equation issue is resolved:

            //problems.Add(new Page312Theorem7_7_Semicircle(false, false));
            //problems.Add(new Page312Corollary2(false, false));



            //problems.Add(new BackwardPage134Problem7(false, false)); 

            //problems.Add(new Page146Problem12(false, false));                 // How to solve this on paper? A theorem missing perhaps?
            //problems.Add(new Page146Problem17(false, false));                 // MAJOR Encoding issues  
            //problems.Add(new Page147Problem21(false, false));                 // Encoding
            //problems.Add(new Page147Problem22(false, false));                 // Encoding Issues
            //problems.Add(new Page146Problem13(false, false));                 // Endocing

            
            //problems.Add(new Page124Figure31(false, false)); // Classic Isosceles Test

            // problems.Add(new Page60Theorem22Extended(false, false)); Not a real problem
            //problems.Add(new Page147Problem20(false);     // LATER ; omit potentially
            //problems.Add(new Page175ClassroomExercise01to02(false, false));  OMIT
            // problems.Add(new Page175ClassroomExercise03to06(false, false)); OMIT
            //problems.Add(new Page175WrittenExercise1to4(false, false)); // OMIT
            //problems.Add(new Page223Problem24(false, false)); // OMIT can't encode goal
            //problems.Add(new Page223Problem25(false, false)); // OMIT can't encode goal
            // problems.Add(new Page223Problem26(false, false)); OMIT goal encoding
            // problems.Add(new Page223Problem27(false, false)); OMIT goal encoding
            // mislabel problems.Add(new Page223Problem32(false, false));
            //problems.Add(new Page229Problem03(false, false));
            //problems.Add(new Page229Problem05(false, false));
            //problems.Add(new Page229Problem07(false, false));
            //problems.Add(new Page229Problem08(false, false));  
            //problems.Add(new Page229Problem09(false, false)); 
            // problems.Add(new Page242Problem16(false, false)); Given Encoding
            //problems.Add(new Page242Problem21(false, false)); OMIT goal encoding
            //problems.Add(new Page243Problem15(false, false)); OMIT goal encoding
            //problems.Add(new Page243Problem16(false, false)); OMIT given encoding

            return problems;
        }
    }
}
