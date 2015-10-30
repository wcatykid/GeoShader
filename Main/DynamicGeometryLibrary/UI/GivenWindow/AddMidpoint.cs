using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddMidpoint : AddGivenWindow
    {
        private ComboBox optionsBox;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddMidpoint() : base("Congruent Segments")
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
            desc.Text = "Specify a midpoint.";
            desc.Margin = new Thickness(0, 0, 0, 10);

            //Create a combo box to choose from
            optionsBox = new ComboBox();
            optionsBox.MinWidth = 200;

            //Align elements in grid and add them to it
            Grid.SetColumn(desc, 0);
            Grid.SetRow(desc, 0);
            grid.Children.Add(desc);
            Grid.SetColumn(optionsBox, 0);
            Grid.SetRow(optionsBox, 1);
            grid.Children.Add(optionsBox);

            return grid;
        }

        /// <summary>
        /// Figure out what possible segment combinations are before showing the window.
        /// </summary>
        protected override void OnShow()
        {
            var options = new List<Midpoint>();
            List<GroundedClause> current = new List<GroundedClause>();

            //Populate current midpoint givens.
            foreach (GroundedClause gc in currentGivens)
            {
                Midpoint m = gc as Midpoint;
                if (m != null)
                {
                    current.Add(m);
                }
            }

            //Each inmiddle that can be strengthened to a midpoint that is not already a given is an option.
            foreach (InMiddle im in parser.backendParser.implied.inMiddles)
            {
                Strengthened s = im.CanBeStrengthened();
                if (s != null)
                {
                    Midpoint m = s.strengthened as Midpoint;
                    if (!StructurallyContains(current, m))
                    {
                        options.Add(m);
                    }
                }
            }

            optionsBox.ItemsSource = null; //Makes sure the box is graphically updated.
            optionsBox.ItemsSource = options;
        }

        protected override GroundedClause MakeClause()
        {
            if (optionsBox.SelectedValue == null)
            {
                return null;
            }
            else
            {
                return new Midpoint(optionsBox.SelectedValue as Midpoint);
            }
        }
    }
}
