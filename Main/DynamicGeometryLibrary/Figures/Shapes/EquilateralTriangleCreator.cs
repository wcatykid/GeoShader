using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DynamicGeometry.Figures.Shapes
{
    [Category(BehaviorCategories.Shapes)]
    [Order(3)]
    public class EquilateralTriangleCreator : TriangleCreator
    {
        protected override IEnumerable<IFigure> CreateFigures()
        {
            RegularPolygon triangle = Factory.CreateRegularPolygon(Drawing, FoundDependencies);
            //triangle.NumberOfSides = 3;
            yield return triangle;
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        public override string Name
        {
            get { return "Equilateral Triangle"; }
        }

        public override string HintText
        {
            get
            {
                return "Click 3 points to construct a triangle.";
            }
        }

        public override FrameworkElement CreateIcon()
        {
            double sideLength = 1.0;
            return IconBuilder
                .BuildIcon()
                .Polygon(
                    Factory.CreateDefaultFillBrush(),
                    new SolidColorBrush(Colors.Black),
                    new Point(sideLength / 2, 0.0),
                    new Point(0.0, Math.SquareRoot(3) * sideLength / 2),
                    new Point(sideLength, Math.SquareRoot(3) * sideLength / 2))
                .Canvas;
        }
    }
}
