using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;

namespace GeometryTutorLib.ConcreteAST
{
    public abstract class Constraint
    {
        public Constraint()
        {
        }

        //
        // Turn all the equations into constraints.
        //
        public static List<Constraint> MakeEquationsIntoConstraints(List<Equation> eqs)
        {
            List<Constraint> constraints = new List<Constraint>();

            foreach (Equation eq in eqs)
            {
                constraints.Add(new EquationConstraint(eq));
            }

            return constraints;
        }

        //
        // Turn all the equations into constraints.
        //
        public static List<Constraint> MakeCongruencesIntoConstraints(List<Congruent> congruences)
        {
            List<Constraint> constraints = new List<Constraint>();

            foreach (Congruent congruence in congruences)
            {
                constraints.Add(new CongruenceConstraint(congruence));
            }

            return constraints;
        }
    }

    //
    // Basic equations like those implemented in GeoTutor.
    //
    public class EquationConstraint : Constraint
    {
        public Equation eqConstraint { get; protected set; }

        public EquationConstraint(Equation eq) : base()
        {
            eqConstraint = eq;
        }

        public override string ToString()
        {
            return eqConstraint.ToString();
        }
    }

    //
    // Congruences like those implemented in GeoTutor.
    //
    public class CongruenceConstraint : Constraint
    {
        public Congruent conConstraint { get; protected set; }

        public CongruenceConstraint(Congruent congruence) : base()
        {
            conConstraint = congruence;
        }

        public override string ToString() { return conConstraint.ToString(); }
    }

    //
    // Handle squaring and square roots; we just keep the figure itself: right triangle, isosceles trapezoid
    //
    public class FigureConstraint : Constraint
    {
        public Figure figConstraint { get; protected set; }

        public FigureConstraint(Figure fig) : base()
        {
            figConstraint = fig;
        }

        public override string ToString() { return figConstraint.ToString(); }
    }
}