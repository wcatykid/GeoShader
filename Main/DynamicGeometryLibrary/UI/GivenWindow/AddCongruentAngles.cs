using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddCongruentAngles : AddGivenWindow
    {
        private ComboBox angle1, angle2;
        private Dictionary<Angle, List<Angle>> options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddCongruentAngles() : base("Congruent Angles")
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
            desc.Text = "Specify two angles as being congruent.";
            desc.Margin = new Thickness(0, 0, 0, 10);

            //Create the first combo box
            StackPanel panel1 = new StackPanel();
            panel1.Margin = new Thickness(0, 0, 10, 0);
            TextBlock label1 = new TextBlock();
            label1.Text = "Angle 1:";
            panel1.Children.Add(label1);
            angle1 = new ComboBox();
            angle1.MinWidth = 150;
            angle1.SelectionChanged += new SelectionChangedEventHandler(Angle1_SelectionChanged);
            panel1.Children.Add(angle1);

            //Create the second combo box
            StackPanel panel2 = new StackPanel();
            TextBlock label2 = new TextBlock();
            label2.Text = "Angle 2:";
            panel2.Children.Add(label2);
            angle2 = new ComboBox();
            angle2.MinWidth = 150;
            panel2.Children.Add(angle2);

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
        /// Figure out what possible angle combinations are before showing the window.
        /// </summary>
        protected override void OnShow()
        {
            options = new Dictionary<Angle, List<Angle>>();

            //Get a list of all congruent angle givens
            List<GroundedClause> cangs = new List<GroundedClause>();
            foreach (GroundedClause gc in currentGivens)
            {
                CongruentAngles cang = gc as CongruentAngles;
                if (cang != null)
                {
                    cangs.Add(cang);
                }
            }

            //Pick a first angle...
            foreach (Angle a1 in parser.backendParser.implied.angles)
            {
                List<Angle> possible = new List<Angle>();

                //... and see what other angles are viable second options.
                foreach (Angle a2 in parser.backendParser.implied.angles)
                {
                    if (a1.measure == a2.measure)
                    {
                        CongruentAngles cang = new CongruentAngles(a1, a2);

                        if (!a1.StructurallyEquals(a2) && !StructurallyContains(cangs, cang))
                        {
                            possible.Add(a2);
                        }
                    }
                }

                //If we found a possible list of combinations, add it to the dictionary
                if (possible.Count > 0)
                {
                    options.Add(a1, possible);
                }
            }

            //Set the options of the angle1 combo box
            angle1.ItemsSource = null; //Graphical refresh
            angle1.ItemsSource = options.Keys;
        }

        /// <summary>
        /// This event is called when the angle1 combo box changes its selction.
        /// The method will update the angle2 combo box to reflect viable combinations with angle1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Angle1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            angle2.ItemsSource = null;
            Angle selection = angle1.SelectedValue as Angle;
            if (selection != null)
            {
                angle2.ItemsSource = options[selection];
            }
        }

        protected override GroundedClause MakeClause()
        {
            Angle a1 = angle1.SelectedValue as Angle;
            Angle a2 = angle2.SelectedValue as Angle;
            if (a1 != null && a2 != null)
            {
                return new GeometricCongruentAngles(a1, a2);
            }
            else
            {
                return null;
            }
        }
    }
}
