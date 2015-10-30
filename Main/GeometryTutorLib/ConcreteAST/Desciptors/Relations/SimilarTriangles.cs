using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometryTutorLib.ConcreteAST
{
    public class SimilarTriangles : Descriptor
    {
        public Triangle st1 { get; private set; }
        public Triangle st2 { get; private set; }

        public SimilarTriangles(Triangle t1, Triangle t2) : base()
        {
            st1 = t1;
            st2 = t2;
        }

        public override bool IsReflexive() { return st1.StructurallyEquals(st2); }

        public Triangle OtherTriangle(Triangle that)
        {
            if (st1.Equals(that)) return st2;
            if (st2.Equals(that)) return st1;

            return null;
        }

        public Triangle SharedTriangle(SimilarTriangles sts)
        {
            if (st1.StructurallyEquals(sts.st1) && st1.StructurallyEquals(sts.st2)) return st1;
            if (st2.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2)) return st2;

            return null;
        }

        public int SharesNumTriangles(SimilarTriangles sts)
        {
            int shared = st1.StructurallyEquals(sts.st1) && st1.StructurallyEquals(sts.st2) ? 1 : 0;
            shared += st2.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2) ? 1 : 0;

            return shared;
        }

        public override bool StructurallyEquals(Object c)
        {
            SimilarTriangles sts = c as SimilarTriangles;
            if (sts == null) return false;
            return st1.StructurallyEquals(sts.st1) && st2.StructurallyEquals(sts.st2) ||
                   st1.StructurallyEquals(sts.st2) && st2.StructurallyEquals(sts.st1);
        }

        public override bool Equals(Object c)
        {
            SimilarTriangles sts = c as SimilarTriangles;
            if (sts == null) return false;
            return (st1.Equals(sts.st1) && st2.Equals(sts.st2)) || (st1.Equals(sts.st2) && st2.Equals(sts.st1));
        }

        public override int GetHashCode() { return base.GetHashCode(); }



        private static readonly string ANGLES_NAME = "Angles of Similar Triangles are Congruent";
        private static readonly string SEGMENTS_NAME = "Segments of Similar Triangles are Proportional";

        private static Hypergraph.EdgeAnnotation angleAnnotation = new Hypergraph.EdgeAnnotation(ANGLES_NAME, EngineUIBridge.JustificationSwitch.SIMILARITY);
        private static Hypergraph.EdgeAnnotation segmentAnnotation = new Hypergraph.EdgeAnnotation(SEGMENTS_NAME, EngineUIBridge.JustificationSwitch.SIMILARITY);
        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        private static List<GenericInstantiator.EdgeAggregator> GenerateSegmentRatio(SimilarTriangles simTris,
                                                                                     List<Point> orderedTriOnePts,
                                                                                     List<Point> orderedTriTwoPts)
        {
            segmentAnnotation.active = EngineUIBridge.JustificationSwitch.SIMILARITY;

            //
            // Cycle through the points creating the angles: ABC - DEF ; BCA - EFD ; CAB - FDE
            //
            List<SegmentRatio> ratios = new List<SegmentRatio>();
            for (int i = 0; i < orderedTriOnePts.Count; i++)
            {
                Segment cs1 = new Segment(orderedTriOnePts[0], orderedTriOnePts[1]);
                Segment cs2 = new Segment(orderedTriTwoPts[0], orderedTriTwoPts[1]);
                SegmentRatio ratio = new SegmentRatio(cs1, cs2);

                ratios.Add(ratio);

                // rotate the lists
                Point tmp = orderedTriOnePts.ElementAt(0);
                orderedTriOnePts.RemoveAt(0);
                orderedTriOnePts.Add(tmp);

                tmp = orderedTriTwoPts.ElementAt(0);
                orderedTriTwoPts.RemoveAt(0);
                orderedTriTwoPts.Add(tmp);
            }

            //
            // Take the ratios and create ratio equations.
            //
            List<GroundedClause> ratioEqs = new List<GroundedClause>();
            for (int i = 0; i < ratios.Count; i++)
            {
                ratioEqs.Add(new GeometricSegmentRatioEquation(ratios[i], ratios[(i + 1) % ratios.Count]));
            }

            //
            // Construct the new deduced edges: proportional segments.
            //
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(simTris);
            foreach (GroundedClause eq in ratioEqs)
            {
                newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, eq, segmentAnnotation));
            }

            return newGrounded;
        }

        //
        // Create the three resultant angles from each triangle to create the congruency of angles
        //
        public static List<GenericInstantiator.EdgeAggregator> GenerateCongruentAngles(SimilarTriangles simTris,
                                                                                       List<Point> orderedTriOnePts,
                                                                                       List<Point> orderedTriTwoPts)
        {
            angleAnnotation.active = EngineUIBridge.JustificationSwitch.SIMILARITY;

            List<GroundedClause> congAngles = CongruentTriangles.GenerateCPCTCAngles(orderedTriOnePts, orderedTriTwoPts);

            //
            // Construct the new deduced edges: congruent angles.
            //
            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();
            List<GroundedClause> antecedent = Utilities.MakeList<GroundedClause>(simTris);
            foreach (GeometricCongruentAngles ccas in congAngles)
            {
                newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, ccas, angleAnnotation));
            }

            return newGrounded;
        }

        public static List<GenericInstantiator.EdgeAggregator> GenerateComponents(SimilarTriangles simTris,
                                                                                  List<Point> orderedTriOnePts,
                                                                                  List<Point> orderedTriTwoPts)
        {
            List<GenericInstantiator.EdgeAggregator> angles = GenerateCongruentAngles(simTris, orderedTriOnePts, orderedTriTwoPts);
            List<GenericInstantiator.EdgeAggregator> segments = GenerateSegmentRatio(simTris, orderedTriOnePts, orderedTriTwoPts);
            angles.AddRange(segments);
 
            return angles;
        }

        private static readonly string NAME = "Transitivity";
        private static Hypergraph.EdgeAnnotation transAnnotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_SIMILAR);

        public static List<GenericInstantiator.EdgeAggregator> CreateTransitiveSimilarTriangles(SimilarTriangles simTris1, SimilarTriangles simTris2)
        {
            transAnnotation.active = EngineUIBridge.JustificationSwitch.TRANSITIVE_SIMILAR;

            List<GenericInstantiator.EdgeAggregator> newGrounded = new List<GenericInstantiator.EdgeAggregator>();

            // Did either of these congruences come from the other?
            // CTA: We don't need this anymore since use is restricted by class TransitiveSubstitution
            //if (simTris1.HasRelationPredecessor(simTris2) || simTris2.HasRelationPredecessor(simTris1)) return newGrounded;

            // Create the antecedent clauses
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(simTris1);
            antecedent.Add(simTris2);

            // Create the consequent clause
            Triangle shared = simTris1.SharedTriangle(simTris2);

            AlgebraicSimilarTriangles newAP = new AlgebraicSimilarTriangles(simTris1.OtherTriangle(shared), simTris2.OtherTriangle(shared));

            newGrounded.Add(new GenericInstantiator.EdgeAggregator(antecedent, newAP, transAnnotation));

            return newGrounded;
        }

        public override string ToString() { return "Similar(" + st1.ToString() + ", " + st2.ToString() + ") " + justification; }
    }
}