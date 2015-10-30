using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Threading;
using GeometryTutorLib.ConcreteAST;

namespace GeometryTutorLib.TutorParser
{
    /// <summary>
    /// Determine all points of intersection among shapes (circles and polygons)
    /// </summary>
    public class ShapeContainmentCalculator
    {
        private ImpliedComponentCalculator implied;

        public ShapeContainmentCalculator(ImpliedComponentCalculator imp)
        {
            implied = imp;
        }

        /// <summary>
        /// Calculate all points of intersection between circles.
        /// </summary>
        public void CalcCircleCircleContainment()
        {
            for (int c1 = 0; c1 < implied.circles.Count - 1; c1++)
            {
                for (int c2 = c1 + 1; c2 < implied.circles.Count; c2++)
                {
                    if (implied.circles[c1].CircleContains(implied.circles[c2]))
                    {
                        implied.circles[c1].AddSubFigure(implied.circles[c2]);
                        implied.circles[c2].AddSuperFigure(implied.circles[c1]);
                    }
                    else if (implied.circles[c2].CircleContains(implied.circles[c1]))
                    {
                        implied.circles[c2].AddSubFigure(implied.circles[c1]);
                        implied.circles[c1].AddSuperFigure(implied.circles[c2]);
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between circles and polygons.
        /// </summary>
        public void CalcCirclePolygonContainment()
        {
            foreach (GeometryTutorLib.ConcreteAST.Circle circle in implied.circles)
            {
                // Iterate over all polygons
                for (int sidesIndex = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                     sidesIndex < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                     sidesIndex++)
                {
                    foreach (GeometryTutorLib.ConcreteAST.Polygon poly in implied.polygons[sidesIndex])
                    {
                        if (circle.Contains(poly))
                        {
                            circle.AddSubFigure(poly);
                            poly.AddSuperFigure(circle);
                        }
                        else if (poly.Contains(circle))
                        {
                            poly.AddSubFigure(circle);
                            circle.AddSuperFigure(poly);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate all points of intersection between polygons and polygons.
        /// </summary>
        public void CalcPolygonPolygonContainment()
        {
            for (int s1 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                 s1 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                 s1++)
            {
                for (int p1 = 0; p1 < implied.polygons[s1].Count; p1++)
                {
                    for (int s2 = GeometryTutorLib.ConcreteAST.Polygon.MIN_POLY_INDEX;
                         s2 < GeometryTutorLib.ConcreteAST.Polygon.MAX_EXC_POLY_INDEX;
                         s2++)
                    {
                        for (int p2 = 0; p2 < implied.polygons[s2].Count; p2++)
                        {
                            if (s1 != s2 || p1 != p2)
                            {
                                if (implied.polygons[s1][p1].Contains(implied.polygons[s2][p2]))
                                {
                                    implied.polygons[s1][p1].AddSubFigure(implied.polygons[s2][p2]);
                                    implied.polygons[s2][p2].AddSuperFigure(implied.polygons[s1][p1]);
                                }
                                else if (implied.polygons[s2][p2].Contains(implied.polygons[s1][p1]))
                                {
                                    implied.polygons[s2][p2].AddSubFigure(implied.polygons[s1][p1]);
                                    implied.polygons[s1][p1].AddSuperFigure(implied.polygons[s2][p2]);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}