using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometryTutorLib.ConcreteAST;
using System.Diagnostics;

namespace GeometryTutorLib.GenericInstantiator
{
    public class TransitiveSubstitution : GenericRule
    {
        private static readonly string NAME = "Transitive Substitution";
        private static Hypergraph.EdgeAnnotation annotation = new Hypergraph.EdgeAnnotation(NAME, EngineUIBridge.JustificationSwitch.TRANSITIVE_SUBSTITUTION);

#region Variable Declarations
        // Congruences imply equations: AB \cong CD -> AB = CD
        private static List<GeometricCongruentSegments> geoCongSegments = new List<GeometricCongruentSegments>();
        private static List<GeometricCongruentAngles> geoCongAngles = new List<GeometricCongruentAngles>();
        private static List<GeometricCongruentArcs> geoCongArcs = new List<GeometricCongruentArcs>();

        // These are transitively deduced congruences
        private static List<AlgebraicCongruentSegments> algCongSegments = new List<AlgebraicCongruentSegments>();
        private static List<AlgebraicCongruentAngles> algCongAngles = new List<AlgebraicCongruentAngles>();
        private static List<AlgebraicCongruentArcs> algCongArcs = new List<AlgebraicCongruentArcs>();

        // Old segment equations
        private static List<GeometricSegmentEquation> geoSegmentEqs = new List<GeometricSegmentEquation>();
        private static List<AlgebraicSegmentEquation> algSegmentEqs = new List<AlgebraicSegmentEquation>();

        // Old angle measure equations
        private static List<AlgebraicAngleEquation> algAngleEqs = new List<AlgebraicAngleEquation>();
        private static List<GeometricAngleEquation> geoAngleEqs = new List<GeometricAngleEquation>();

        // Old arc equations
        private static List<GeometricArcEquation> geoArcEqs = new List<GeometricArcEquation>();
        private static List<AlgebraicArcEquation> algArcEqs = new List<AlgebraicArcEquation>();

        // Old angle-arc equation
        private static List<GeometricAngleArcEquation> geoAngleArcEqs = new List<GeometricAngleArcEquation>();
        private static List<AlgebraicAngleArcEquation> algAngleArcEqs = new List<AlgebraicAngleArcEquation>();

        // Old Proportional Segment Expressions
        private static List<SegmentRatio> propSegs = new List<SegmentRatio>();

        // Old Proportional Angle Expressions
        private static List<GeometricProportionalAngles> geoPropAngs = new List<GeometricProportionalAngles>();
        private static List<AlgebraicProportionalAngles> algPropAngs = new List<AlgebraicProportionalAngles>();

        // For cosntruction of the new equations
        private static readonly int SEGMENT_EQUATION = 0;
        private static readonly int ANGLE_EQUATION = 1;
        private static readonly int ARC_EQUATION = 2;
        private static readonly int ANGLE_ARC_EQUATION = 3;
        private static readonly int EQUATION_ERROR = 1;
#endregion

#region Instantiation and Clearing
        // Resets all saved data.
        public static void Clear()
        {
            geoCongSegments.Clear();
            geoCongAngles.Clear();
            geoCongArcs.Clear();

            algCongSegments.Clear();
            algCongAngles.Clear();
            algCongArcs.Clear();

            geoSegmentEqs.Clear();
            algSegmentEqs.Clear();

            algAngleEqs.Clear();
            geoAngleEqs.Clear();

            algArcEqs.Clear();
            geoArcEqs.Clear();

            algAngleArcEqs.Clear();
            geoAngleArcEqs.Clear();

            propSegs.Clear();

            geoPropAngs.Clear();
            algPropAngs.Clear();
        }

        //
        // Implements transitivity with equations
        // Equation(A, B), Equation(B, C) -> Equation(A, C)
        //
        // This includes CongruentSegments and CongruentAngles
        //
        // Generation of new equations is restricted to the following rules; let G be Geometric and A algebriac
        //     G + G -> A
        //     G + A -> A
        //     A + A -X> A  <- Not allowed
        //
        public static List<EdgeAggregator> Instantiate(GroundedClause clause)
        {
            annotation.active = EngineUIBridge.JustificationSwitch.TRANSITIVE_SUBSTITUTION;

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // Do we have an equation or congruence?
            if (!(clause is Equation) && !(clause is Congruent) && !(clause is SegmentRatio)) return newGrounded;

            // Has this clause been generated before?
            // Since generated clauses will eventually be instantiated as well, this will reach a fixed point and stop.
            // Uniqueness of clauses needs to be handled by the class calling this
            if (ClauseHasBeenDeduced(clause)) return newGrounded;

            // A reflexive expression provides no information of interest or consequence.
            if (clause.IsReflexive()) return newGrounded;

            //
            // Process the clause
            //
            if (clause is SegmentEquation)
            {
                newGrounded.AddRange(HandleNewSegmentEquation(clause as SegmentEquation));
            }
            else if (clause is AngleEquation)
            {
                newGrounded.AddRange(HandleNewAngleEquation(clause as AngleEquation));
            }
            else if (clause is ArcEquation)
            {
                newGrounded.AddRange(HandleNewArcEquation(clause as ArcEquation));
            }
            else if (clause is AngleArcEquation)
            {
                newGrounded.AddRange(HandleNewAngleArcEquation(clause as AngleArcEquation));
            }
            else if (clause is CongruentAngles)
            {
                newGrounded.AddRange(HandleNewCongruentAngles(clause as CongruentAngles));
            }
            else if (clause is CongruentSegments)
            {
                newGrounded.AddRange(HandleNewCongruentSegments(clause as CongruentSegments));
            }
            else if (clause is CongruentArcs)
            {
                newGrounded.AddRange(HandleNewCongruentArcs(clause as CongruentArcs));
            }
            else if (clause is SegmentRatio)
            {
                SegmentRatio ratio = clause as SegmentRatio;
                if (!ratio.ProportionValueKnown()) return newGrounded;

                // Avoid using proportional segments that should really be congruent (they are deduced from similar triangles which are, in fact, congruent)
                if (Utilities.CompareValues((clause as SegmentRatio).dictatedProportion, 1)) return newGrounded;

                newGrounded.AddRange(HandleNewSegmentRatio(clause as SegmentRatio));
            }

            // Add the new clause to the right list for later combining
            AddToAppropriateList(clause);

            // Add predecessors
            MarkPredecessors(newGrounded);

            return newGrounded;
        }
#endregion

#region Segments
        #region Congruency Handling
        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewCongruentSegments(CongruentSegments congSegs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentSegments gcss in geoCongSegments)
            {
                newGrounded.AddRange(CreateCongruentSegments(gcss, congSegs));
            }

            // New equations? G + G -> A
            foreach (GeometricSegmentEquation gseqs in geoSegmentEqs)
            {
                newGrounded.AddRange(CreateSegmentEquation(gseqs, congSegs));
            }

            // New proportions? G + G -> A
            foreach (SegmentRatio ps in propSegs)
            {
                newGrounded.AddRange(CreateSegmentRatio(ps, congSegs));
            }

            if (congSegs is GeometricCongruentSegments)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentSegments acss in algCongSegments)
                {
                    newGrounded.AddRange(CreateCongruentSegments(acss, congSegs));
                }

                // New equations? G + A -> A
                foreach (AlgebraicSegmentEquation aseqs in algSegmentEqs)
                {
                    newGrounded.AddRange(CreateSegmentEquation(aseqs, congSegs));
                }

                // New proportions? G + A -> A
                //foreach (AlgebraicSegmentRatio aps in algPropSegs)
                //{
                //    newGrounded.AddRange(CreateSegmentRatio(aps, congSegs));
                //}
            }

            //
            // NEW
            //
            else if (congSegs is AlgebraicCongruentSegments)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentSegments oldACSS in algCongSegments)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateCongruentSegments(oldACSS, congSegs)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicSegmentEquation aseqs in algSegmentEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentEquation(aseqs, congSegs)));
                }

                // New proportions? A + A -> A
                //foreach (AlgebraicSegmentRatio aps in algPropSegs)
                //{
                //    newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentRatio(aps, congSegs)));
                //}
            }

            return newGrounded;
        }
        //
        // For generation of transitive congruent segments
        //
        private static List<EdgeAggregator> CreateCongruentSegments(CongruentSegments css, CongruentSegments geoCongSeg)
        {
            int numSharedExps = css.SharesNumClauses(geoCongSeg);
            return CreateCongruent<CongruentSegments>(css, geoCongSeg, numSharedExps);
        }
        #endregion

        #region Ratio Handling
        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewSegmentRatio(SegmentRatio newRatio)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            foreach (SegmentRatio old in propSegs)
            {
                newGrounded.AddRange(SegmentRatio.CreateProportionEquation(old, newRatio));
            }

            // New transitivity? G + G -> A
            //foreach (GeometricCongruentSegments gcs in geoCongSegments)
            //{
            //    newGrounded.AddRange(CreateSegmentRatio(propSegs, gcs));
            //}

            //if (propSegs is SegmentRatio)
            //{
            //    // New transitivity? G + A -> A
            //    foreach (AlgebraicCongruentSegments acs in algCongSegments)
            //    {
            //        newGrounded.AddRange(CreateSegmentRatio(propSegs, acs));
            //    }
            //}

            //else if (propSegs is AlgebraicSegmentRatio)
            //{
            //    // New transitivity? A + A -> A
            //    foreach (AlgebraicCongruentSegments acs in algCongSegments)
            //    {
            //        newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentRatio(propSegs, acs)));
            //    }
            //}

            return newGrounded;
        }

        //
        // For generation of transitive proportional segments
        //
        private static List<EdgeAggregator> CreateSegmentRatio(SegmentRatio pss, CongruentSegments conSeg)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            int numSharedExps = pss.SharesNumClauses(conSeg);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                // Expected case to create a new congruence relationship
                //return SegmentRatio.CreateTransitiveProportion(pss, conSeg);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:
                    throw new Exception("Proportional / Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }
        #endregion

        #region Equation Handling
        //
        // Generate all new relationships from an Equation Containing Segments
        //
        private static List<EdgeAggregator> HandleNewSegmentEquation(SegmentEquation newSegEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentSegments gcss in geoCongSegments)
            {
                newGrounded.AddRange(CreateSegmentEquation(newSegEq, gcss));
            }

            // New equations? G + G -> A
            foreach (GeometricSegmentEquation gSegs in geoSegmentEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gSegs, newSegEq));
            }

            if (newSegEq is GeometricSegmentEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentSegments acss in algCongSegments)
                {
                    newGrounded.AddRange(CreateSegmentEquation(newSegEq, acss));
                }

                // New equations? G + A -> A
                foreach (AlgebraicSegmentEquation aSegs in algSegmentEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aSegs, newSegEq));
                }
            }

            //
            // NEW
            //
            else if (newSegEq is AlgebraicSegmentEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentSegments oldACSS in algCongSegments)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateSegmentEquation(newSegEq, oldACSS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicSegmentEquation aSegs in algSegmentEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aSegs, newSegEq)));
                }
            }


            //
            // Combining TWO algebraic equations only if the result is a congruence: A + A -> Congruent
            //
            //else if (newSegEq is AlgebraicSegmentEquation)
            //{
            //    foreach (AlgebraicSegmentEquation asegs in algSegmentEqs)
            //    {
            //        List<KeyValuePair<List<GroundedClause>, GroundedClause>> newEquationList = CreateNewEquation(newSegEq, asegs);
            //        if (newEquationList.Any())
            //        {
            //            KeyValuePair<List<GroundedClause>, GroundedClause> newEq = newEquationList[0];

            //            if (newEq.Value is AlgebraicCongruentSegments)
            //            {
            //                newGrounded.AddRange(newEquationList);
            //            }
            //        }
            //    }
            //}
            return newGrounded;
        }

        //
        // Substitute this new segment congruence into old segment equations
        //
        private static List<EdgeAggregator> CreateSegmentEquation(SegmentEquation segEq, CongruentSegments congSeg)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

            newEquationEdge = PerformEquationSubstitution(segEq, congSeg, congSeg.cs1, congSeg.cs2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(segEq, congSeg, congSeg.cs2, congSeg.cs1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }
        #endregion
#endregion

#region Angles
        #region Congruency Handling
        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Angles
        //
        private static List<EdgeAggregator> HandleNewCongruentAngles(CongruentAngles congAngs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcss in geoCongAngles)
            {
                newGrounded.AddRange(CreateCongruentAngles(gcss, congAngs));
            }

            // New equations? G + G -> A
            foreach (GeometricAngleEquation gseqs in geoAngleEqs)
            {
                newGrounded.AddRange(CreateAngleEquation(gseqs, congAngs));
            }

            foreach (GeometricAngleArcEquation gseqs in geoAngleArcEqs)
            {
                newGrounded.AddRange(CreateAngleArcEquationFromAngleSubstitution(gseqs, congAngs));
            }

            // New proportions? G + G -> A
            foreach (GeometricProportionalAngles gpas in geoPropAngs)
            {
                newGrounded.AddRange(CreateProportionalAngles(gpas, congAngs));
            }

            if (congAngs is GeometricCongruentAngles)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acss in algCongAngles)
                {
                    newGrounded.AddRange(CreateCongruentAngles(acss, congAngs));
                }

                // New equations? G + A -> A
                foreach (AlgebraicAngleEquation aseqs in algAngleEqs)
                {
                    newGrounded.AddRange(CreateAngleEquation(aseqs, congAngs));
                }

                foreach (AlgebraicAngleArcEquation aseqs in algAngleArcEqs)
                {
                    newGrounded.AddRange(CreateAngleArcEquationFromAngleSubstitution(aseqs, congAngs));
                }

                // New proportions? G + A -> A
                foreach (AlgebraicProportionalAngles apas in algPropAngs)
                {
                    newGrounded.AddRange(CreateProportionalAngles(apas, congAngs));
                }
            }

            //
            // NEW
            //
            else if (congAngs is AlgebraicCongruentAngles)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles oldACSS in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateCongruentAngles(oldACSS, congAngs)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicAngleEquation aseqs in algAngleEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleEquation(aseqs, congAngs)));
                }

                foreach (AlgebraicAngleArcEquation aseqs in algAngleArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleArcEquationFromAngleSubstitution(aseqs, congAngs)));
                }

                // New proportions? G + A -> A
                foreach (AlgebraicProportionalAngles apas in algPropAngs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalAngles(apas, congAngs)));
                }
            }

            return newGrounded;
        }

        //
        // For generation of transitive congruent Angles
        //
        private static List<EdgeAggregator> CreateCongruentAngles(CongruentAngles css, CongruentAngles congAng)
        {
            int numSharedExps = css.SharesNumClauses(congAng);
            return CreateCongruent<CongruentAngles>(css, congAng, numSharedExps);
        }
        #endregion

        #region Proportionality Handling
        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Segments
        //
        private static List<EdgeAggregator> HandleNewProportionalAngles(ProportionalAngles propAngs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcas in geoCongAngles)
            {
                newGrounded.AddRange(CreateProportionalAngles(propAngs, gcas));
            }

            if (propAngs is GeometricProportionalAngles)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(CreateProportionalAngles(propAngs, acas));
                }
            }

            //
            // NEW
            //
            if (propAngs is AlgebraicProportionalAngles)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalAngles(propAngs, acas)));
                }
            }


            return newGrounded;
        }

        //
        // For generation of transitive proportional angles
        //
        private static List<EdgeAggregator> CreateProportionalAngles(ProportionalAngles pas, CongruentAngles conAng)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            int numSharedExps = pas.SharesNumClauses(conAng);
            switch (numSharedExps)
            {
                case 0:
                    // Nothing is shared: do nothing
                    break;

                case 1:
                // Expected case to create a new congruence relationship
                //return ProportionalAngles.CreateTransitiveProportion(pas, conAng);

                case 2:
                    // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                    break;

                default:
                    throw new Exception("Proportional / Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
            }

            return newGrounded;
        }
        #endregion

        #region Equation Handling
        //
        // Generate all new relationships from an Equation Containing Angle measurements
        //
        private static List<EdgeAggregator> HandleNewAngleEquation(AngleEquation newAngEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentAngles gcas in geoCongAngles)
            {
                newGrounded.AddRange(CreateAngleEquation(newAngEq, gcas));
            }

            // New equations? G + G -> A
            foreach (GeometricAngleEquation gangs in geoAngleEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gangs, newAngEq));
            }

            if (newAngEq is GeometricAngleEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(CreateAngleEquation(newAngEq, acas));
                }

                // New equations? G + A -> A
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aangs, newAngEq));
                }
            }

            //
            // NEW
            //
            else if (newAngEq is AlgebraicAngleEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentAngles oldACAS in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleEquation(newAngEq, oldACAS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aangs, newAngEq)));
                }
            }

            return newGrounded;
        }

        //
        // Substitute this new angle congruence into old angle equations
        //
        private static List<EdgeAggregator> CreateAngleEquation(AngleEquation angEq, CongruentAngles congAng)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

            //            if (angEq.HasRelationPredecessor(congAng) || congAng.HasRelationPredecessor(angEq)) return newGrounded;

            newEquationEdge = PerformEquationSubstitution(angEq, congAng, congAng.ca1, congAng.ca2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(angEq, congAng, congAng.ca2, congAng.ca1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }
        #endregion

        #region Relation Handling (Supplementary, Complementary, Collinear)
        //
        // If both sides of the substituted equation are atomic and not Numeric Values, create a congruence relationship instead.
        //
        private static GroundedClause HandleAngleRelation(Equation simplified)
        {
            // One side must be atomic
            int atomicity = simplified.GetAtomicity();
            if (atomicity == Equation.NONE_ATOMIC) return null;

            GroundedClause atomic = null;
            GroundedClause nonAtomic = null;
            if (atomicity == Equation.LEFT_ATOMIC)
            {
                atomic = simplified.lhs;
                nonAtomic = simplified.rhs;
            }
            else if (atomicity == Equation.RIGHT_ATOMIC)
            {
                atomic = simplified.rhs;
                nonAtomic = simplified.lhs;
            }
            else if (atomicity == Equation.BOTH_ATOMIC)
            {
                return HandleCollinearPerpendicular(simplified.lhs, simplified.rhs);
            }

            NumericValue atomicValue = atomic as NumericValue;
            if (atomicValue == null) return null;

            //
            // We need only consider special angles (90 or 180)
            //
            if (!Utilities.CompareValues(atomicValue.IntValue, 90) && !Utilities.CompareValues(atomicValue.IntValue, 180)) return null;

            List<GroundedClause> nonAtomicSide = nonAtomic.CollectTerms();

            // Check multiplier for all terms; it must be 1.
            foreach (GroundedClause gc in nonAtomicSide)
            {
                if (gc.multiplier != 1) return null;
            }

            //
            // Complementary or Supplementary
            //
            AnglePairRelation newRelation = null;
            if (nonAtomicSide.Count == 2)
            {
                if (Utilities.CompareValues(atomicValue.IntValue, 90))
                {
                    newRelation = new Complementary((Angle)nonAtomicSide[0], (Angle)nonAtomicSide[1]);
                }
                else if (Utilities.CompareValues(atomicValue.IntValue, 180))
                {
                    newRelation = new Supplementary((Angle)nonAtomicSide[0], (Angle)nonAtomicSide[1]);
                }
            }

            return newRelation;
        }

        //
        // Create a deduced collinear or perpendicular relationship
        //
        private static GroundedClause HandleCollinearPerpendicular(GroundedClause left, GroundedClause right)
        {
            NumericValue numeral = null;
            Angle angle = null;

            //
            // Split the sides
            //
            if (left is NumericValue)
            {
                numeral = left as NumericValue;
                angle = right as Angle;
            }
            else if (right is NumericValue)
            {
                numeral = right as NumericValue;
                angle = left as Angle;
            }

            if (numeral == null || angle == null) return null;

            //
            // Create the new relationships
            //
            Descriptor newDescriptor = null;
            //if (Utilities.CompareValues(numeral.value, 90))
            //{
            //    newDescriptor = new Perpendicular(angle.GetVertex(), angle.ray1, angle.ray2);
            //}
            //else
            if (Utilities.CompareValues(numeral.IntValue, 180))
            {
                List<Point> pts = new List<Point>();
                pts.Add(angle.A);
                pts.Add(angle.B);
                pts.Add(angle.C);
                newDescriptor = new Collinear(pts);
            }

            return newDescriptor;
        }
        #endregion
#endregion

#region Arcs
        #region Congruency Handling
        //
        // Generate all new relationships from a Geoemetric, Congruent Pair of Arcs
        //
        private static List<EdgeAggregator> HandleNewCongruentArcs(CongruentArcs congArcs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentArcs gcss in geoCongArcs)
            {
                newGrounded.AddRange(CreateCongruentArcs(gcss, congArcs));
            }

            // New equations? G + G -> A
            foreach (GeometricArcEquation gseqs in geoArcEqs)
            {
                newGrounded.AddRange(CreateArcEquation(gseqs, congArcs));
            }

            foreach (GeometricAngleArcEquation gseqs in geoAngleArcEqs)
            {
                newGrounded.AddRange(CreateAngleArcEquationFromArcSubstitution(gseqs, congArcs));
            }

            //// New proportions? G + G -> A
            //foreach (GeometricProportionalAngles gpas in geoPropAngs)
            //{
            //    newGrounded.AddRange(CreateProportionalAngles(gpas, congAngs));
            //}

            if (congArcs is GeometricCongruentArcs)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentArcs acas in algCongArcs)
                {
                    newGrounded.AddRange(CreateCongruentArcs(acas, congArcs));
                }

                // New equations? G + A -> A
                foreach (AlgebraicArcEquation aseqs in algArcEqs)
                {
                    newGrounded.AddRange(CreateArcEquation(aseqs, congArcs));
                }

                foreach (AlgebraicAngleArcEquation gseqs in algAngleArcEqs)
                {
                    newGrounded.AddRange(CreateAngleArcEquationFromArcSubstitution(gseqs, congArcs));
                }

                // New proportions? G + A -> A
                //foreach (AlgebraicProportionalAngles apas in algPropAngs)
                //{
                //    newGrounded.AddRange(CreateProportionalAngles(apas, congAngs));
                //}
            }

            //
            // NEW
            //
            else if (congArcs is AlgebraicCongruentArcs)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentArcs oldACAS in algCongArcs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateCongruentArcs(oldACAS, congArcs)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicArcEquation aseqs in algArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateArcEquation(aseqs, congArcs)));
                }

                foreach (AlgebraicAngleArcEquation gseqs in algAngleArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleArcEquationFromArcSubstitution(gseqs, congArcs)));
                }

                //// New proportions? G + A -> A
                //foreach (AlgebraicProportionalAngles apas in algPropAngs)
                //{
                //    newGrounded.AddRange(MakePurelyAlgebraic(CreateProportionalAngles(apas, congAngs)));
                //}
            }

            return newGrounded;
        }

        //
        // For generation of transitive congruent Arcs
        //
        private static List<EdgeAggregator> CreateCongruentArcs(CongruentArcs cas1, CongruentArcs cas2)
        {
            int numSharedExps = cas1.SharesNumClauses(cas2);
            return CreateCongruent<CongruentArcs>(cas1, cas2, numSharedExps);
        }
        #endregion

        #region Equation Handling
        //
        // Generate all new relationships from an Equation Containing Arc measurements
        //
        private static List<EdgeAggregator> HandleNewArcEquation(ArcEquation newArcEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            // New transitivity? G + G -> A
            foreach (GeometricCongruentArcs gcas in geoCongArcs)
            {
                newGrounded.AddRange(CreateArcEquation(newArcEq, gcas));
            }

            // New equations? G + G -> A
            foreach (GeometricArcEquation gangs in geoArcEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gangs, newArcEq));
            }

            if (newArcEq is GeometricArcEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentArcs acas in algCongArcs)
                {
                    newGrounded.AddRange(CreateArcEquation(newArcEq, acas));
                }

                // New equations? G + A -> A
                foreach (AlgebraicArcEquation aarcs in algArcEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aarcs, newArcEq));
                }
            }

            //
            // NEW
            //
            else if (newArcEq is AlgebraicArcEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentArcs oldACAS in algCongArcs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateArcEquation(newArcEq, oldACAS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicArcEquation aarcs in algArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aarcs, newArcEq)));
                }
            }

            return newGrounded;
        }

        //
        // Substitute this new arc congruence into old arc equations
        //
        private static List<EdgeAggregator> CreateArcEquation(ArcEquation arcEq, CongruentArcs congArcs)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

            //            if (angEq.HasRelationPredecessor(congAng) || congAng.HasRelationPredecessor(angEq)) return newGrounded;

            newEquationEdge = PerformEquationSubstitution(arcEq, congArcs, congArcs.ca1, congArcs.ca2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(arcEq, congArcs, congArcs.ca2, congArcs.ca1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }
        #endregion
#endregion

#region AngleArc

        private static List<EdgeAggregator> HandleNewAngleArcEquation(AngleArcEquation angleArcEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //New transitivity?
            foreach (GeometricCongruentArcs gcas in geoCongArcs)
            {
                newGrounded.AddRange(CreateAngleArcEquationFromArcSubstitution(angleArcEq, gcas));
            }
            foreach (GeometricCongruentAngles gcas in geoCongAngles)
            {
                newGrounded.AddRange(CreateAngleArcEquationFromAngleSubstitution(angleArcEq, gcas));
            }

            // New equations? G + G -> A
            foreach (GeometricArcEquation garcs in geoArcEqs)
            {
                newGrounded.AddRange(CreateNewEquation(garcs, angleArcEq));
            }
            foreach (GeometricAngleEquation gangs in geoAngleEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gangs, angleArcEq));
            }
            foreach (GeometricAngleArcEquation gangs in geoAngleArcEqs)
            {
                newGrounded.AddRange(CreateNewEquation(gangs, angleArcEq));
            }

            if (angleArcEq is GeometricAngleArcEquation)
            {
                // New transitivity? G + A -> A
                foreach (AlgebraicCongruentArcs acas in algCongArcs)
                {
                    newGrounded.AddRange(CreateAngleArcEquationFromArcSubstitution(angleArcEq, acas));
                }

                foreach (AlgebraicCongruentAngles acas in algCongAngles)
                {
                    newGrounded.AddRange(CreateAngleArcEquationFromAngleSubstitution(angleArcEq, acas));
                }

                // New equations? G + A -> A
                foreach (AlgebraicArcEquation aarcs in algArcEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aarcs, angleArcEq));
                }
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(aangs, angleArcEq));
                }
                foreach (GeometricAngleArcEquation gangs in geoAngleArcEqs)
                {
                    newGrounded.AddRange(CreateNewEquation(gangs, angleArcEq));
                }
            }

            //
            // NEW
            //
            else if (angleArcEq is AlgebraicAngleArcEquation)
            {
                // New transitivity? A + A -> A
                foreach (AlgebraicCongruentArcs oldACAS in algCongArcs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleArcEquationFromArcSubstitution(angleArcEq, oldACAS)));
                }

                foreach (AlgebraicCongruentAngles oldACAS in algCongAngles)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateAngleArcEquationFromAngleSubstitution(angleArcEq, oldACAS)));
                }

                // New equations? A + A -> A
                foreach (AlgebraicArcEquation aarcs in algArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aarcs, angleArcEq)));
                }
                foreach (AlgebraicAngleEquation aangs in algAngleEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(aangs, angleArcEq)));
                }
                foreach (GeometricAngleArcEquation gangs in geoAngleArcEqs)
                {
                    newGrounded.AddRange(MakePurelyAlgebraic(CreateNewEquation(gangs, angleArcEq)));
                }
            }

            return newGrounded;
        }

        private static List<EdgeAggregator> CreateAngleArcEquationFromArcSubstitution(AngleArcEquation angleArcEq, CongruentArcs congArcs) 
        {
            return CreateEquationFromSubstitution(angleArcEq, congArcs, congArcs.ca1, congArcs.ca2);
        }

        private static List<EdgeAggregator> CreateAngleArcEquationFromAngleSubstitution(AngleArcEquation angleArcEq, CongruentAngles congAngles)
        {
            return CreateEquationFromSubstitution(angleArcEq, congAngles, congAngles.ca1, congAngles.ca2);
        }

        private static List<EdgeAggregator> CreateEquationFromSubstitution(Equation eq, GroundedClause subbedEq, GroundedClause c1, GroundedClause c2)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();
            EdgeAggregator newEquationEdge;

            newEquationEdge = PerformEquationSubstitution(eq, subbedEq, c1, c2);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);
            newEquationEdge = PerformEquationSubstitution(eq, subbedEq, c2, c1);
            if (newEquationEdge != null) newGrounded.Add(newEquationEdge);

            return newGrounded;
        }
#endregion

#region Substitution
        //
        // Substitute some clause (subbedEq) into an equation (eq)
        //
        private static EdgeAggregator PerformEquationSubstitution(Equation eq, GroundedClause subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            //Debug.WriteLine("Substituting with " + eq.ToString() + " and " + subbedEq.ToString());

            if (!eq.ContainsClause(toFind)) return null;

            //
            // Make a deep copy of the equation
            //
            Equation newEq = null;
            if (eq is SegmentEquation)
            {
                newEq = new AlgebraicSegmentEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }
            else if (eq is AngleEquation)
            {
                newEq = new AlgebraicAngleEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }
            else if (eq is ArcEquation)
            {
                newEq = new AlgebraicArcEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }
            else if (eq is AngleArcEquation)
            {
                newEq = new AlgebraicAngleArcEquation(eq.lhs.DeepCopy(), eq.rhs.DeepCopy());
            }

            // Substitute into the copy
            newEq.Substitute(toFind, toSub);

            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq);
            antecedent.Add(subbedEq);

            //
            // Simplify the equation
            //
            Equation simplified = Simplification.Simplify(newEq);

            // Create a congruence relationship if it applies
            GroundedClause newCongruence = HandleCongruence(simplified);
            if (newCongruence != null) return new EdgeAggregator(antecedent, newCongruence, annotation);

            // If a congruence was not established, create a complementary or supplementary relationship, if applicable
            if (simplified is AngleEquation && newCongruence == null)
            {
                GroundedClause newRelation = HandleAngleRelation(simplified);
                if (newRelation != null) return new EdgeAggregator(antecedent, newRelation, annotation);
            }

            return new EdgeAggregator(antecedent, simplified, annotation);
        }

        //
        // This is pure transitivity where A + B = C , A + B = D -> C = D
        //
        private static EdgeAggregator PerformNonAtomicEquationSubstitution(Equation eq, GroundedClause subbedEq, GroundedClause toFind, GroundedClause toSub)
        {
            //Debug.WriteLine("Substituting with " + eq.ToString() + " and " + subbedEq.ToString());

            // If there is a deduction relationship between the given congruences, do not perform another substitution
            //  subbedEq.HasPredecessor(eq)
            //if (eq.HasGeneralPredecessor(subbedEq) || subbedEq.HasGeneralPredecessor(eq)) return new EdgeAggregator(null, null);

            //
            // Verify that the non-atomic sides to both equations are the exact same
            //
            GroundedClause nonAtomicOriginal = null;
            GroundedClause atomicOriginal = null;
            if (eq.GetAtomicity() == Equation.LEFT_ATOMIC)
            {
                nonAtomicOriginal = eq.rhs;
                atomicOriginal = eq.lhs;
            }
            else if (eq.GetAtomicity() == Equation.RIGHT_ATOMIC)
            {
                nonAtomicOriginal = eq.lhs;
                atomicOriginal = eq.rhs;
            }

            // We collect all the flattened terms
            List<GroundedClause> originalTerms = nonAtomicOriginal.CollectTerms();
            List<GroundedClause> subbedTerms = toFind.CollectTerms();

            // Now, the lists must be the same; we check for containment in both directions
            foreach (GroundedClause originalTerm in originalTerms)
            {
                if (!subbedTerms.Contains(originalTerm)) return null;
            }

            foreach (GroundedClause subbedTerm in subbedTerms)
            {
                if (!originalTerms.Contains(subbedTerm)) return null;
            }

            // Hypergraph
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq);
            antecedent.Add(subbedEq);
            EdgeAggregator newEdge;

            //
            // Generate a simple equation or an algebraic congruent statement
            //
            if (atomicOriginal is NumericValue || toSub is NumericValue)
            {
                Equation newEquation = null;
                if (eq is AngleEquation)
                {
                    newEquation = new AlgebraicAngleEquation(atomicOriginal.DeepCopy(), toSub.DeepCopy());
                }
                else if (eq is SegmentEquation)
                {
                    newEquation = new AlgebraicSegmentEquation(atomicOriginal.DeepCopy(), toSub.DeepCopy());
                }

                if (newEquation == null)
                {
                    Debug.WriteLine("");
                    throw new NullReferenceException("Unexpected Problem in Non-atomic substitution (equation)...");
                }

                newEdge = new EdgeAggregator(antecedent, newEquation, annotation);
            }
            else
            {
                Congruent newCongruent = null;
                if (eq is AngleEquation)
                {
                    newCongruent = new AlgebraicCongruentAngles((Angle)atomicOriginal.DeepCopy(), (Angle)toSub.DeepCopy());
                }
                else if (eq is SegmentEquation)
                {
                    newCongruent = new AlgebraicCongruentSegments((Segment)atomicOriginal.DeepCopy(), (Segment)toSub.DeepCopy());
                }

                if (newCongruent == null)
                {
                    Debug.WriteLine("");
                    throw new NullReferenceException("Unexpected Problem in Non-atomic substitution (Congruence)...");
                }

                newEdge = new EdgeAggregator(antecedent, newCongruent, annotation);
            }

            return newEdge;
        }

        //
        // Given two equations, perform a direct, transitive substitution of one equation into the other (and vice versa)
        //
        private static List<EdgeAggregator> PerformEquationTransitiviteSubstitution(Equation eq1, Equation eq2)
        {
            List<GroundedClause> newRelations = new List<GroundedClause>();

            //
            // Collect the terms from each side of both equations
            //
            List<GroundedClause> lhsTermsEq1 = eq1.lhs.CollectTerms();
            List<GroundedClause> lhsTermsEq2 = eq2.lhs.CollectTerms();
            List<GroundedClause> rhsTermsEq1 = eq1.rhs.CollectTerms();
            List<GroundedClause> rhsTermsEq2 = eq2.rhs.CollectTerms();

            int equationType = GetEquationType(eq1);

            //
            // Construct the new equations using all possible combinations
            //
            if (EqualLists(lhsTermsEq1, lhsTermsEq2))
            {
                GroundedClause newEq = ConstructNewEquation(equationType, eq1.rhs, eq2.rhs);
                if (newEq != null) newRelations.Add(newEq);
            }
            if (EqualLists(lhsTermsEq1, rhsTermsEq2))
            {
                GroundedClause newEq = ConstructNewEquation(equationType, eq1.rhs, eq2.lhs);
                if (newEq != null) newRelations.Add(newEq);
            }
            if (EqualLists(rhsTermsEq1, lhsTermsEq2))
            {
                GroundedClause newEq = ConstructNewEquation(equationType, eq1.lhs, eq2.rhs);
                if (newEq != null) newRelations.Add(newEq);
            }
            if (EqualLists(rhsTermsEq1, rhsTermsEq2))
            {
                GroundedClause newEq = ConstructNewEquation(equationType, eq1.lhs, eq2.lhs);
                if (newEq != null) newRelations.Add(newEq);
            }

            //
            // Construct the hypergraph edges for all of the new relationships
            //
            List<GroundedClause> antecedent = new List<GroundedClause>();
            antecedent.Add(eq1);
            antecedent.Add(eq2);

            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            foreach (GroundedClause gc in newRelations)
            {
                newGrounded.Add(new EdgeAggregator(antecedent, gc, annotation));
            }

            return newGrounded;
        }

        //
        // Given two equations, if one equation is atomic, substitute into the other (and vice versa)
        //
        //private static List<KeyValuePair<List<GroundedClause>, GroundedClause>> PerformEquationDirectSubstitution(Equation eq1, Equation eq2)
        //{
        //    //
        //    // Does the new equation have one side which is isolated (an atomic expression)?
        //    //
        //    int atomicSide = newEq.GetAtomicity();

        //    GroundedClause atomicExp = null;
        //    GroundedClause otherSide = null;
        //    switch (atomicSide)
        //    {
        //        case Equation.LEFT_ATOMIC:
        //            atomicExp = newEq.lhs;
        //            otherSide = newEq.rhs;
        //            break;

        //        case Equation.RIGHT_ATOMIC:
        //            atomicExp = newEq.rhs;
        //            otherSide = newEq.lhs;
        //            break;

        //        case Equation.BOTH_ATOMIC:
        //            // Choose both sides
        //            break;

        //        case Equation.NONE_ATOMIC:
        //            // If neither side of this new equation are atomic, for simplicty,
        //            // we do not perform a substitution
        //            return new List<EdgeAggregator>();
        //    }

        //    KeyValuePair<List<GroundedClause>, GroundedClause> cl;
        //    // One side of the equation is atomic
        //    if (atomicExp != null)
        //    {
        //        // Check to see if the old equation is dually atomic as
        //        // we will want to substitute the old eq into the new one
        //        int oldAtomic = oldEq.GetAtomicity();
        //        if (oldAtomic != Equation.BOTH_ATOMIC)
        //        {
        //            // Simple sub of new equation into old
        //            cl = PerformEquationSubstitution(oldEq, newEq, atomicExp, otherSide);
        //            if (cl.Value != null) newGrounded.Add(cl);

        //            //
        //            // In the case where we have a situation of the form: A + B = C
        //            //                                                    A + B = D  -> C = D
        //            //
        //            // Perform a non-atomic substitution
        //            cl = PerformNonAtomicEquationSubstitution(oldEq, newEq, otherSide, atomicExp);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //        }
        //        else if (oldAtomic == Equation.BOTH_ATOMIC)
        //        {
        //            // Dual sub of old equation into new
        //            cl = PerformEquationSubstitution(newEq, oldEq, oldEq.lhs, oldEq.rhs);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //            cl = PerformEquationSubstitution(newEq, oldEq, oldEq.rhs, oldEq.lhs);
        //            if (cl.Value != null) newGrounded.Add(cl);
        //        }
        //    }
        //    // The new equation has both sides atomic; try to sub in the other side
        //    else
        //    {
        //        cl = PerformEquationSubstitution(oldEq, newEq, newEq.lhs, newEq.rhs);
        //        if (cl.Value != null) newGrounded.Add(cl);
        //        cl = PerformEquationSubstitution(oldEq, newEq, newEq.rhs, newEq.lhs);
        //        if (cl.Value != null) newGrounded.Add(cl);
        //    }

        //    return newGrounded;
        //}
#endregion

#region Congruency Helpers
        //
        // If both sides of the substituted equation are atomic and not Numeric Values, create a congruence relationship instead.
        //
        private static GroundedClause HandleCongruence(Equation simplified)
        {
            // Both sides must be atomic and multiplied by a factor of 1 for a proper congruence
            if (simplified.GetAtomicity() != Equation.BOTH_ATOMIC) return null;

            if (!simplified.IsProperCongruence()) return null;

            // Then create a congruence, whether it be angle or segment
            Congruent newCongruent = null;
            if (simplified is AlgebraicAngleEquation)
            {
                // Do not generate for lines; that is, 180^o angles
                //if (((Angle)simplified.lhs).IsStraightAngle() && ((Angle)simplified.lhs).IsStraightAngle()) return null;
                newCongruent = new AlgebraicCongruentAngles((Angle)simplified.lhs, (Angle)simplified.rhs);
            }
            else if (simplified is AlgebraicSegmentEquation)
            {
                newCongruent = new AlgebraicCongruentSegments((Segment)simplified.lhs, (Segment)simplified.rhs);
            }
            else if (simplified is AlgebraicArcEquation)
            {
                newCongruent = new AlgebraicCongruentArcs((Arc)simplified.lhs, (Arc)simplified.rhs);
            }

            // There is no need to simplify a congruence, so just return
            return newCongruent;
        }

        //
        // For generic generation of transitive congruent clauses
        //
        private static List<EdgeAggregator> CreateCongruent<T>(T css, T geoCong, int numSharedExps) where T : Congruent
        {
            {
                List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

                //if (css.HasRelationPredecessor(geoCongSeg) || geoCongSeg.HasRelationPredecessor(css)) return newGrounded;

                switch (numSharedExps)
                {
                    case 0:
                        // Nothing is shared: do nothing
                        break;

                    case 1:
                        // Expected case to create a new congruence relationship
                        return Congruent.CreateTransitiveCongruence(css, geoCong);

                    case 2:
                        // This is either reflexive or the exact same congruence relationship (which shouldn't happen)
                        break;

                    default:
                        throw new Exception("Congruent Statements may only have 0, 1, or 2 common expressions; not, " + numSharedExps);
                }

                return newGrounded;
            }
        }
#endregion

#region Equation Helpers
        private static int GetEquationType(Equation eq)
        {
            if (eq is SegmentEquation) return SEGMENT_EQUATION;
            if (eq is AngleEquation) return ANGLE_EQUATION;
            if (eq is ArcEquation) return ARC_EQUATION;
            if (eq is AngleArcEquation) return ANGLE_ARC_EQUATION;
            return EQUATION_ERROR;
        }

        //
        // Given two news sides for an equation, create the equation. If possible, create a congruence or a supplementary / complementary relationship
        //
        private static GroundedClause ConstructNewEquation(int equationType, GroundedClause left, GroundedClause right)
        {
            //
            //Account for possibility that substitution may require a change in equation type between Angle, Arc, and AngleArc
            //
            if (equationType != SEGMENT_EQUATION)
            {
                List<GroundedClause> terms = new List<GroundedClause>();
                terms.AddRange(left.CollectTerms());
                terms.AddRange(right.CollectTerms());
                bool hasAngle = terms.Any(c => c is Angle);
                bool hasArc = terms.Any(c => c is Arc);
                if (hasAngle && hasArc) equationType = ANGLE_ARC_EQUATION;
                else if (hasAngle) equationType = ANGLE_EQUATION;
                else if (hasArc) equationType = ARC_EQUATION;
            }

            //
            // Construct the new equation with a given left / right side
            //
            Equation newEq = null;
            if (equationType == SEGMENT_EQUATION)
            {
                newEq = new AlgebraicSegmentEquation(left, right);
            }
            else if (equationType == ANGLE_EQUATION)
            {
                newEq = new AlgebraicAngleEquation(left, right);
            }
            else if (equationType == ARC_EQUATION)
            {
                //Do not try to relate arcs from non-congruent circles
                Arc arc1 = (left is Arc) ? left as Arc : null;
                Arc arc2 = (right is Arc) ? right as Arc : null;
                if (arc1 != null && arc2 != null && !Utilities.CompareValues(arc1.theCircle.radius, arc2.theCircle.radius)) return null;

                newEq = new AlgebraicArcEquation(left, right);
            }
            else if (equationType == ANGLE_ARC_EQUATION)
            {
                newEq = new AlgebraicAngleArcEquation(left, right);
            }

            //
            // Simplify the equation
            //
            Equation simplified = Simplification.Simplify(newEq);
			if (simplified == null) return null;
			
            //
            // Create a congruence relationship if it applies
            //
            GroundedClause newCongruence = HandleCongruence(simplified);
            if (newCongruence != null) return newCongruence;

            //
            // If a congruence was not established, create a complementary or supplementary relationship, if applicable
            //
            if (equationType == ANGLE_EQUATION)
            {
                GroundedClause newRelation = HandleAngleRelation(simplified);
                if (newRelation != null) return newRelation;
            }

            // Just return the simplified equation if nothing else could be deduced
            return simplified;
        }

        //
        // Given an old and new set of angle measure equations substitute if possible.
        //
        private static List<EdgeAggregator> CreateNewEquation(Equation oldEq, Equation newEq)
        {
            List<EdgeAggregator> newGrounded = new List<EdgeAggregator>();

            //Debug.WriteLine("Considering combining: " + oldEq + " + " + newEq);

            // Avoid redundant equation generation
            //            if (oldEq.HasRelationPredecessor(newEq) || newEq.HasRelationPredecessor(oldEq)) return newGrounded;

            // Determine if there is a direct, transitive relationship with the equations
            newGrounded.AddRange(PerformEquationTransitiviteSubstitution(oldEq, newEq));

            // If we have an atomic situation, substitute into the other equation
            //newGrounded.AddRange(PerformEquationDirectSubstitution(oldEq, newEq));

            return newGrounded;
        }
#endregion

#region General Helpers
        //
        // Add predecessors to the equations, congruence relationships, etc.
        //
        private static void MarkPredecessors(List<EdgeAggregator> edges)
        {
            foreach (EdgeAggregator edge in edges)
            {
                foreach (GroundedClause predNode in edge.antecedent)
                {
                    edge.consequent.AddRelationPredecessor(predNode);
                    edge.consequent.AddRelationPredecessors(predNode.relationPredecessors);
                }
            }
        }

        // Sets all of the deduced nodes to be purely algebraic: A + A -> A
        private static List<EdgeAggregator> MakePurelyAlgebraic(List<EdgeAggregator> edges)
        {
            foreach (EdgeAggregator edge in edges)
            {
                edge.consequent.MakePurelyAlgebraic();
            }

            return edges;
        }

        //
        // Check equivalence of lists by verifying dual containment.
        //
        private static bool EqualLists(List<GroundedClause> list1, List<GroundedClause> list2)
        {
            foreach (GroundedClause val1 in list1)
            {
                if (!list2.Contains(val1)) return false;
            }

            foreach (GroundedClause val2 in list2)
            {
                if (!list1.Contains(val2)) return false;
            }

            return true;
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static void AddToAppropriateList(GroundedClause c)
        {
            if (c is GeometricCongruentSegments)
            {
                geoCongSegments.Add(c as GeometricCongruentSegments);
            }
            else if (c is GeometricSegmentEquation)
            {
                geoSegmentEqs.Add(c as GeometricSegmentEquation);
            }
            else if (c is GeometricCongruentAngles)
            {
                geoCongAngles.Add(c as GeometricCongruentAngles);
            }
            else if (c is GeometricAngleEquation)
            {
                geoAngleEqs.Add(c as GeometricAngleEquation);
            }
            else if (c is GeometricCongruentArcs)
            {
                geoCongArcs.Add(c as GeometricCongruentArcs);
            }
            else if (c is GeometricArcEquation)
            {
                geoArcEqs.Add(c as GeometricArcEquation);
            }
            else if (c is GeometricAngleArcEquation)
            {
                geoAngleArcEqs.Add(c as GeometricAngleArcEquation);
            }
            else if (c is AlgebraicSegmentEquation)
            {
                algSegmentEqs.Add(c as AlgebraicSegmentEquation);
            }
            else if (c is AlgebraicAngleEquation)
            {
                algAngleEqs.Add(c as AlgebraicAngleEquation);
            }
            else if (c is AlgebraicArcEquation)
            {
                algArcEqs.Add(c as AlgebraicArcEquation);
            }
            else if (c is AlgebraicAngleArcEquation)
            {
                algAngleArcEqs.Add(c as AlgebraicAngleArcEquation);
            }
            else if (c is AlgebraicCongruentSegments)
            {
                algCongSegments.Add(c as AlgebraicCongruentSegments);
            }
            else if (c is AlgebraicCongruentAngles)
            {
                algCongAngles.Add(c as AlgebraicCongruentAngles);
            }
            else if (c is AlgebraicCongruentArcs)
            {
                algCongArcs.Add(c as AlgebraicCongruentArcs);
            }
            else if (c is SegmentRatio)
            {
                propSegs.Add(c as SegmentRatio);
            }
            //else if (c is AlgebraicSegmentRatioEquation)
            //{
            //    algPropSegs.Add(c as AlgebraicSegmentRatioEquation);
            //}
            else if (c is GeometricProportionalAngles)
            {
                geoPropAngs.Add(c as GeometricProportionalAngles);
            }
            else if (c is AlgebraicProportionalAngles)
            {
                algPropAngs.Add(c as AlgebraicProportionalAngles);
            }
        }

        //
        // Add the new grounded clause to the correct list.
        //
        private static bool ClauseHasBeenDeduced(GroundedClause c)
        {
            if (c is GeometricCongruentSegments)
            {
                return geoCongSegments.Contains(c as GeometricCongruentSegments);
            }
            else if (c is GeometricSegmentEquation)
            {
                return geoSegmentEqs.Contains(c as GeometricSegmentEquation);
            }
            else if (c is GeometricCongruentAngles)
            {
                return geoCongAngles.Contains(c as GeometricCongruentAngles);
            }
            else if (c is GeometricAngleEquation)
            {
                return geoAngleEqs.Contains(c as GeometricAngleEquation);
            }
            else if (c is GeometricCongruentArcs)
            {
                return geoCongArcs.Contains(c as GeometricCongruentArcs);
            }
            else if (c is GeometricArcEquation)
            {
                return geoArcEqs.Contains(c as GeometricArcEquation);
            }
            else if (c is GeometricAngleArcEquation)
            {
                return geoAngleArcEqs.Contains(c as GeometricAngleArcEquation);
            }
            else if (c is AlgebraicSegmentEquation)
            {
                return algSegmentEqs.Contains(c as AlgebraicSegmentEquation);
            }
            else if (c is AlgebraicAngleEquation)
            {
                return algAngleEqs.Contains(c as AlgebraicAngleEquation);
            }
            else if (c is AlgebraicArcEquation)
            {
                return algArcEqs.Contains(c as AlgebraicArcEquation);
            }
            else if (c is AlgebraicAngleArcEquation)
            {
                return algAngleArcEqs.Contains(c as AlgebraicAngleArcEquation);
            }
            else if (c is AlgebraicCongruentSegments)
            {
                return algCongSegments.Contains(c as AlgebraicCongruentSegments);
            }
            else if (c is AlgebraicCongruentAngles)
            {
                return algCongAngles.Contains(c as AlgebraicCongruentAngles);
            }
            else if (c is AlgebraicCongruentArcs)
            {
                return algCongArcs.Contains(c as AlgebraicCongruentArcs);
            }
            else if (c is GeometricSegmentRatioEquation)
            {
                return propSegs.Contains(c as SegmentRatio);
            }
            //else if (c is AlgebraicSegmentRatioEquation)
            //{
            //    return algPropSegs.Contains(c as AlgebraicSegmentRatioEquation);
            //}
            else if (c is GeometricProportionalAngles)
            {
                return geoPropAngs.Contains(c as GeometricProportionalAngles);
            }
            else if (c is AlgebraicProportionalAngles)
            {
                return algPropAngs.Contains(c as AlgebraicProportionalAngles);
            }

            return false;
        }
#endregion

    }
}