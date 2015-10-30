using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using GeometryTutorLib.Area_Based_Analyses.Atomizer;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib
{
    public enum TemplateType
    {
        // Two Shapes
        ALPHA_MINUS_BETA,                           // a - b
        ALPHA_PLUS_BETA,                            // a + b

        // Marker between 2 and 3 elements in the template
        DEMARCATION,

        // Three Shapes
        ALPHA_PLUS_BETA_PLUS_GAMMA,                 // a + b + c
        ALPHA_PLUS_LPAREN_BETA_MINUS_GAMMA_RPAREN,  // a + (b - c)
        LPAREN_ALPHA_PLUS_BETA_RPAREN_MINUS_GAMMA,  // (a + b) - c
        ALPHA_MINUS_BETA_MINUS_GAMMA,               // a - b - c
        ALPHA_MINUS_BETA_PLUS_GAMMA,                // a - b + c
        ALPHA_MINUS_LPAREN_BETA_MINUS_GAMMA_RPAREN, // a - (b - c)
    }

    public enum ShapeType
    {
        TRIANGLE = 0,
        ISOSCELES_TRIANGLE = 1,
        RIGHT_TRIANGLE = 2,
        ISO_RIGHT_TRIANGLE = 3,
        EQUILATERAL_TRIANGLE = 4,

        TRI_DEMARCATION = 5,

        QUADRILATERAL = 6,
        KITE = 7,
        TRAPEZOID = 8,
        ISO_TRAPEZOID = 9,
        PARALLELOGRAM = 10,
        RECTANGLE = 11,
        RHOMBUS = 12,
        SQUARE = 13,

        QUAD_DEMARCATION=14,

        CIRCLE = 15,
        SECTOR = 16
    }

    public static partial class FigureSynthesizerMain
    {

        //
        // Converts the shape map to a list of shapes to process IN ORDER.
        //
        private static List<List<ShapeType>> ConvertShapeMapToList(Dictionary<ShapeType, int> figureCountMap)
        {
            List<ShapeType> shapes = new List<ShapeType>();

            foreach (KeyValuePair<ShapeType, int> pair in figureCountMap)
            {
                // Add the shape X number of times to the list.
                for (int s = 0; s < pair.Value; s++)
                {
                    shapes.Add(pair.Key);
                }
            }

            List<List<ShapeType>> shapeSets = new List<List<ShapeType>>();
            shapeSets.Add(shapes);
            List<ShapeType> reversed = new List<ShapeType>(shapes);
            reversed.Reverse();
            shapeSets.Add(reversed);

            return shapeSets;
            //
            // Use the powerset restriction to acquire all possible orderings.
            //
            //List<List<int>> sets = Utilities.ConstructPowerSetWithNoEmpty(shapes.Count, shapes.Count);

            //List<List<ShapeType>> shapeSets = new List<List<ShapeType>>();
            //foreach (List<int> set in sets)
            //{
            //    List<ShapeType> shapeSet = new List<ShapeType>();
            //    foreach (int index in set)
            //    {
            //        shapeSet.Add(shapes[index]);
            //    }

            //    //
            //    // Only add if this set is not redundant to what exists in this set.
            //    //
            //    bool contains = false;
            //    foreach (List<ShapeType> existent in shapeSets)
            //    {
            //        if (shapeSet.Count == existent.Count)
            //        {
            //            bool equal = false;
            //            for (int i = 0; i < existent.Count; i++)
            //            {
            //                if (shapeSet[i] == existent[i])
            //                {
            //                    equal = true;
            //                    break;
            //                }
            //            }
            //            if (equal)
            //            {
            //                contains = false;
            //                break;
            //            }
            //        }

            //        if (contains) break;
            //    }

            //    if (!contains)
            //    {
            //        shapeSets.Add(shapeSet);

            //        if (Utilities.FIGURE_SYNTHESIZER_DEBUG)
            //        {
            //            string s = "{ "; 
            //            foreach (ShapeType type in shapeSet)
            //            {
            //                s += type + ", ";
            //            }
            //            s += " }";

            //            System.Diagnostics.Debug.WriteLine(s);
            //        }
            //    }
            //}

            //// Return the resultant list of lists.
            //return shapeSets;


            ////
            //// This should change for experimental purposes...
            ////
            //// Sort the shapes based on the order defined by the ShapeType enumeration.
            ////
            ////
            //shapes.Sort();
            //shapes.Reverse();

            //return shapes;
        }

        //
        // Converts the shape map to a list of shapes to process IN ORDER.
        //
        private static Dictionary<ShapeType, int> ConvertListToShapeMap(List<ShapeType> shapeList)
        {
            Dictionary<ShapeType, int> figureCountMap = new Dictionary<ShapeType, int>();

            foreach (ShapeType type in shapeList)
            {
                if (figureCountMap.ContainsKey(type)) figureCountMap[type] = figureCountMap[type] + 1;
                else figureCountMap[type] = 1;
            }

            return figureCountMap;
        }

        //
        // Ensure that the input set of shapes meets the desired application and template parameters.
        //
        public static bool VerifyInputParameters(List<ShapeType> shapeList, TemplateType type)
        {
            // We have an artificial limitation in the number of figures we combine.
            if (shapeList.Count > 3) throw new ArgumentException("Cannot synthesize a figure with more than 3 Figures.");

            if (type.CompareTo(TemplateType.DEMARCATION) < 0 && shapeList.Count != 2)
            {
                throw new ArgumentException("Expected two figures with a synthesis dictacted by template: " + type);
            }

            if (type.CompareTo(TemplateType.DEMARCATION) > 0 && shapeList.Count < 3)
            {
                throw new ArgumentException("Expected three figures with a synthesis dictacted by template: " + type);
            }

            return true;
        }
    }
}