using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TrapezoidDefinition : Definition
    {
        private readonly static string NAME = "Definition of Trapezoid";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRAPEZOID_DEFINITION);

        // Reset saved data for another problem
        public static void Clear()
        {
            candidateParallel.Clear();
            candidateQuadrilateral.Clear();
        }

        //
        // This implements forward and Backward instantiation
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TRAPEZOID_DEFINITION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral || clause is Parallel)
            {
                newGrounded.AddRange(InstantiateToTrapezoid(clause));
            }

            else if (clause is Trapezoid || clause is Strengthened)
            {
                newGrounded.AddRange(InstantiateFromTrapezoid(clause));
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
        private static List<EdgeAggregator> InstantiateFromTrapezoid(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Trapezoid)
            {
                Trapezoid trapezoid  = clause as Trapezoid;

                newGrounded.AddRange(InstantiateFromTrapezoid(trapezoid, trapezoid));
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Trapezoid)) return newGrounded;

                newGrounded.AddRange(InstantiateFromTrapezoid(streng.strengthened as Trapezoid, streng));
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateFromTrapezoid(Trapezoid trapezoid, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Determine the parallel opposing sides and output that.
            //
            GeometricParallel newParallel = new GeometricParallel(trapezoid.baseSegment, trapezoid.oppBaseSegment);

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);

            newGrounded.Add(new EdgeAggregator(antecedent, newParallel, annotation));

            return newGrounded;
        }

        //     A __________ B
        //      /          \
        //     /            \
        //    /              \
        // D /________________\ C
        //
        //
        // Quadrilateral(A, B, C, D), Parallel(Segment(A, B), Segment(C, D)) -> Trapezoid(A, B, C, D)
        //
        private static List<Quadrilateral> candidateQuadrilateral = new List<Quadrilateral>();
        private static List<Parallel> candidateParallel = new List<Parallel>();
        private static List<EdgeAggregator> InstantiateToTrapezoid(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Quadrilateral)
            {
                Quadrilateral quad = clause as Quadrilateral;

                if (!quad.IsStrictQuadrilateral()) return newGrounded;

                foreach (Parallel parallel in candidateParallel)
                {
                    newGrounded.AddRange(InstantiateToTrapezoid(quad, parallel));
                }

                candidateQuadrilateral.Add(quad);
            }
            else if (clause is Parallel)
            {
                Parallel parallel = clause as Parallel;

                foreach (Quadrilateral quad in candidateQuadrilateral)
                {
                    newGrounded.AddRange(InstantiateToTrapezoid(quad, parallel));
                }

                candidateParallel.Add(parallel);
            }


            return newGrounded;
        }

        private static List<EdgeAggregator> InstantiateToTrapezoid(Quadrilateral quad, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this parallel set apply to this quadrilateral?
            if (!quad.HasOppositeParallelSides(parallel)) return InstantiateToTrapezoidSubsegments(quad, parallel);

            //
            // The other set of sides should NOT be parallel (that's a parallelogram)
            //
            List<Segment> otherSides = quad.GetOtherSides(parallel);

            if (otherSides.Count != 2)
            {
                throw new ArgumentException("Expected TWO sides returned from a quadrilateral / parallel relationship: " + quad + " " + parallel);
            }

            if (otherSides[0].IsParallelWith(otherSides[1])) return newGrounded;

            return MakeTrapezoid(quad, parallel);
        }

        //
        // Are the parallel sides subsegments of sides of the quadrilateral?
        // If so, instantiate.
        //
        private static List<EdgeAggregator> InstantiateToTrapezoidSubsegments(Quadrilateral quad, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Does this parallel set apply to this quadrilateral?
            if (!quad.HasOppositeParallelSubsegmentSides(parallel)) return newGrounded;

            //
            // The other set of sides should NOT be parallel (that's a parallelogram)
            //
            List<Segment> otherSides = quad.GetOtherSubsegmentSides(parallel);

            if (otherSides.Count != 2)
            {
                throw new ArgumentException("Expected TWO sides returned from a quadrilateral / parallel relationship: "
                                            + quad + " " + parallel + " returned " + otherSides.Count);
            }

            if (otherSides[0].IsParallelWith(otherSides[1])) return newGrounded;

            return MakeTrapezoid(quad, parallel);
        }

        private static List<EdgeAggregator> MakeTrapezoid(Quadrilateral quad, Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //
            // Create the new Trapezoid object
            //
            Strengthened newTrapezoid = new Strengthened(quad, new Trapezoid(quad));

            // For hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(quad);
            antecedent.Add(parallel);

            newGrounded.Add(new EdgeAggregator(antecedent, newTrapezoid, annotation));

            return newGrounded;
        }
    }
}