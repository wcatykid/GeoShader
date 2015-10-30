using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddEquilateralTriangle : AddGivenWindow
    {
        private ComboBox options;
        public const double EPSILON_ANGLE = 0.1;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddEquilateralTriangle() : base("Equilateral Triangle")
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
            desc.Text = "Specify a triangle as being an equilateral triangle.";
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
                EquilateralTriangle et = gc as EquilateralTriangle;
                if (et != null)
                {
                    givens.Add(et);
                }
            }

            List<Triangle> equiTriangles = new List<Triangle>();
            //Populate list with possible choices
            foreach (Triangle t in parser.backendParser.implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX])
            {
                if (isEquilateral(t))
                {
                    EquilateralTriangle et = new EquilateralTriangle(t);
                    if (!StructurallyContains(givens, et))
                    {
                        equiTriangles.Add(et);
                    }
                }
            }

            options.ItemsSource = null; //Makes sure the box is graphically updated.
            options.ItemsSource = equiTriangles;
        }

        protected override GroundedClause MakeClause()
        {
            if (options.SelectedValue == null)
            {
                return null;
            }
            else
            {
                return new EquilateralTriangle(options.SelectedValue as Triangle);
            }
        }

        /// <summary>
        /// Tests to see if a triangle is equilateral
        /// </summary>
        /// <param name="t">The triangle to test</param>
        /// <returns>true if all angles are 60 degrees</returns>
        private bool isEquilateral(Triangle t)
        {
            return t is EquilateralTriangle || //If the tool was used, this will be true.
                (Math.Abs(t.AngleA.measure - 60) < EPSILON_ANGLE &&
                Math.Abs(t.AngleB.measure - 60) < EPSILON_ANGLE &&
                Math.Abs(t.AngleC.measure - 60) < EPSILON_ANGLE);
        }
    }
}
