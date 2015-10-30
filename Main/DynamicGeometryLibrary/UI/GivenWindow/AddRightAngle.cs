using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddRightAngle : AddGivenWindow
    {
        private ComboBox options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddRightAngle() : base("Right Angle")
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
            desc.Text = "Specify an angle as being a right angle.";
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
        /// Figure out which angles we can choose from before the window is shown.
        /// </summary>
        protected override void OnShow()
        {
            List<GroundedClause> givens = new List<GroundedClause>();
            //Populate list with applicable givens
            foreach (GroundedClause gc in currentGivens)
            {
                RightAngle ra = gc as RightAngle;
                if (ra != null)
                {
                    givens.Add(ra);
                }
            }

            List<Angle> rightAngles = new List<Angle>();
            //Populate list with possible choices
            foreach (Angle a in parser.backendParser.implied.angles)
            {
                if (a.measure == 90)
                {
                    RightAngle ra = new RightAngle(a);
                    if (!StructurallyContains(givens, ra))
                    {
                        rightAngles.Add(ra);
                    }
                }
            }

            options.ItemsSource = null; //Makes sure the box is graphically updated.
            options.ItemsSource = rightAngles;
        }

        protected override GroundedClause MakeClause()
        {
            if (options.SelectedValue == null)
            {
                return null;
            }
            else
            {
                return new RightAngle(options.SelectedValue as Angle);
            }
        }
    }
}
