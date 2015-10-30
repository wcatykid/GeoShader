using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
{
    public class RelationTransitiveSubstitution : GenericRule
    {
        //private static readonly string NAME = "Relation Transitive Substitution";

        // Transitivity of Parallel Lines
        private static List<GeometricParallel> geoParallel = new List<GeometricParallel>();
        private static List<AlgebraicParallel> algParallel = new List<AlgebraicParallel>();

        // Transitvity of Similar Triangles
        private static List<GeometricSimilarTriangles> geoSimilarTriangles = new List<GeometricSimilarTriangles>();
        private static List<AlgebraicSimilarTriangles> algSimilarTriangles = new List<AlgebraicSimilarTriangles>();

        // Resets all saved data.
        public static void Clear()
        {
            geoParallel.Clear();
            algParallel.Clear();

            geoSimilarTriangles.Clear();
            algSimilarTriangles.Clear();
        }

        //
        // Implements transitivity with Relations (Parallel, Similar)
        // Relation(A, B), Relation(B, C) -> Relation(A, C)
        //
        // Generation of new relations is restricted to the following rules; let G be Geometric and A algebriac
        //     G + G -> A
        //     G + A -> A
        //     A + A -X> A  <- Not allowed
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have appropriate clauses?
            if (!(clause is Parallel) && !(clause is SimilarTriangles)) return newGrounded;

            // Has this clause been generated before?
            // Since generated clauses will eventually be instantiated as well, this will reach a fixed point and stop.
            // Uniqueness of clauses needs to be handled by the class calling this
            if (ClauseHasBeenDeduced(clause)) return newGrounded;

            // A reflexive expression provides no information of interest or consequence.
            if (clause.IsReflexive()) return newGrounded;

            //
            // Process the clause
            //
            if (clause is Parallel)
            {
                newGrounded.AddRange(HandleNewParallelRelation(clause as Parallel));
            }
            else if (clause is SimilarTriangles)
            {
                newGrounded.AddRange(HandleNewSimilarTrianglesRelation(clause as SimilarTriangles));
            }

            // Add the new clause to the right list for later combining
            AddToAppropriateList(clause);

            return newGrounded;
        }

        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Parallel Segments
        //
        private static List<EdgeAggregator> HandleNewParallelRelation(Parallel parallel)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricParallel gp in geoParallel)
            {
                newGrounded.AddRange(CreateTransitiveParallelSegments(gp, parallel));
            }

            if (parallel is GeometricParallel)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicParallel ap in algParallel)
                {
                    newGrounded.AddRange(CreateTransitiveParallelSegments(ap, parallel));
                }
            }

            return newGrounded;
        }

        //
        // For generation of transitive Parallel Lines
        //
        private static List<EdgeAggregator> CreateTransitiveParallelSegments(Parallel parallel1, Parallel parallel2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If there is a deduction relationship between the given congruences, do not perform another substitution
            // CTA: remove?
            if (parallel1.HasGeneralPredecessor(parallel2))
            {
                return newGrounded;
            }

            int numSharedExps = parallel1.SharesNumClauses(parallel2);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return Parallel.CreateTransitiveParallel(parallel1, parallel2);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;
                
                default:

                    throw new Exception("Parallel Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }


        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of SimilarTriangles Segments
        //
        private static List<EdgeAggregator> HandleNewSimilarTrianglesRelation(SimilarTriangles simTris)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricSimilarTriangles gsts in geoSimilarTriangles)
            {
                newGrounded.AddRange(CreateTransitiveSimilarTriangles(gsts, simTris));
            }

            if (simTris is GeometricSimilarTriangles)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicSimilarTriangles asts in algSimilarTriangles)
                {
                    newGrounded.AddRange(CreateTransitiveSimilarTriangles(asts, simTris));
                }
            }

            return newGrounded;
        }

        //
        // For generation of transitive SimilarTriangles Lines
        //
        private static List<EdgeAggregator> CreateTransitiveSimilarTriangles(SimilarTriangles simTris1, SimilarTriangles simTris2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // If there is a deduction relationship between the given congruences, do not perform another substitution
            // CTA: remove?
            if (simTris1.HasGeneralPredecessor(simTris2))
            {
                return newGrounded;
            }

            int numSharedExps = simTris1.SharesNumTriangles(simTris2);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                    // Expected case to create a new congruence relationship
                    return SimilarTriangles.CreateTransitiveSimilarTriangles(simTris1, simTris2);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:

                    throw new Exception("Similar Triangles may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static void AddToAppropriateList(GroundedClause c)
        {
            if (c is GeometricParallel)
            {
                geoParallel.Add(c as GeometricParallel);
            }
            else if (c is AlgebraicParallel)
            {
                algParallel.Add(c as AlgebraicParallel);
            }
            else if (c is GeometricSimilarTriangles)
            {
                geoSimilarTriangles.Add(c as GeometricSimilarTriangles);
            }
            else if (c is AlgebraicSimilarTriangles)
            {
                algSimilarTriangles.Add(c as AlgebraicSimilarTriangles);
            }
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static bool ClauseHasBeenDeduced(GroundedClause c)
        {
            if (c is GeometricParallel)
            {
                return geoParallel.Contains(c as GeometricParallel);
            }
            else if (c is AlgebraicParallel)
            {
                return algParallel.Contains(c as AlgebraicParallel);
            }
            else if (c is GeometricSimilarTriangles)
            {
                return geoSimilarTriangles.Contains(c as GeometricSimilarTriangles);
            }
            else if (c is AlgebraicSimilarTriangles)
            {
                return algSimilarTriangles.Contains(c as AlgebraicSimilarTriangles);
            }

            return false;
        }
    }
}