using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;


namespace GeometryTutorLib.GenericInstantiator
{
    public class TransversalPerpendicularToParallelImplyBothPerpendicular : Theorem
    {
        private readonly static string NAME = "If a transversal is perpendicular to one of two parallel lines, then it is perpendicular to the other one also.";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR);

        private static List<Perpendicular> candidatePerpendicular = new List<Perpendicular>();
        private static List<Strengthened> candidateStrengthened = new List<Strengthened>();
        private static List<Intersection> candidateIntersection = new List<Intersection>();
        private static List<Parallel> candidateParallel = new List<Parallel>();

        // Resets all saved data.
        public static void Clear()
        {
            candidateIntersection.Clear();
            candidateParallel.Clear();
            candidatePerpendicular.Clear();
            candidateStrengthened.Clear();
        }

        //
        // Perpendicular(Segment(E, F), Segment(C, D)),
        // Parallel(Segment(E, F), Segment(A, B)),
        // Intersection(M, Segment(A, B), Segment(C, D)) -> Perpendicular(Segment(A, B), Segment(C, D)) 
        //
        //                                   E       B
        //                                   |       |
        //                              C----|-------|--------D
        //                                   | N     | M
        //                                   |       |
        //                                   F       A
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            if (clause is Parallel)
            {
                Parallel newParallel = clause as Parallel;

                foreach (Perpendicular perp in candidatePerpendicular)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(perp, newParallel, inter, perp));
                    }
                }

                foreach (Strengthened streng in candidateStrengthened)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(streng.strengthened as Perpendicular, newParallel, inter, streng));
                    }
                }

                candidateParallel.Add(newParallel);
            }
            else if (clause is Perpendicular)
            {
                Perpendicular newPerp = clause as Perpendicular;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(newPerp, parallel, inter, newPerp));
                    }
                }

                candidatePerpendicular.Add(newPerp);
            }
            else if (clause is Intersection)
            {
                Intersection newIntersection = clause as Intersection;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Perpendicular perp in candidatePerpendicular)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(perp, parallel, newIntersection, perp));
                    }
                }

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Strengthened streng in candidateStrengthened)
                    {
                        newGrounded.AddRange(CheckAndGeneratePerpendicular(streng.strengthened as Perpendicular, parallel, newIntersection, streng));
                    }
                }

                candidateIntersection.Add(newIntersection);
            }
            else if (clause is Strengthened)
            {
                Strengthened streng = clause as Strengthened;

                if (!(streng.strengthened is Perpendicular)) return newGrounded;

                foreach (Parallel parallel in candidateParallel)
                {
                    foreach (Intersection inter in candidateIntersection)
                    {
                        if (!inter.Equals(streng.original))
                        {
                            newGrounded.AddRange(CheckAndGeneratePerpendicular(streng.strengthened as Perpendicular, parallel, inter, streng));
                        }
                    }
                }

                candidateStrengthened.Add(streng);
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CheckAndGeneratePerpendicular(Perpendicular perp, Parallel parallel, Intersection inter, GroundedClause original)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // The perpendicular intersection must refer to one of the parallel segments
            Segment shared = perp.CommonSegment(parallel);
            if (shared == null) return newGrounded;

            // The other intersection must refer to a segment in the parallel pair
            Segment otherShared = inter.CommonSegment(parallel);
            if (otherShared == null) return newGrounded;

            // The two shared segments must be distinct
            if (shared.Equals(otherShared)) return newGrounded;

            // Transversals must align
            if (!inter.OtherSegment(otherShared).Equals(perp.OtherSegment(shared))) return newGrounded;

            // Strengthen the old intersection to be perpendicular
            Strengthened strengthenedPerp = new Strengthened(inter, new Perpendicular(inter));

            // Construct hyperedge
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(original);
            antecedent.Add(parallel);
            antecedent.Add(inter);

            newGrounded.Add(new EdgeAggregator(antecedent, strengthenedPerp, annotation));

            return newGrounded;
        }
    }
}