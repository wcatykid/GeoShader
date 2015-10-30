using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.EngineUIBridge;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// This window allows the user to edit problem characteristcs for the back-end.
    /// </summary>
    public class ProblemCharacteristicsWindow : ChildWindow
    {
        private ManageGivensWindow manageGivensWindow;
        private TextBox minWidth, maxWidth, minLength, maxLength;
        private Dictionary<Relationship, CheckBox> givenCheckboxes;
        private Dictionary<Relationship, RadioButton> goalCheckboxes;
        private ProblemCharacteristics problemCharacteristics;

        /// <summary>
        /// Create a new Problem Characteristics window.
        /// </summary>
        /// <param name="mgw">A ManageGivensWindow</param>
        public ProblemCharacteristicsWindow(ManageGivensWindow mgw)
        {
            Initialize();
            this.manageGivensWindow = mgw;
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            problemCharacteristics = ProblemCharacteristics.GetInstance();
            this.Title = "Desired Problem Characteristics";
            MakeCheckboxes();
            this.MaxHeight = 600;
            this.MaxWidth = 800;
            this.Closed += new EventHandler(UpdateOnClose);
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up the grid.
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Create the width characteritic label and inputs
            StackPanel widthPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            widthPanel.Margin = new Thickness(0, 0, 0, 10);
            TextBlock label = new TextBlock();
            label.Text = "Width:";
            label.Margin = new Thickness(0, 0, 10, 0);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Width = 45;
            widthPanel.Children.Add(label);
            minWidth = new NumericTextBox();
            minWidth.Width = 75;
            minWidth.Margin = new Thickness(0, 0, 10, 0);
            widthPanel.Children.Add(minWidth);
            maxWidth = new NumericTextBox();
            maxWidth.Width = 75;
            widthPanel.Children.Add(maxWidth);

            //Create the length characteritic label and inputs
            StackPanel lengthPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            lengthPanel.Margin = new Thickness(0, 0, 0, 10);
            label = new TextBlock();
            label.Text = "Length:";
            label.Margin = new Thickness(0, 0, 10, 0);
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Width = 45;
            lengthPanel.Children.Add(label);
            minLength = new NumericTextBox();
            minLength.Width = 75;
            minLength.Margin = new Thickness(0, 0, 10, 0);
            lengthPanel.Children.Add(minLength);
            maxLength = new NumericTextBox();
            maxLength.Width = 75;
            lengthPanel.Children.Add(maxLength);

            //Create a label and list of given relationships
            StackPanel givenPanel = new StackPanel();
            givenPanel.Margin = new Thickness(0, 0, 10, 0);
            label = new TextBlock();
            label.Text = "Given:";
            givenPanel.Children.Add(label);
            ListBox givenList = new ListBox();
            givenList.MinWidth = 200;
            givenList.MaxWidth = 200;
            givenList.MinHeight = 300;
            givenList.MaxHeight = 300;
            givenList.ItemsSource = givenCheckboxes.Values;
            givenPanel.Children.Add(givenList);

            //Create a label and list of goal relationships
            StackPanel goalPanel = new StackPanel();
            label = new TextBlock();
            label.Text = "Goal:";
            goalPanel.Children.Add(label);
            ListBox goalList = new ListBox();
            goalList.MinWidth = 200;
            goalList.MaxWidth = 200;
            goalList.MinHeight = 300;
            goalList.MaxHeight = 300;
            goalList.ItemsSource = goalCheckboxes.Values;
            goalPanel.Children.Add(goalList);

            //Create button to manage givens
            Button openGivensWindow = new Button();
            openGivensWindow.Content = "Manage Problem Givens";
            openGivensWindow.HorizontalAlignment = HorizontalAlignment.Center;
            openGivensWindow.Margin = new Thickness(0, 10, 0, 0);
            openGivensWindow.Click += new RoutedEventHandler(ManageGivens_Open);

            //Arrange items in the grid and add them to the grid.
            Grid.SetColumn(widthPanel, 0);
            Grid.SetRow(widthPanel, 0);
            Grid.SetColumnSpan(widthPanel, 2);
            grid.Children.Add(widthPanel);
            Grid.SetColumn(lengthPanel, 0);
            Grid.SetRow(lengthPanel, 1);
            Grid.SetColumnSpan(lengthPanel, 2);
            grid.Children.Add(lengthPanel);
            Grid.SetColumn(givenPanel, 0);
            Grid.SetRow(givenPanel, 2);
            grid.Children.Add(givenPanel);
            Grid.SetColumn(goalPanel, 1);
            Grid.SetRow(goalPanel, 2);
            grid.Children.Add(goalPanel);
            Grid.SetColumn(openGivensWindow, 0);
            Grid.SetRow(openGivensWindow, 3);
            Grid.SetColumnSpan(openGivensWindow, 2);
            grid.Children.Add(openGivensWindow);

            //Set the grid as the content of the window in order to display it.
            this.Content = grid;
        }

        /// <summary>
        /// Create the checkboxes associated with each relation.
        /// </summary>
        private void MakeCheckboxes()
        {
            givenCheckboxes = new Dictionary<Relationship, CheckBox>();
            goalCheckboxes = new Dictionary<Relationship, RadioButton>();

            //Make a given and goal checkbox for each relation
            foreach (Relationship r in problemCharacteristics.Relationships)
            {
                //Make given box
                CheckBox cb = new CheckBox();
                cb.Content = r.Name;
                cb.IsChecked = r.isGiven;
                cb.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { r.isGiven = true; });
                cb.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { r.isGiven = false; });
                givenCheckboxes.Add(r, cb);

                //Make goal box
                RadioButton rb = new RadioButton();
                rb.GroupName = "GoalButtons";
                rb.Content = r.Name;
                rb.IsChecked = r.isGoal;
                rb.Checked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { r.isGoal = true; });
                rb.Unchecked += new RoutedEventHandler((object sender, RoutedEventArgs e) => { r.isGoal = false; });
                goalCheckboxes.Add(r, rb);
            }
        }
        
        /// <summary>
        /// This event is fired when the problem characteristics window is closed.
        /// The method will update the problem characteristics to reflect the user input. The relationships from goal and given are not updated here,
        /// because their clicked and unclicked events already directly update the relationships.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateOnClose(object sender, EventArgs e)
        {
            problemCharacteristics.LowerWidth = minWidth.Text.Equals("") ? 0 : int.Parse(minWidth.Text);
            problemCharacteristics.UpperWidth = maxWidth.Text.Equals("") ? problemCharacteristics.LowerWidth : int.Parse(maxWidth.Text);
            problemCharacteristics.LowerLength = minLength.Text.Equals("") ? 0 : int.Parse(minLength.Text);
            problemCharacteristics.UpperLength = maxLength.Text.Equals("") ? problemCharacteristics.LowerLength : int.Parse(maxLength.Text);

            if (problemCharacteristics.UpperWidth < problemCharacteristics.LowerWidth)
            {
                problemCharacteristics.UpperWidth = problemCharacteristics.LowerWidth;
            }

            if (problemCharacteristics.UpperLength < problemCharacteristics.LowerLength)
            {
                problemCharacteristics.UpperLength = problemCharacteristics.LowerLength;
            }
        }

        /// <summary>
        /// Open the manage givens window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManageGivens_Open(object sender, RoutedEventArgs e)
        {
            ShowManageGivensWindow();
        }

        /// <summary>
        /// Open the manage givens window.
        /// This function is here just to make this functionality publically accessible.
        /// </summary>
        public void ShowManageGivensWindow()
        {
            manageGivensWindow.Show();
        }
    }
}
