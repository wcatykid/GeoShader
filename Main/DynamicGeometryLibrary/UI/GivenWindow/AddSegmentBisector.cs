using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddSegmentBisector : AddGivenWindow
    {
        private ComboBox options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddSegmentBisector() : base("Segment Bisector")
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
            desc.Text = "Specify a segment bisector.";
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
                GeometryTutorLib.ConcreteAST.SegmentBisector sb = gc as GeometryTutorLib.ConcreteAST.SegmentBisector;
                if (sb != null)
                {
                    givens.Add(sb);
                }
            }

            var bisectors = new List<GeometryTutorLib.ConcreteAST.SegmentBisector>();
            //Populate list with possible choices
            foreach (GeometryTutorLib.ConcreteAST.SegmentBisector sb in parser.backendParser.implied.segmentBisectors)
            {
                if (!StructurallyContains(givens, sb))
                {
                    bisectors.Add(sb);
                }
            }

            options.ItemsSource = null; //Makes sure the box is graphically updated.
            options.ItemsSource = bisectors;
        }

        protected override GroundedClause MakeClause()
        {
            if (options.SelectedValue == null)
            {
                return null;
            }
            else
            {
                return options.SelectedValue as GeometryTutorLib.ConcreteAST.SegmentBisector;
            }
        }
    }
}
