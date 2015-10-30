using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.ConcreteAST;
using LiveGeometry;
using LiveGeometry.TutorParser;

namespace DynamicGeometry.UI.GivenWindow
{
    public abstract class AddGivenWindow : ChildWindow
    {
        protected string givenName;
        protected List<GroundedClause> currentGivens;
        protected DrawingParserMain parser;

        public Result WindowResult { get; private set; }
        public GroundedClause Clause { get; private set; }

        public enum Result { Accept, Cancel };

        /// <summary>
        /// Create the window.
        /// </summary>
        /// <param name="givenName">The name of the given being added.</param>
        protected AddGivenWindow(string givenName)
        {
            this.givenName = givenName;
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Add Given: " + givenName;
            this.HasCloseButton = false;
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Should layout the design for the particular given.
        /// </summary>
        /// <returns>A Grid with UI components that create the given.</returns>
        protected abstract Grid MakeGivenGrid();

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

            //Get the grid created by the subclass for the specific given.
            Grid innerGrid = MakeGivenGrid();

            //Create the Accept and Cancel buttons.
            StackPanel acceptCancelPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            acceptCancelPanel.HorizontalAlignment = HorizontalAlignment.Right;
            acceptCancelPanel.Margin = new Thickness(0, 10, 0, 0);
            Button acceptBtn = new Button();
            acceptBtn.Content = "Accept";
            acceptBtn.MinWidth = 50;
            acceptBtn.Margin = new Thickness(0, 0, 5, 0);
            acceptBtn.Click += new RoutedEventHandler(AcceptBtn_Click);
            acceptCancelPanel.Children.Add(acceptBtn);
            Button cancelBtn = new Button();
            cancelBtn.Content = "Cancel";
            cancelBtn.MinWidth = 50;
            cancelBtn.Click += new RoutedEventHandler(CancelBtn_Click);
            acceptCancelPanel.Children.Add(cancelBtn);

            //Arrange items in the grid and add them to the grid.
            Grid.SetColumn(innerGrid, 0);
            Grid.SetRow(innerGrid, 0);
            grid.Children.Add(innerGrid);
            Grid.SetColumn(acceptCancelPanel, 0);
            Grid.SetRow(acceptCancelPanel, 1);
            grid.Children.Add(acceptCancelPanel);

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Give the window critical data and then display the window.
        /// Call this method instead of the inherited Show().
        /// </summary>
        /// <param name="parser">A Drawing Parser initialized with the current drawing. (This method will call Parse)</param>
        /// <param name="currentGivens">A list of all the current givens.</param>
        public void Show(DrawingParserMain parser, List<GroundedClause> currentGivens)
        {
            this.parser = parser;
            this.currentGivens = currentGivens;
            parser.Parse();
            OnShow();
            Show();
        }

        /// <summary>
        /// This method will be called before the window is shown.
        /// Subclasses should override this method to include calculations that should happen
        /// each time the window is displayed.
        /// </summary>
        protected abstract void OnShow();

        /// <summary>
        /// This method will be called when the accept button is clicked.
        /// Should create the GroundedClause representing the new given.
        /// </summary>
        /// <returns></returns>
        protected abstract GroundedClause MakeClause();

        /// <summary>
        /// This event is called when the Accept button is clicked.
        /// Will set the window result to Accept and update the Clause, then close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowResult = Result.Accept;
            Clause = MakeClause();
            Close();
        }

        /// <summary>
        /// This event is called when the Cancel button is clicked.
        /// Will set the window result to Cancel and set the Clause to null, then close the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowResult = Result.Cancel;
            Clause = null;
            Close();
        }

        /// <summary>
        /// Iterate through a list of GroundedClauses and see if the supplied clause is
        /// struturally equivilant to an item in the list.
        /// </summary>
        /// <param name="list">The list to iterate through</param>
        /// <param name="toMatch">The GroundedClause being matched</param>
        /// <returns>true if the list contains a structurally equal clause, false otherwise</returns>
        protected static bool StructurallyContains(List<GroundedClause> list, GroundedClause toMatch)
        {
            bool contains = false;
            foreach (GroundedClause gc in list)
            {
                if (toMatch.StructurallyEquals(gc))
                {
                    contains = true;
                    break;
                }
            }
            return contains;
        }
    }
}
