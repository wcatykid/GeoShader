using System;
using System.Collections.Generic;

namespace GeometryTutorLib.EngineUIBridge
{
    //
    // The bridge between the front and back end where the user indicates which axioms, definitions, and theorems to use.
    //
    public static class JustificationSwitch
    {
        private static Dictionary<DeductionJustType, Assumption> justMap = null;

        public enum DeductionJustType
        {
            // Axioms
            AA_SIMILARITY,
            ANGLE_ADDITION_AXIOM,
            ASA,
            CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL,
            CORRESPONDING_ANGLES_OF_PARALLEL_LINES,
            SEGMENT_ADDITION_AXIOM,
            SAS_CONGRUENCE,
            SSS,
            ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT,
            TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE,
            ARC_ADDITION_AXIOM,

            // Definitions
            ALTITUDE_DEFINITION,
            ANGLE_BISECTOR_DEFINITION,
            COMPLEMENTARY_DEFINITION,
            CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION,
            EQUILATERAL_TRIANGLE_DEFINITION,
            ISOSCELES_TRIANGLE_DEFINITION,
            MEDIAN_DEFINITION,
            MIDPOINT_DEFINITION,
            PERPENDICULAR_DEFINITION,
            PERPENDICULAR_BISECTOR_DEFINITION,
            RIGHT_ANGLE_DEFINITION,
            RIGHT_TRIANGLE_DEFINITION,
            SEGMENT_BISECTOR_DEFINITION,
            SUPPLEMENTARY_DEFINITION,
            STRAIGHT_ANGLE_DEFINITION,

            // Theorems
            AAS,
            ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY,
            ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY,
            ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL,
            ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR,
            ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES,
            ANGLE_BISECTOR_THEOREM,
            CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES,
            CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR,
            CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES,
            CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES,
            EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES,
            EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES,
            HYPOTENUSE_LEG,
            ISOSCELES_TRIANGLE_THEOREM,
            MIDPOINT_THEOREM,
            PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES,
            PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY,
            PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES,
            RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT,
            SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL,
            SAS_SIMILARITY,
            SSS_SIMILARITY,
            SUM_ANGLES_IN_TRIANGLE_180,
            TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR,
            TRANSITIVE_CONGRUENT_TRIANGLES,
            TRANSITIVE_PARALLEL,
            TRANSITIVE_SIMILAR,
            TRIANGLE_PROPORTIONALITY,
            TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT,
            VERTICAL_ANGLES,

            //
            // Quadrilaterals
            //
            QUADRILATERAL_DEFINITION,
            KITE_DEFINITION,
            PARALLELOGRAM_DEFINITION,
            RHOMBUS_DEFINITION,
            RECTANGLE_DEFINITION,
            SQUARE_DEFINITION,
            TRAPEZOID_DEFINITION,
            ISOSCELES_TRAPEZOID_DEFINITION,

            OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT,
            OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT,
            DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER,
            OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM,
            ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM,
            OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM,
            DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM,
            DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT,
            DIAGONALS_OF_KITE_ARE_PERPENDICULAR,
            DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR,
            DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS,
            TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS,
            BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT,
            MEDIAN_TRAPEZOID_PARALLEL_TO_BASE,
            MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES,

            //
            // Circles
            //
            TANGENT_IS_PERPENDICULAR_TO_RADIUS,
            TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT,
            PERPENDICULAR_TO_RADIUS_IS_TANGENT,
            MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT,
            CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT,
            CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS,
            CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS,
            DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC,
            MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC,
            CIRCLE_DEFINITION,
            CIRCLE_CONGRUENCE_DEFINITION,
            ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT,
            CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC,
            CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC,
            EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS,
            TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS,
            TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES,
            INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY,

            // General Switches
            CIRCLES,
            QUADRILATERALS,
            SIMILARITY,
            TRIANGLE_CONGREUNCE,
            REFLEXIVE,
        };

        public static Assumption GetAssumption(DeductionJustType type)
        {
            ConstructJustificationMap();

            Assumption assumption = null;
            if (justMap.TryGetValue(type, out assumption))
            {
                throw new ArgumentException("Deduction Type Value (" + type + ") not recognized.");
            }

            return assumption;
        }

        public static Dictionary<DeductionJustType, Assumption> GetAssumptions()
        {
            ConstructJustificationMap();

            //if (justMap == null) ConstructJustificationMap();

            return justMap;
        }

        private static void ConstructJustificationMap()
        {
            justMap = new Dictionary<DeductionJustType, Assumption>();

            // Axioms
            justMap.Add(DeductionJustType.AA_SIMILARITY, new Assumption("AA Similarity", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.ANGLE_ADDITION_AXIOM, new Assumption("Angle Addition", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.ASA, new Assumption("ASA", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL, new Assumption("Congruent Corresponging Imply Parallel", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.CORRESPONDING_ANGLES_OF_PARALLEL_LINES, new Assumption("Parallel Imply Congruent Corresponding Angles", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.SEGMENT_ADDITION_AXIOM, new Assumption("Segment Addition", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.SAS_CONGRUENCE, new Assumption("SAS Congruence", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.SSS, new Assumption("SSS Congruence", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT, new Assumption("Angles of Equal Measure are Congruent", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION, new Assumption("Congruent Segments Imply Proportional Segments", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.ARC_ADDITION_AXIOM, new Assumption("Arc Addition", Assumption.AssumptionType.Axiom));

            // Definitions
            justMap.Add(DeductionJustType.ALTITUDE_DEFINITION, new Assumption("Altitude", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.ANGLE_BISECTOR_DEFINITION, new Assumption("Defintion of Angle Bisector", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.COMPLEMENTARY_DEFINITION, new Assumption("Complementary Angles", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.ISOSCELES_TRIANGLE_DEFINITION, new Assumption("Isosceles Triangle", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.EQUILATERAL_TRIANGLE_DEFINITION, new Assumption("Equilateral Triangle", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.MEDIAN_DEFINITION, new Assumption("Median", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.MIDPOINT_DEFINITION, new Assumption("Midpoint", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.PERPENDICULAR_DEFINITION, new Assumption("Perpendicular", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.PERPENDICULAR_BISECTOR_DEFINITION, new Assumption("Perpendicular Bisector", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.RIGHT_ANGLE_DEFINITION, new Assumption("Right Angle", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.RIGHT_TRIANGLE_DEFINITION, new Assumption("Right Triangle", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.SEGMENT_BISECTOR_DEFINITION, new Assumption("Segment Bisector", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.SUPPLEMENTARY_DEFINITION, new Assumption("Supplementary", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.STRAIGHT_ANGLE_DEFINITION, new Assumption("Straight Angle", Assumption.AssumptionType.Definition));

            // Quadrilateral Defs
            justMap.Add(DeductionJustType.QUADRILATERAL_DEFINITION, new Assumption("Quadrilateral", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.KITE_DEFINITION, new Assumption("Kite", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.PARALLELOGRAM_DEFINITION, new Assumption("Parallelogram", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.RHOMBUS_DEFINITION, new Assumption("Rhombus", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.SQUARE_DEFINITION, new Assumption("Square", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.RECTANGLE_DEFINITION, new Assumption("Rectangle", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.TRAPEZOID_DEFINITION, new Assumption("Trapezoid", Assumption.AssumptionType.Definition));
            justMap.Add(DeductionJustType.ISOSCELES_TRAPEZOID_DEFINITION, new Assumption("Isosceles Trapezoid", Assumption.AssumptionType.Definition));


            // Theorems
            justMap.Add(DeductionJustType.AAS, new Assumption("AAS", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY, new Assumption("Acute Angles in Right Triangle are Complementary", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY, new Assumption("Perpendicular Adjacent Angles Imply Complementary", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL, new Assumption("Congruent Alternate Interior Angles Imply Parallel", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR, new Assumption("Altitude of Right Triangles Imply Similar Triangles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES, new Assumption("Angle Bisector of Isosceles Triangles is Perpendicular Bisector", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ANGLE_BISECTOR_THEOREM, new Assumption("Angle Bisector Theorem", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES, new Assumption("Congruent Supplementary Angles Are Right Angles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR, new Assumption("Two Lines Forming Congruent Adjacent Imply Perpendicular", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES, new Assumption("Congruent Angles in Triangle Imply Congruent Opposite Sides", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES, new Assumption("Congruent Sides in Triangle Imply Congruent Opposite Angles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES, new Assumption("Exterioir Angle Measure Equal to Sum of Remote Interior", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES, new Assumption("Equilateral Triangle Angles Measure 60 Degrees", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.HYPOTENUSE_LEG, new Assumption("Hypotenuse Leg", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ISOSCELES_TRIANGLE_THEOREM, new Assumption("Isosceles Triangle Theorem", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.MIDPOINT_THEOREM, new Assumption("Midpoint Theorem", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES, new Assumption("Parallel Imply Alternate Interioir Angles Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY, new Assumption("Parallel Imply Same Side Interioir Supplementary", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES, new Assumption("Perpendicular Lines Imply Congruent Adjacent Angles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT, new Assumption("Supplementary (or Complementary) Relations of Congruent Angles are Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL, new Assumption("Same Side Supplementary Angles Imply Parallel", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.SAS_SIMILARITY, new Assumption("SAS Similarity", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.SSS_SIMILARITY, new Assumption("SSS Similarity", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.SUM_ANGLES_IN_TRIANGLE_180, new Assumption("Sum of Angles in a Triangle is 180 Degrees", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR, new Assumption("Perpendicular Transversal To Parallel Lines Imply Perpendicular to Both Lines", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TRANSITIVE_CONGRUENT_TRIANGLES, new Assumption("Transitivity of Congruent Triangles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TRANSITIVE_PARALLEL, new Assumption("Transitivity of Parallel Lines", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TRANSITIVE_SIMILAR, new Assumption("Transitivity of Similar Triangles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TRIANGLE_PROPORTIONALITY, new Assumption("Proportionality of Triangles", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT, new Assumption("Two pairs of Congruent Angles in Triangle Imply Third Pair Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.VERTICAL_ANGLES, new Assumption("Vertical Angles", Assumption.AssumptionType.Theorem));

            // Quadrilateral Theorems
            justMap.Add(DeductionJustType.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT, new Assumption("Opp. Sides in Parallelogram Are Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT, new Assumption("Opp. Angles in Parallelogram Are Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER, new Assumption("Diagonals in Parallelogram Bisect Each Other", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM, new Assumption("Opp. Sides Congruent in Quad Implies Parallelogram", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM, new Assumption("Opp. Sides Congruent in Quad Implies Parallelogram", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM, new Assumption("Opp. Sides Congruent in Quad Implies Parallelogram", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM, new Assumption("Diagonals Bisect Each Other Imply Parallelogram", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT, new Assumption("Diagonals of a Rectangle are Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_OF_KITE_ARE_PERPENDICULAR, new Assumption("Diagonals of a Kite are Perpendicular", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR, new Assumption("Diagonals of a Rhombus are Perpendicular", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS, new Assumption("Diagonals of a Rhmbus Bisect the Angles of a Rhombus", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS, new Assumption("Two Congruent Consecutive Sides of a Parallelogram Imply Rhombus", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT, new Assumption("Two Congruent Consecutive Sides of a Parallelogram Imply Rhombus", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE, new Assumption("Two Congruent Consecutive Sides of a Parallelogram Imply Rhombus", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES, new Assumption("Two Congruent Consecutive Sides of a Parallelogram Imply Rhombus", Assumption.AssumptionType.Theorem));

            // Circles
            justMap.Add(DeductionJustType.CIRCLE_DEFINITION, new Assumption("Definition of Circle: Congruent Radii", Assumption.AssumptionType.Axiom));
            justMap.Add(DeductionJustType.CIRCLE_CONGRUENCE_DEFINITION, new Assumption("Circles of Equal Radii Congruent", Assumption.AssumptionType.Axiom));

            justMap.Add(DeductionJustType.TANGENT_IS_PERPENDICULAR_TO_RADIUS, new Assumption("A Tangent Is Perpendicular to a Radius", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.PERPENDICULAR_TO_RADIUS_IS_TANGENT, new Assumption("Perpendicular to Radius is Tangent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT, new Assumption("Tangents to Circle from Same Point Congruent", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT, new Assumption("Central Angle Congruence Implies Arc Minor Congruence", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT, new Assumption("Arc Minor Congruence Congruence Implies Central Angle", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS, new Assumption("Congruent Chords have Congruent Arcs", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS, new Assumption("Congruent Arcs have Congruent Chords", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC, new Assumption("Diameter Perpendicular to Chord Bisects the Chord and Arc", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC, new Assumption("Measure of Inscribed Angle is Half Intercepted Arc", Assumption.AssumptionType.Theorem));

            justMap.Add(DeductionJustType.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT, new Assumption("Inscribed Angle In Semicircle is Right", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC, new Assumption("Measure Central Angle Equals Measure Intercepted Arc", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC, new Assumption("Angle Created by Tangent and Chord is Half Intercepted Arc", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS, new Assumption("Angle Measure of Exterior Angle is Half Difference of Intercepted Arcs", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS, new Assumption("Intersecting Chords Inside a Circle Create Angle Measure Half Sum of Intercepted Arc", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES, new Assumption("Two Insribed Angles have Equal Measure if they Intercept Same Arc", Assumption.AssumptionType.Theorem));
            justMap.Add(DeductionJustType.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY, new Assumption("Inscribed Quadrilateral has Supplementary Opposite Angles", Assumption.AssumptionType.Theorem));
        }

        public static bool RELATION_TRANSITIVE_SUBSTITUTION = true;
        public static bool TRANSITIVE_SUBSTITUTION = true;
        public static bool ANGLE = true;

        //
        // Axioms
        //
        public static bool AA_SIMILARITY = true;
        public static bool ANGLE_ADDITION_AXIOM = true;
        public static bool ASA = true;
        public static bool CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL = true;
        public static bool CORRESPONDING_ANGLES_OF_PARALLEL_LINES = true;
        public static bool SEGMENT_ADDITION_AXIOM = true;
        public static bool SAS_CONGRUENCE = true;
        public static bool SSS = true;
        public static bool ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT = true;
        public static bool TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE = true;
        public static bool ARC_ADDITION_AXIOM = true;

        //
        // Definitions
        //
        public static bool ALTITUDE_DEFINITION = true;
        public static bool ANGLE_BISECTOR_DEFINITION = true;
        public static bool COMPLEMENTARY_DEFINITION = true;
        public static bool CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = true;
        public static bool ISOSCELES_TRIANGLE_DEFINITION = true;
        public static bool EQUILATERAL_TRIANGLE_DEFINITION = true;
        public static bool MEDIAN_DEFINITION = true;
        public static bool MIDPOINT_DEFINITION = true;
        public static bool PERPENDICULAR_DEFINITION = true;
        public static bool PERPENDICULAR_BISECTOR_DEFINITION = true;
        public static bool RIGHT_ANGLE_DEFINITION = true;
        public static bool RIGHT_TRIANGLE_DEFINITION = true;
        public static bool SEGMENT_BISECTOR_DEFINITION = true;
        public static bool SUPPLEMENTARY_DEFINITION = true;
        public static bool STRAIGHT_ANGLE_DEFINITION = true;

        // Quadrilateral Defs
        public static bool QUADRILATERAL_DEFINITION = true;
        public static bool KITE_DEFINITION = true;
        public static bool PARALLELOGRAM_DEFINITION = true;
        public static bool RHOMBUS_DEFINITION = true;
        public static bool SQUARE_DEFINITION = true;
        public static bool RECTANGLE_DEFINITION = true;
        public static bool TRAPEZOID_DEFINITION = true;
        public static bool ISOSCELES_TRAPEZOID_DEFINITION = true;

        //
        // Theorems
        //
        public static bool AAS = true;
        public static bool ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY = true;
        public static bool ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY = true;
        public static bool ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL = true;
        public static bool ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR = true;
        public static bool ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES = true;
        public static bool ANGLE_BISECTOR_THEOREM = true;
        public static bool CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES = true;
        public static bool CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR = true;
        public static bool CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES = true;
        public static bool CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES = true;
        public static bool EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES = true;
        public static bool EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES = false;
        public static bool HYPOTENUSE_LEG = true;
        public static bool ISOSCELES_TRIANGLE_THEOREM = true;
        public static bool MIDPOINT_THEOREM = true;
        public static bool PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES = true;
        public static bool PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY = true;
        public static bool PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES = true;
        public static bool RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT = true;
        public static bool SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL = true;
        public static bool SAS_SIMILARITY = true;
        public static bool SSS_SIMILARITY = true;
        public static bool SUM_ANGLES_IN_TRIANGLE_180 = false;
        public static bool TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR = true;
        public static bool TRANSITIVE_CONGRUENT_TRIANGLES = true;
        public static bool TRANSITIVE_PARALLEL = true;
        public static bool TRANSITIVE_SIMILAR = true;
        public static bool TRIANGLE_PROPORTIONALITY = true;
        public static bool TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT = true;
        public static bool VERTICAL_ANGLES = true;

        // Quadrilateral
        public static bool OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT = true;
        public static bool OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT = true;
        public static bool DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER = true;
        public static bool OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM = true;
        public static bool ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM = true;
        public static bool OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM = true;
        public static bool DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM = true;
        public static bool DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT = true;
        public static bool DIAGONALS_OF_KITE_ARE_PERPENDICULAR = true;
        public static bool DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR = true;
        public static bool DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS = true;
        public static bool TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS = true;
        public static bool BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT = true;
        public static bool MEDIAN_TRAPEZOID_PARALLEL_TO_BASE = true;
        public static bool MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES = true;

        // Circles
        public static bool CIRCLE_DEFINITION = true;
        public static bool TANGENT_IS_PERPENDICULAR_TO_RADIUS = true;
        public static bool PERPENDICULAR_TO_RADIUS_IS_TANGENT = true;
        public static bool TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT = true;
        public static bool MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT = true;
        public static bool CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT = true;
        public static bool CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS = true;
        public static bool CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS = true;
        public static bool DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC = true;
        public static bool MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC = true;
        public static bool CIRCLE_CONGRUENCE_DEFINITION = true;
        public static bool ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT = true;
        public static bool CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC = true;
        public static bool CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC = true;
        public static bool EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS = true;
        public static bool TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS = true;
        public static bool TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES = true;
        public static bool INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY = true;

        //
        // General Switches
        //
        public static bool QUADRILATERALS = true;
        public static bool SIMILARITY = true;
        public static bool TRIANGLE_CONGREUNCE = true;
        public static bool REFLEXIVE = true;

        //
        // Based on user input, we can turn on or off the stated justifications
        //
        public static void SetAssumptions(Dictionary<DeductionJustType, Assumption> assumptions)
        {
            foreach (KeyValuePair<DeductionJustType, Assumption> assumption in assumptions)
            {
                // If the technique is turned off
                if (!assumption.Value.Enabled)
                {
                    //
                    // Handle general switches first
                    //
                    if (assumption.Key == DeductionJustType.CIRCLES)
                    {
                        EngineUIBridge.JustificationSwitch.ARC_ADDITION_AXIOM = false;
                        EngineUIBridge.JustificationSwitch.TANGENT_IS_PERPENDICULAR_TO_RADIUS = false;
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_TO_RADIUS_IS_TANGENT = false;
                        EngineUIBridge.JustificationSwitch.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT = false;
                        EngineUIBridge.JustificationSwitch.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS = false;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS = false;
                        EngineUIBridge.JustificationSwitch.DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC = false;
                        EngineUIBridge.JustificationSwitch.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC = false;
                        EngineUIBridge.JustificationSwitch.CIRCLE_CONGRUENCE_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.CIRCLE_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT = false;
                        EngineUIBridge.JustificationSwitch.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC = false;
                        EngineUIBridge.JustificationSwitch.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC = false;
                        EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS = false;
                        EngineUIBridge.JustificationSwitch.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS = false;
                        EngineUIBridge.JustificationSwitch.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES = false;
                        EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY = false;
                    }
                    if (assumption.Key == DeductionJustType.QUADRILATERALS)
                    {
                        EngineUIBridge.JustificationSwitch.QUADRILATERAL_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.KITE_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.PARALLELOGRAM_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.RHOMBUS_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.SQUARE_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.RECTANGLE_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.TRAPEZOID_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRAPEZOID_DEFINITION = false;

                        // Theorems
                        EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER = false;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM = false;
                        EngineUIBridge.JustificationSwitch.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM = false;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_KITE_ARE_PERPENDICULAR = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR = false;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS = false;
                        EngineUIBridge.JustificationSwitch.TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS = false;
                        EngineUIBridge.JustificationSwitch.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT = false;
                        EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE = false;
                        EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES = false;
                        EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY = false;
                    }
                    if (assumption.Key == DeductionJustType.TRIANGLE_CONGREUNCE)
                    {
                        EngineUIBridge.JustificationSwitch.AAS = false;
                        EngineUIBridge.JustificationSwitch.ASA = false;
                        EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG = false;
                        EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE = false;
                        EngineUIBridge.JustificationSwitch.SSS = false;
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.AA_SIMILARITY = false;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.SAS_SIMILARITY = false;
                        EngineUIBridge.JustificationSwitch.SSS_SIMILARITY = false;
                        EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY = false;
                    }
                    else if (assumption.Key == DeductionJustType.AA_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.AA_SIMILARITY = false;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_ADDITION_AXIOM)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_ADDITION_AXIOM = false;
                    }
                    else if (assumption.Key == DeductionJustType.ASA)
                    {
                        EngineUIBridge.JustificationSwitch.ASA = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL = false;
                    }
                    else if (assumption.Key == DeductionJustType.CORRESPONDING_ANGLES_OF_PARALLEL_LINES)
                    {
                        EngineUIBridge.JustificationSwitch.CORRESPONDING_ANGLES_OF_PARALLEL_LINES = false;

                    }
                    else if (assumption.Key == DeductionJustType.SEGMENT_ADDITION_AXIOM)
                    {
                        EngineUIBridge.JustificationSwitch.SEGMENT_ADDITION_AXIOM = false;
                    }
                    else if (assumption.Key == DeductionJustType.SAS_CONGRUENCE)
                    {
                        EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE = false;
                    }
                    else if (assumption.Key == DeductionJustType.SSS)
                    {
                        EngineUIBridge.JustificationSwitch.SSS = false;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE = false;
                    }
                    else if (assumption.Key == DeductionJustType.ALTITUDE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ALTITUDE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.COMPLEMENTARY_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.COMPLEMENTARY_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.ISOSCELES_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.EQUILATERAL_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.MEDIAN_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.MEDIAN_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.MIDPOINT_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.MIDPOINT_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_DEFINITION = false;
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.RIGHT_ANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.RIGHT_ANGLE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.RIGHT_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.SEGMENT_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.SEGMENT_BISECTOR_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.SUPPLEMENTARY_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.SUPPLEMENTARY_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.STRAIGHT_ANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.STRAIGHT_ANGLE_DEFINITION = false;
                    }
                    else if (assumption.Key == DeductionJustType.AAS)
                    {
                        EngineUIBridge.JustificationSwitch.AAS = false;
                    }
                    else if (assumption.Key == DeductionJustType.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY = false;
                    }
                    else if (assumption.Key == DeductionJustType.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY = false;
                    }
                    else if (assumption.Key == DeductionJustType.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL = false;
                    }
                    else if (assumption.Key == DeductionJustType.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR)
                    {
                        EngineUIBridge.JustificationSwitch.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR = false;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES = false;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_THEOREM = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES = false;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.HYPOTENUSE_LEG)
                    {
                        EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG = false;
                    }
                    else if (assumption.Key == DeductionJustType.ISOSCELES_TRIANGLE_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_THEOREM = false;
                    }
                    else if (assumption.Key == DeductionJustType.MIDPOINT_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.MIDPOINT_THEOREM = false;
                    }
                    else if (assumption.Key == DeductionJustType.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY = false;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT = false;
                    }
                    else if (assumption.Key == DeductionJustType.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL = false;
                    }
                    else if (assumption.Key == DeductionJustType.SAS_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.SAS_SIMILARITY = false;
                    }
                    else if (assumption.Key == DeductionJustType.SSS_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.SSS_SIMILARITY = false;
                    }
                    else if (assumption.Key == DeductionJustType.SUM_ANGLES_IN_TRIANGLE_180)
                    {
                        EngineUIBridge.JustificationSwitch.SUM_ANGLES_IN_TRIANGLE_180 = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_CONGRUENT_TRIANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_PARALLEL = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_SIMILAR)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_SIMILAR = false;
                    }
                    else if (assumption.Key == DeductionJustType.TRIANGLE_PROPORTIONALITY)
                    {
                        EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY = false;
                    }
                    else if (assumption.Key == DeductionJustType.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT = false;
                    }
                    else if (assumption.Key == DeductionJustType.VERTICAL_ANGLES) { EngineUIBridge.JustificationSwitch.VERTICAL_ANGLES = false; }
                }



                else if (assumption.Value.Enabled)
                {
                    //
                    // Handle general switches first
                    //
                    if (assumption.Key == DeductionJustType.CIRCLES)
                    {
                        EngineUIBridge.JustificationSwitch.ARC_ADDITION_AXIOM = true;
                        EngineUIBridge.JustificationSwitch.TANGENT_IS_PERPENDICULAR_TO_RADIUS = true;
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_TO_RADIUS_IS_TANGENT = true;
                        EngineUIBridge.JustificationSwitch.TANGENT_TO_CIRCLE_ARE_CONGRUENT_FROM_SAME_POINT = true;
                        EngineUIBridge.JustificationSwitch.MINOR_ARCS_CONGRUENT_IF_CENTRAL_ANGLE_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.CENTRAL_ANGLES_CONGRUENT_IF_MINOR_ARCS_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_CHORDS_HAVE_CONGRUENT_ARCS = true;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ARCS_HAVE_CONGRUENT_CHORDS = true;
                        EngineUIBridge.JustificationSwitch.DIAMETER_PERPENDICULAR_TO_CHORD_BISECTS_CHORD_AND_ARC = true;
                        EngineUIBridge.JustificationSwitch.MEASURE_INSCRIBED_ANGLE_EQUAL_HALF_INTERCEPTED_ARC = true;
                        EngineUIBridge.JustificationSwitch.CIRCLE_CONGRUENCE_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.CIRCLE_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.ANGLE_INSCRIBED_SEMICIRCLE_IS_RIGHT = true;
                        EngineUIBridge.JustificationSwitch.CENTRAL_ANGLE_EQUAL_MEASURE_INTERCEPTED_ARC = true;
                        EngineUIBridge.JustificationSwitch.CHORD_AND_TANGENT_ANGLE_IS_HALF_INTERCEPTED_ARC = true;
                        EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_HALF_DIFFERENCE_INTERCEPTED_ARCS = true;
                        EngineUIBridge.JustificationSwitch.TWO_INTERSECTING_CHORDS_ANGLE_MEASURE_HALF_SUM_INTERCEPTED_ARCS = true;
                        EngineUIBridge.JustificationSwitch.TWO_INTERCEPTED_ARCS_HAVE_CONGRUENT_ANGLES = true;
                        EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY = true;
                    }
                    if (assumption.Key == DeductionJustType.QUADRILATERALS)
                    {
                        EngineUIBridge.JustificationSwitch.QUADRILATERAL_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.KITE_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.PARALLELOGRAM_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.RHOMBUS_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.SQUARE_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.RECTANGLE_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.TRAPEZOID_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRAPEZOID_DEFINITION = true;

                        // Theorems
                        EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_PARALLELOGRAM_ARE_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_PARALLELOGRAM_ARE_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_PARALLELOGRAM_BISECT_EACH_OTHER = true;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_SIDES_CONGRUENT_IMPLIES_PARALLELOGRAM = true;
                        EngineUIBridge.JustificationSwitch.ONE_PAIR_OPPOSITE_SIDES_CONGRUENT_PARALLEL_IMPLIES_PARALLELOGRAM = true;
                        EngineUIBridge.JustificationSwitch.OPPOSITE_ANGLES_CONGRUENT_IMPLIES_PARALLELOGRAM = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_BISECT_EACH_OTHER_IMPLY_PARALLELOGRAM = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RECTANGLE_ARE_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_KITE_ARE_PERPENDICULAR = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_ARE_PERPENDICULAR = true;
                        EngineUIBridge.JustificationSwitch.DIAGONALS_OF_RHOMBUS_BISECT_ANGLES_OF_RHOMBUS = true;
                        EngineUIBridge.JustificationSwitch.TWO_CONSECUTIVE_SIDES_OF_PARALLELOGRAM_CONGRUENT_IMPLY_RHOMBUS = true;
                        EngineUIBridge.JustificationSwitch.BASE_ANGLES_OF_ISOSCELES_TRAPEZOID_CONGRUENT = true;
                        EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_PARALLEL_TO_BASE = true;
                        EngineUIBridge.JustificationSwitch.MEDIAN_TRAPEZOID_LENGTH_HALF_SUM_BASES = true;
                        EngineUIBridge.JustificationSwitch.INSCRIBED_QUADRILATERAL_OPPOSITE_ANGLES_SUPPLEMENTARY = true;
                    }
                    if (assumption.Key == DeductionJustType.TRIANGLE_CONGREUNCE)
                    {
                        EngineUIBridge.JustificationSwitch.AAS = true;
                        EngineUIBridge.JustificationSwitch.ASA = true;
                        EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG = true;
                        EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE = true;
                        EngineUIBridge.JustificationSwitch.SSS = true;
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.AA_SIMILARITY = true;
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.SAS_SIMILARITY = true;
                        EngineUIBridge.JustificationSwitch.SSS_SIMILARITY = true;
                        EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY = true;
                    }
                    else if (assumption.Key == DeductionJustType.AA_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.AA_SIMILARITY = true;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_ADDITION_AXIOM)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_ADDITION_AXIOM = true;
                    }
                    else if (assumption.Key == DeductionJustType.ASA)
                    {
                        EngineUIBridge.JustificationSwitch.ASA = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_CORRESPONDING_ANGLES_IMPLY_PARALLEL = true;
                    }
                    else if (assumption.Key == DeductionJustType.CORRESPONDING_ANGLES_OF_PARALLEL_LINES)
                    {
                        EngineUIBridge.JustificationSwitch.CORRESPONDING_ANGLES_OF_PARALLEL_LINES = true;

                    }
                    else if (assumption.Key == DeductionJustType.SEGMENT_ADDITION_AXIOM)
                    {
                        EngineUIBridge.JustificationSwitch.SEGMENT_ADDITION_AXIOM = true;
                    }
                    else if (assumption.Key == DeductionJustType.SAS_CONGRUENCE)
                    {
                        EngineUIBridge.JustificationSwitch.SAS_CONGRUENCE = true;
                    }
                    else if (assumption.Key == DeductionJustType.SSS)
                    {
                        EngineUIBridge.JustificationSwitch.SSS = true;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLES_OF_EQUAL_MEASUREARE_CONGRUENT = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_ANGLE_WITH_RIGHT_ANGLE = true;
                    }
                    else if (assumption.Key == DeductionJustType.ALTITUDE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ALTITUDE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.COMPLEMENTARY_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.COMPLEMENTARY_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SEGMENTS_IMPLY_PROPORTIONAL_SEGMENTS_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.ISOSCELES_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.EQUILATERAL_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.MEDIAN_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.MEDIAN_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.MIDPOINT_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.MIDPOINT_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_DEFINITION = true;
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_BISECTOR_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.RIGHT_ANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.RIGHT_ANGLE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.RIGHT_TRIANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.RIGHT_TRIANGLE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.SEGMENT_BISECTOR_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.SEGMENT_BISECTOR_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.SUPPLEMENTARY_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.SUPPLEMENTARY_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.STRAIGHT_ANGLE_DEFINITION)
                    {
                        EngineUIBridge.JustificationSwitch.STRAIGHT_ANGLE_DEFINITION = true;
                    }
                    else if (assumption.Key == DeductionJustType.AAS)
                    {
                        EngineUIBridge.JustificationSwitch.AAS = true;
                    }
                    else if (assumption.Key == DeductionJustType.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.ACUTE_ANGLES_IN_RIGHT_TRIANGLE_ARE_COMPLEMENTARY = true;
                    }
                    else if (assumption.Key == DeductionJustType.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.ADJACENT_ANGLES_PERPENDICULAR_IMPLY_COMPLEMENTARY = true;
                    }
                    else if (assumption.Key == DeductionJustType.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.ALT_INT_CONGRUENT_ANGLES_IMPLY_PARALLEL = true;
                    }
                    else if (assumption.Key == DeductionJustType.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR)
                    {
                        EngineUIBridge.JustificationSwitch.ALTITUDE_OF_RIGHT_TRIANGLES_IMPLIES_SIMILAR = true;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_IS_PERPENDICULAR_BISECTOR_IN_ISOSCELES = true;
                    }
                    else if (assumption.Key == DeductionJustType.ANGLE_BISECTOR_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.ANGLE_BISECTOR_THEOREM = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SUPPLEMENTARY_ANGLES_IMPLY_RIGHT_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ADJACENT_ANGLES_IMPLY_PERPENDICULAR = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_ANGLES_IN_TRIANGLE_IMPLY_CONGRUENT_SIDES = true;
                    }
                    else if (assumption.Key == DeductionJustType.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.CONGRUENT_SIDES_IN_TRIANGLE_IMPLY_CONGRUENT_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.EXTERIOR_ANGLE_EQUAL_SUM_REMOTE_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.EQUILATERAL_TRIANGLE_HAS_SIXTY_DEGREE_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.HYPOTENUSE_LEG)
                    {
                        EngineUIBridge.JustificationSwitch.HYPOTENUSE_LEG = true;
                    }
                    else if (assumption.Key == DeductionJustType.ISOSCELES_TRIANGLE_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.ISOSCELES_TRIANGLE_THEOREM = true;
                    }
                    else if (assumption.Key == DeductionJustType.MIDPOINT_THEOREM)
                    {
                        EngineUIBridge.JustificationSwitch.MIDPOINT_THEOREM = true;
                    }
                    else if (assumption.Key == DeductionJustType.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_ALT_INT_CONGRUENT_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY)
                    {
                        EngineUIBridge.JustificationSwitch.PARALLEL_IMPLY_SAME_SIDE_INTERIOR_SUPPLEMENTARY = true;
                    }
                    else if (assumption.Key == DeductionJustType.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.PERPENDICULAR_IMPLY_CONGRUENT_ADJACENT_ANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.RELATIONS_OF_CONGRUENT_ANGLES_ARE_CONGRUENT = true;
                    }
                    else if (assumption.Key == DeductionJustType.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.SAME_SIDE_SUPPLE_ANGLES_IMPLY_PARALLEL = true;
                    }
                    else if (assumption.Key == DeductionJustType.SAS_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.SAS_SIMILARITY = true;
                    }
                    else if (assumption.Key == DeductionJustType.SSS_SIMILARITY)
                    {
                        EngineUIBridge.JustificationSwitch.SSS_SIMILARITY = true;
                    }
                    else if (assumption.Key == DeductionJustType.SUM_ANGLES_IN_TRIANGLE_180)
                    {
                        EngineUIBridge.JustificationSwitch.SUM_ANGLES_IN_TRIANGLE_180 = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSVERSAL_PERPENDICULAR_TO_PARALLEL_IMPLY_BOTH_PERPENDICULAR = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_CONGRUENT_TRIANGLES)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_CONGRUENT_TRIANGLES = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_PARALLEL)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_PARALLEL = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRANSITIVE_SIMILAR)
                    {
                        EngineUIBridge.JustificationSwitch.TRANSITIVE_SIMILAR = true;
                    }
                    else if (assumption.Key == DeductionJustType.TRIANGLE_PROPORTIONALITY)
                    {
                        EngineUIBridge.JustificationSwitch.TRIANGLE_PROPORTIONALITY = true;
                    }
                    else if (assumption.Key == DeductionJustType.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT)
                    {
                        EngineUIBridge.JustificationSwitch.TWO_PAIRS_CONGRUENT_ANGLES_IMPLY_THIRD_PAIR_CONGRUENT = true;
                    }
                    else if (assumption.Key == DeductionJustType.VERTICAL_ANGLES) { EngineUIBridge.JustificationSwitch.VERTICAL_ANGLES = true; }
                }
            }
        }
    }
}
