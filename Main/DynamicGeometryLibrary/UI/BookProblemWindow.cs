using GeometryTutorLib.GeometryTestbed;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    public class BookProblemWindow : ChildWindow
    {
        private Dictionary<string, ActualProblem> problems;
        private ComboBox problemsBox;

        /// <summary>
        /// Create the window.
        /// </summary>
        public BookProblemWindow()
        {
            Initialize();
            GetProblems();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Draw Book Problem";
            this.MaxHeight = 120;
            this.MaxWidth = 200;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up the grid.
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Drop down menu
            problemsBox = new ComboBox();
            problemsBox.Width = 200;
            problemsBox.Margin = new Thickness(0, 0, 0, 10);
            problemsBox.ItemsSource = problems.Keys;

            //Add Button
            Button addBtn = new Button();
            addBtn.Content = "Add";
            addBtn.HorizontalAlignment = HorizontalAlignment.Right;
            addBtn.Click += new RoutedEventHandler(AddBtn_Click);

            //Arrange the UI elements in the grid.
            Grid.SetColumn(problemsBox, 0);
            Grid.SetRow(problemsBox, 0);
            grid.Children.Add(problemsBox);
            Grid.SetColumn(addBtn, 0);
            Grid.SetRow(addBtn, 1);
            grid.Children.Add(addBtn);

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Retrieve The problems for the window.
        /// </summary>
        private void GetProblems()
        {
            List<ActualProblem> problemList = ShadedAreaProblems.GetProblems();

            problems = new Dictionary<string, ActualProblem>();
            foreach (var problem in problemList)
            {
                problems.Add(problem.problemName, problem);
            }
        }

        /// <summary>
        /// This method is executed with the Add button is clicked.
        /// Will draw the problem and run it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            UIProblemDrawer drawer = UIProblemDrawer.getInstance();
            ActualProblem problem = problems[problemsBox.SelectedValue as string];

            //Create the problem description from the actual problem
            UIProblemDrawer.ProblemDescription desc = new UIProblemDrawer.ProblemDescription();
            desc.Points = problem.parser.implied.allFigurePoints;
            desc.Segments = problem.segments;
            desc.Circles = problem.circles;
            //Decide if there is a region to shade
            ActualShadedAreaProblem saProb = problem as ActualShadedAreaProblem;
            if (saProb != null) desc.Regions = saProb.goalRegions;
            else desc.Regions = null;

            //Draw the problem
            drawer.reset();
            drawer.clear();
            drawer.draw(desc);

            //Run the problem
            problem.Run();

            Close();
        }
    }
}
