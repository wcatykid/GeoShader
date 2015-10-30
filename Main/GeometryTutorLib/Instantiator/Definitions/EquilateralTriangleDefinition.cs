using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class EquilateralTriangleDefinition : Definition
    {
        private readonly static string NAME = "Definition of Equilateral Triangle";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_DEFINITION);

        private static List<CongruentSegments> candCongruent = new List<CongruentSegments>();
        private static List<Triangle> candTriangles = new List<Triangle>();

        // Resets all saved data.
        public static void Clear()
        {
            candCongruent.Clear();
            candTriangles.Clear();
        }

        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_DEFINITION;

            if (clause is EquilateralTriangle || clause is Strengthened)
            {
                return InstantiateFromDefinition(clause);
            }

            if (clause is Triangle || clause is CongruentSegments)
            {
                return InstantiateToDefinition(clause);
            }

            return new List<EdgeAggregator>();
        }

        private static List<EdgeAggregator> InstantiateFromDefinition(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is EquilateralTriangle)
            {
                EquilateralTriangle tri = clause as EquilateralTriangle;

                return InstantiateFromDefinition(tri, clause);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is EquilateralTriangle)) return newGrounded;

                return InstantiateFromDefinition(streng.strengthened as EquilateralTriangle, clause);
            }

            return newGrounded;
        }

        //
        // Generate the three pairs of congruent segments.
        //
        private static List<EdgeAggregator> InstantiateFromDefinition(EquilateralTriangle tri, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            //
            // Create the 3 sets of congruent segments.
            //
            for (int s = 0; s < tri.orderedSides.Count; s++)
            {
                GeometricCongruentSegments gcs = new GeometricCongruentSegments(tri.orderedSides[s], tri.orderedSides[(s+1) % tri.orderedSides.Count]);

                newGrounded.Add(new EdgeAggregator(antecedent, gcs, annotation));
            }

            //
            // Create the 3 congruent angles.
            //
            for (int a = 0; a < tri.angles.Count; a++)
            {
                GeometricCongruentAngles gcas = new GeometricCongruentAngles(tri.angles[a], tri.angles[(a + 1) % tri.angles.Count]);

                newGrounded.Add(new EdgeAggregator(antecedent, gcas, annotation));
            }

            //
            // Create the 3 equations for the measure of each angle being 60 degrees.
            //
            for (int a = 0; a < tri.angles.Count; a++)
            {
                GeometricAngleEquation gae = new GeometricAngleEquation(tri.angles[a], new NumericValue(60));

                newGrounded.Add(new EdgeAggregator(antecedent, gae, annotation));
            }

            return newGrounded;
        }


        private static List<EdgeAggregator> InstantiateToDefinition(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Triangle)
            {
                // Avoid strengthening an equilateral triangle to an equilateral triangle.
                if (clause is EquilateralTriangle || (clause as Triangle).provenEquilateral) return newGrounded;

                candTriangles.Add(clause as Triangle);
            }
            else if (clause is CongruentSegments)
            {
                CongruentSegments cs = clause as CongruentSegments;

                foreach (Triangle tri in candTriangles)
                {
                    for (int s1 = 0; s1 < candCongruent.Count - 1; s1++)
                    {
                        for (int s2 = s1 + 1; s2 < candCongruent.Count; s2++)
                        {
                            newGrounded.AddRange(InstantiateToDefinition(tri, candCongruent[s1], candCongruent[s2]));
                        }
                    }
                }

                candCongruent.Add(cs);
            }
            
            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToDefinition(Triangle tri, CongruentSegments cs1, CongruentSegments cs2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do these congruences relate one common segment?
            Segment shared = cs1.SharedSegment(cs2);

            if (shared == null) return newGrounded;

            //
            // Do these congruences apply to this triangle?
            //
            if (!tri.HasSegment(cs1.cs1) || !tri.HasSegment(cs1.cs2)) return newGrounded;
            if (!tri.HasSegment(cs2.cs1) || !tri.HasSegment(cs2.cs2)) return newGrounded;

            //
            // These cannot be reflexive congruences.
            //
            if (cs1.IsReflexive() || cs2.IsReflexive()) return newGrounded;

            //
            // Are the non-shared segments unique?
            //
            Segment other1 = cs1.OtherSegment(shared);
            Segment other2 = cs2.OtherSegment(shared);

            if (other1.StructurallyEquals(other2)) return newGrounded;

            //
            // Generate the new equialteral clause
            //
            Strengthened newStrengthened = new Strengthened(tri, new EquilateralTriangle(tri));

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(cs1);
            antecedent.Add(cs2);
            antecedent.Add(tri);

            newGrounded.Add(new EdgeAggregator(antecedent, newStrengthened, annotation));

            return newGrounded;
        }
    }
}