using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddIsoscelesTriangle : AddGivenWindow
    {
        private ComboBox options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddIsoscelesTriangle() : base("Isosceles Triangle")
        {
        }

        protected override Grid MakeGivenGrid()
        {
            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Create the description text
            TextBlock desc = new TextBlock();
            desc.TextWrapping = TextWrapping.Wrap;
            desc.Text = "Specify a triangle as being an isosceles triangle.";
            desc.Margin = new Thickness(0, 0, 0, 10);

            //Create a combo box to choose from
            options = new ComboBox();
            options.MinWidth = 200;

            //Align elements in grid and add them to it
            Grid.SetColumn(desc, 0);
            Grid.SetRow(desc, 0);
            grid.Children.Add(desc);
            Grid.SetColumn(options, 0);
            Grid.SetRow(options, 1);
            grid.Children.Add(options);

            return grid;
        }

        /// <summary>
        /// Figure out which triangles we can choose from before the window is shown.
        /// </summary>
        protected override void OnShow()
        {
            List<GroundedClause> givens = new List<GroundedClause>();
            //Populate list with applicable givens
            foreach (GroundedClause gc in currentGivens)
            {
                IsoscelesTriangle it = gc as IsoscelesTriangle;
                if (it != null)
                {
                    givens.Add(it);
                }
            }

            List<Triangle> isosTriangles = new List<Triangle>();
            //Populate list with possible choices
            foreach (Triangle t in parser.backendParser.implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX])
            {
                if (isIsosceles(t))
                {
                    IsoscelesTriangle it = new IsoscelesTriangle(t);
                    if (!StructurallyContains(givens, it))
                    {
                        isosTriangles.Add(it);
                    }
                }
            }

            options.ItemsSource = null; //Makes sure the box is graphically updated.
            options.ItemsSource = isosTriangles;
        }

        protected override GroundedClause MakeClause()
        {
            if (options.SelectedValue == null)
            {
                return null;
            }
            else
            {
                return new IsoscelesTriangle(options.SelectedValue as Triangle);
            }
        }

        /// <summary>
        /// Tests to see if a triangle is isosceles
        /// </summary>
        /// <param name="t">The triangle to test</param>
        /// <returns>true if at least two sides have equal length</returns>
        private bool isIsosceles(Triangle t)
        {
            return (t.SegmentA.Length == t.SegmentB.Length) || (t.SegmentA.Length == t.SegmentC.Length) || (t.SegmentB.Length == t.SegmentC.Length);
        }
    }
}
