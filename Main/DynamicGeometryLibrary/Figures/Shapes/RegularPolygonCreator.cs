﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace DynamicGeometry
{
    [Category(BehaviorCategories.Shapes)]
    [Order(7)]
    public class RegularPolygonCreator : FigureCreator
    {
        protected override IEnumerable<IFigure> CreateFigures()
        {
            yield return Factory.CreateRegularPolygon(Drawing, FoundDependencies);
        }

        protected override DependencyList InitExpectedDependencies()
        {
            return DependencyList.PointPoint;
        }

        public override string Name
        {
            get
            {
                return "Regular polygon";
            }
        }

        public override string HintText
        {
            get
            {
                return "Click the polygon center and then click a vertex.";
            }
        }

        public override FrameworkElement CreateIcon()
        {
            return IconBuilder.BuildIcon()
                .Polygon(
                    new SolidColorBrush(Color.FromArgb(255, 128, 255, 128)),
                    new SolidColorBrush(Colors.Black),
                    new Point(0.68, 0.98),
                    new Point(1.01, 0.63),
                    new Point(0.875, 0.166),
                    new Point(0.401, 0.055),
                    new Point(0.068, 0.409),
                    new Point(0.208, 0.874))
                .Line(0.68, 0.98, 1.01, 0.63)
                .Line(1.01, 0.63, 0.875, 0.166)
                .Line(0.875, 0.166, 0.401, 0.055)
                .Line(0.401, 0.055, 0.068, 0.409)
                .Line(0.068, 0.409, 0.208, 0.874)
                .Line(0.208, 0.874, 0.68, 0.98)
                .Point(0.68, 0.98)
                .Point(1.01, 0.63)
                .Point(0.875, 0.166)
                .Point(0.401, 0.055)
                .DependentPoint(0.068, 0.409)
                .Point(0.208, 0.874)
                .DependentPoint(0.55, 0.5)
                .Canvas;
        }
    }
}