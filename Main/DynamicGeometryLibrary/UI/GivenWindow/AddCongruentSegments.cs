using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;

namespace DynamicGeometry.UI.GivenWindow
{
    public class AddCongruentSegments : AddGivenWindow
    {
        private ComboBox segment1, segment2;
        private Dictionary<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Segment>> options;

        /// <summary>
        /// Create the new window by calling the base constructor.
        /// </summary>
        public AddCongruentSegments() : base("Congruent Segments")
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
            desc.Text = "Specify two segments as being congruent.";
            desc.Margin = new Thickness(0, 0, 0, 10);

            //Create the first combo box
            StackPanel panel1 = new StackPanel();
            panel1.Margin = new Thickness(0, 0, 10, 0);
            TextBlock label1 = new TextBlock();
            label1.Text = "Segment 1:";
            panel1.Children.Add(label1);
            segment1 = new ComboBox();
            segment1.MinWidth = 150;
            segment1.SelectionChanged += new SelectionChangedEventHandler(Segment1_SelectionChanged);
            panel1.Children.Add(segment1);

            //Create the second combo box
            StackPanel panel2 = new StackPanel();
            TextBlock label2 = new TextBlock();
            label2.Text = "Segment 2:";
            panel2.Children.Add(label2);
            segment2 = new ComboBox();
            segment2.MinWidth = 150;
            panel2.Children.Add(segment2);

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
        /// Figure out what possible segment combinations are before showing the window.
        /// </summary>
        protected override void OnShow()
        {
            options = new Dictionary<GeometryTutorLib.ConcreteAST.Segment, List<GeometryTutorLib.ConcreteAST.Segment>>();

            //Get a list of all congruent segment givens
            List<GroundedClause> csegs = new List<GroundedClause>();
            foreach (GroundedClause gc in currentGivens)
            {
                CongruentSegments cseg = gc as CongruentSegments;
                if (cseg != null)
                {
                    csegs.Add(cseg);
                }
            }

            //Pick a first segment...
            foreach (GeometryTutorLib.ConcreteAST.Segment s1 in parser.backendParser.implied.segments)
            {
                List<GeometryTutorLib.ConcreteAST.Segment> possible = new List<GeometryTutorLib.ConcreteAST.Segment>();

                //... and see what other segments are viable second options.
                foreach (GeometryTutorLib.ConcreteAST.Segment s2 in parser.backendParser.implied.segments)
                {
                    if (s1.Length == s2.Length)
                    {
                        CongruentSegments cseg = new CongruentSegments(s1, s2);

                        if (!s1.StructurallyEquals(s2) && !StructurallyContains(csegs, cseg))
                        {
                            possible.Add(s2);
                        }
                    }
                }

                //If we found a possible list of combinations, add it to the dictionary
                if (possible.Count > 0)
                {
                    options.Add(s1, possible);
                }
            }

            //Set the options of the segment1 combo box
            segment1.ItemsSource = null; //Graphical refresh
            segment1.ItemsSource = options.Keys;
        }

        /// <summary>
        /// This event is called when the segment1 combo box changes its selction.
        /// The method will update the segment2 combo box to reflect viable combinations with segment1.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Segment1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            segment2.ItemsSource = null;
            GeometryTutorLib.ConcreteAST.Segment selection = segment1.SelectedValue as GeometryTutorLib.ConcreteAST.Segment;
            if (selection != null)
            {
                segment2.ItemsSource = options[selection];
            }
        }

        protected override GroundedClause MakeClause()
        {
            GeometryTutorLib.ConcreteAST.Segment s1 = segment1.SelectedValue as GeometryTutorLib.ConcreteAST.Segment;
            GeometryTutorLib.ConcreteAST.Segment s2 = segment2.SelectedValue as GeometryTutorLib.ConcreteAST.Segment;
            if (s1 != null && s2 != null)
            {
                return new GeometricCongruentSegments(s1, s2);
            }
            else
            {
                return null;
            }
        }
    }
}
