using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddSimilarTriangles : AddGivenWindow
    {
        private const double EPSILON_ANGLE = 0.1;

        private ComboBox triangle1, triangle2;
        private Dictionary<Triangle, List<Triangle>> options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddSimilarTriangles() : base("Similar Triangles")
        {
        }

        protected override Grid MakeGivenGrid()
        {
            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Create the description text
            TextBlock desc = new TextBlock();
            desc.TextWrapping = TextWrapping.Wrap;
            desc.Text = "Specify two triangles as being similar.";
            desc.Margin = new Thickness(0, 0, 0, 10);

            //Create the first combo box
            StackPanel panel1 = new StackPanel();
            panel1.Margin = new Thickness(0, 0, 10, 0);
            TextBlock label1 = new TextBlock();
            label1.Text = "Triangle 1:";
            panel1.Children.Add(label1);
            triangle1 = new ComboBox();
            triangle1.MinWidth = 150;
            triangle1.SelectionChanged += new SelectionChangedEventHandler(Triangle1_SelectionChanged);
            panel1.Children.Add(triangle1);

            //Create the second combo box
            StackPanel panel2 = new StackPanel();
            TextBlock label2 = new TextBlock();
            label2.Text = "Triangle 2:";
            panel2.Children.Add(label2);
            triangle2 = new ComboBox();
            triangle2.MinWidth = 150;
            panel2.Children.Add(triangle2);

            //Align elements in grid and add them to it
            Grid.SetColumn(desc, 0);
            Grid.SetColumnSpan(desc, 2);
            Grid.SetRow(desc, 0);
            grid.Children.Add(desc);
            Grid.SetColumn(panel1, 0);
            Grid.SetRow(panel1, 1);
            grid.Children.Add(panel1);
            Grid.SetColumn(panel2, 1);
            Grid.SetRow(panel2, 1);
            grid.Children.Add(panel2);

            return grid;
        }

        /// <summary>
        /// Figure out what possible triangle combinations are before showing the window.
        /// </summary>
        protected override void OnShow()
        {
            options = new Dictionary<Triangle, List<Triangle>>();

            //Get a list of all congruent segment givens
            List<GroundedClause> stris = new List<GroundedClause>();
            foreach (GroundedClause gc in currentGivens)
            {
                GeometricSimilarTriangles stri = gc as GeometricSimilarTriangles;
                if (stri != null)
                {
                    stris.Add(stri);
                }
            }

            // Pick a first triangle...
            foreach (Triangle t1 in parser.backendParser.implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX])
            {
                List<Triangle> possible = new List<Triangle>();

                //... and see what other triangles are viable second options.
                foreach (Triangle t2 in parser.backendParser.implied.polygons[GeometryTutorLib.ConcreteAST.Polygon.TRIANGLE_INDEX])
                {
                    if (isSimilar(t1, t2))
                    {
                        GeometricSimilarTriangles stri = new GeometricSimilarTriangles(t1, t2);

                        if (!t1.StructurallyEquals(t2) && !StructurallyContains(stris, stri))
                        {
                            possible.Add(t2);
                        }
                    }
                }

                //If we found a possible list of combinations, add it to the dictionary
                if (possible.Count > 0)
                {
                    options.Add(t1, possible);
                }
            }

            //Set the options of the segment1 combo box
            triangle1.ItemsSource = null; //Graphical refresh
            triangle1.ItemsSource = options.Keys;
        }

        /// <summary>
        /// Checks to see if two triangles are similar. (All angles have equal measure)
        /// </summary>
        /// <param name="t1">A triangle</param>
        /// <param name="t2">A triangle</param>
        /// <returns>true if the triangles are similar, false otherwise.</returns>
        private bool isSimilar(Triangle t1, Triangle t2)
        {
            //Convert each triangle into an array of angles, such that angles i % n and i+1 % n are adjancent.
            Angle[] angles1 = { t1.AngleA, t1.AngleB, t1.AngleC };
            Angle[] angles2 = { t2.AngleA, t2.AngleB, t2.AngleC };

            //Pick an angle of triangle 1 and compare it to each side of triangle 2
            for (int i = 0; i < 3; i++)
            {
                if (System.Math.Abs(angles1[0].measure - angles2[i].measure) < EPSILON_ANGLE) //See if they have the same length
                {
                    //We need to compare angles in each direction. Start in the forward direction.
                    bool pass = true;
                    for (int j = 1; j < 3; j++)
                    {
                        pass = System.Math.Abs(angles1[j].measure - angles2[(i + j) % 3].measure) < EPSILON_ANGLE && pass;
                    }

                    if (pass)
                    {
                        return true;
                    }

                    //If the previous direction failed, check the backwards direction.
                    pass = true;
                    for (int j = 1; j < 3; j++)
                    {
                        pass = System.Math.Abs(angles1[j].measure - angles2[((i - j) + 2) % 3].measure) < EPSILON_ANGLE && pass;
                    }

                    if (pass)
                    {
                        return true;
                    }

                    //Both directions failed. Containue checking angles of triangle 2.
                }
            }

            //Both directions failed for all angles of triangle 2.
            return false;
        }

        /// <summary>
        /// This event is called when the triangle1 combo box changes its selction.
        /// The method will update the triangle2 combo box to reflect viable combinations with triangle1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Triangle1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            triangle2.ItemsSource = null;
            Triangle selection = triangle1.SelectedValue as Triangle;
            if (selection != null)
            {
                triangle2.ItemsSource = options[selection];
            }
        }

        protected override GroundedClause MakeClause()
        {
            Triangle t1 = triangle1.SelectedValue as Triangle;
            Triangle t2 = triangle2.SelectedValue as Triangle;
            if (t1 != null && t2 != null)
            {
                return new GeometricSimilarTriangles(t1, t2);
            }
            else
            {
                return null;
            }
        }
    }
}
