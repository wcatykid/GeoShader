using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class IsoscelesTrapezoidDefinition : Definition
    {
        private readonly static string NAME = "Definition of Isosceles Trapezoid";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.ISOSCELES_TRAPEZOID_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateCongruent.Clear();
            candidateTrapezoid.Clear();
            candidateStrengthened.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.ISOSCELES_TRAPEZOID_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Trapezoid || clause is Strengthened || clause is CongruentSegments)
            {
                newGrounded.AddRange(InstantiateToIsoscelesTrapezoid(clause));
            }

            if (clause is IsoscelesTrapezoid || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromIsoscelesTrapezoid(clause));
            }

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        // Trapezoid(A, B, C, D) -> Parallel(Segment(A, B), Segment(C, D))
        //
        private static List<EdgeAggregator> InstantiateFromIsoscelesTrapezoid(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is IsoscelesTrapezoid)
            {
                IsoscelesTrapezoid trapezoid  = clause as IsoscelesTrapezoid;

                newGrounded.AddRange(InstantiateFromIsoscelesTrapezoid(trapezoid, trapezoid));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                // Only interested in strenghthened intersection -> perpendicular or -> perpendicular bisector
                if (!(streng.strengthened is IsoscelesTrapezoid)) return newGrounded;

                newGrounded.AddRange(InstantiateFromIsoscelesTrapezoid(streng.strengthened as IsoscelesTrapezoid, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromIsoscelesTrapezoid(Trapezoid trapezoid, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Create the congruent, oppsing side segments
            GeometricCongruentSegments gcs = new GeometricCongruentSegments(trapezoid.leftLeg, trapezoid.rightLeg);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, gcs, annotation));

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Trapezoid(A, B, C, D), CongruentSegments(Segment(A, D), Segment(B, C)) -> IsoscelesTrapezoid(A, B, C, D)
        //
        private static List<Trapezoid> candidateTrapezoid = new List<Trapezoid>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<CongruentSegments> candidateCongruent = new List<CongruentSegments>();
        private static List<EdgeAggregator> InstantiateToIsoscelesTrapezoid(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Trapezoid)
            {
                Trapezoid trapezoid = clause as Trapezoid;

                if (trapezoid is IsoscelesTrapezoid) return newGrounded;

                foreach (CongruentSegments css in candidateCongruent)
                {
                    newGrounded.AddRange(InstantiateToIsoscelesTrapezoid(trapezoid, css, trapezoid));
                }

                candidateTrapezoid.Add(trapezoid);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Trapezoid)) return newGrounded;
                //Don't strengthen an isosceles trapezoid to an isosceles trapezoid
                if (streng.strengthened is IsoscelesTrapezoid) return newGrounded;

                foreach (CongruentSegments css in candidateCongruent)
                {
                    newGrounded.AddRange(InstantiateToIsoscelesTrapezoid(streng.strengthened as Trapezoid, css, streng));
                }

                candidateStrengthened.Add(streng);
            }
            else if (clause is CongruentSegments)
            {
                CongruentSegments newCss = clause as CongruentSegments;

                if (newCss.IsReflexive()) return newGrounded;

                foreach (Trapezoid trapezoid in candidateTrapezoid)
                {
                    newGrounded.AddRange(InstantiateToIsoscelesTrapezoid(trapezoid, newCss, trapezoid));
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    newGrounded.AddRange(InstantiateToIsoscelesTrapezoid(streng.strengthened as Trapezoid, newCss, streng));
                }

                candidateCongruent.Add(newCss);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToIsoscelesTrapezoid(Trapezoid trapezoid, CongruentSegments css, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this paralle set apply to this triangle?
            if (!trapezoid.CreatesCongruentLegs(css)) return newGrounded;

            //
            // Create the new IsoscelesTrapezoid object
            //
            Strengthened newIsoscelesTrapezoid = new Strengthened(trapezoid, new IsoscelesTrapezoid(trapezoid));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(css);

            newGrounded.Add(new EdgeAggregator(antecedent, newIsoscelesTrapezoid, annotation));

            return newGrounded;
        }
    }
}