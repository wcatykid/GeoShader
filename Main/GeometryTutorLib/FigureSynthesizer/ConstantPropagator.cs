using System;
using System.Collections.Generic;
using System.Linq;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.Area_Based_Analyses
{
    /// <summary>
    /// A calculator for aggregator of all known measurements for a particular problem (angles, segment lengths, etc.)
    /// </summary>
    public static class ConstantPropagator
    {
        public static KnownMeasurementsAggregator Propogate(KnownMeasurementsAggregator known, List<Constraint> constraints)
        {
            List<GroundedClause> congruences = new List<GroundedClause>();
            List<GroundedClause> equations = new List<GroundedClause>();
            List<Figure> figures = new List<Figure>();

            foreach (Constraint constraint in constraints)
            {
                if (constraint is CongruenceConstraint) congruences.Add((constraint as CongruenceConstraint).conConstraint);
                if (constraint is EquationConstraint) equations.Add((constraint as EquationConstraint).eqConstraint);
                if (constraint is FigureConstraint) figures.Add((constraint as FigureConstraint).figConstraint);
            }

            //
            // Fixed-point acquisition of values using congruences and equations.
            //
            bool change = true;
            while(change)
            {
                change = KnownValueAcquisition.AcquireCongruences(known, congruences);

                // Right Triangles, Isosceles Triangles, Isosceles Trapezoids
                change = AcquireViaFigures(known, figures) || change;

                change = KnownValueAcquisition.AcquireViaEquations(known, equations) || change;
            }

            return known;
        }

        private static bool AcquireViaFigures(KnownMeasurementsAggregator known, List<Figure> figures)
        {
            //
            // Split into the types of figures.
            //
            List<Triangle> triangles = new List<Triangle>();
            List<IsoscelesTrapezoid> isoTraps = new List<IsoscelesTrapezoid>();

            foreach (Figure fig in figures)
            {
                if (fig is Triangle) triangles.Add(fig as Triangle);
                if (fig is IsoscelesTrapezoid) isoTraps.Add(fig as IsoscelesTrapezoid);
            }

            //
            // Process each type of figure.
            //
            bool addedKnown = false;
            
            foreach (Triangle tri in triangles)
            {
                addedKnown = KnownValueAcquisition.HandleTriangle(known, tri) || addedKnown;
            }

            foreach (IsoscelesTrapezoid isoTrap in isoTraps)
            {
                HandleIsoscelesTrapezoid(known, isoTrap);
            }

            return addedKnown;
        }

        //
        // An isosceles trapezoid can use an equation to find the height based on the bases and the side lengths.
        //
        public static void HandleIsoscelesTrapezoid(KnownMeasurementsAggregator known, IsoscelesTrapezoid isoTrap)
        {
            isoTrap.CalculateHeight(known);
        }
    }
}
