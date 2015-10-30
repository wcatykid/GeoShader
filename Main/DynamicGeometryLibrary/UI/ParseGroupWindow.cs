using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GeometryTutorLib.EngineUIBridge;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to configure custom Assumption groups.
    /// </summary>
    public class ParseGroupWindow : ChildWindow
    {
        private ListBox GroupSelector, UnselectedAssumptions, SelectedAssumptions;
        private AddParseGroupWindow AddParseGroupWindow;

        /// <summary>
        /// Create the window.
        /// </summary>
        public ParseGroupWindow()
        {
            Initialize();
            AddParseGroupWindow = new AddParseGroupWindow();
            AddParseGroupWindow.Closed += new EventHandler(AddGroupWindow_Closed);
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Assumption Groups Configuration";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up outer grid
            Grid outerGrid = new Grid();
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            outerGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            outerGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Create the group selector and along with add and remove buttons
            StackPanel groupsPanel = new StackPanel();
            TextBlock groupSelectorLabel = new TextBlock();
            groupSelectorLabel.Text = "Select a Group:";
            groupsPanel.Children.Add(groupSelectorLabel);
            GroupSelector = new ListBox();
            GroupSelector.MaxWidth = 200;
            GroupSelector.MinWidth = 200;
            GroupSelector.MaxHeight = 500;
            GroupSelector.MinHeight = 500;
            GroupSelector.ItemsSource = GetUserGroups();
            GroupSelector.SelectionMode = SelectionMode.Single;
            GroupSelector.SelectionChanged += new SelectionChangedEventHandler(GroupSelectionChanged);
            groupsPanel.Children.Add(GroupSelector);
            //Create a the add and remove buttons in their own subpanel
            StackPanel addRemPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            addRemPanel.HorizontalAlignment = HorizontalAlignment.Center;
            addRemPanel.Margin = new Thickness(0, 5, 0, 0);
            Button addBtn = new Button();
            addBtn.Content = "Add";
            addBtn.Click += new RoutedEventHandler(AddGroupButton_Click);
            addBtn.Margin = new Thickness(0, 0, 5, 0);
            addBtn.Width = 75;
            addRemPanel.Children.Add(addBtn);
            Button remBtn = new Button();
            remBtn.Content = "Remove";
            remBtn.Click += new RoutedEventHandler(RemoveGroupButton_Click);
            remBtn.Width = 75;
            addRemPanel.Children.Add(remBtn);
            groupsPanel.Children.Add(addRemPanel);

            //Create vertical bar as a divider
            Canvas verticalBar = new Canvas();
            verticalBar.Background = new SolidColorBrush(Colors.Black);
            verticalBar.Width = 2;
            verticalBar.VerticalAlignment = VerticalAlignment.Stretch;
            verticalBar.Margin = new Thickness(10, 0, 10, 0);

            //Arange elements in the grid and add them to the grid.
            Grid.SetColumn(groupsPanel, 0);
            Grid.SetRow(groupsPanel, 0);
            outerGrid.Children.Add(groupsPanel);
            Grid.SetColumn(verticalBar, 1);
            Grid.SetRow(verticalBar, 0);
            outerGrid.Children.Add(verticalBar);
            Grid innerGrid = LayoutInnerGrid();
            Grid.SetColumn(innerGrid, 2);
            Grid.SetRow(innerGrid, 0);
            outerGrid.Children.Add(innerGrid);

            //Set the content of this window to be the newly designed layout.
            this.Content = outerGrid;
        }

        /// <summary>
        /// Layout the inner grid, which is the selected and unselected assumption panels and the transfer buttons
        /// </summary>
        /// <returns>The inner grid</returns>
        private Grid LayoutInnerGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            StackPanel unselectedAssumptionsPanel = new StackPanel();
            TextBlock usaLabel = new TextBlock();
            usaLabel.Text = "Available";
            usaLabel.HorizontalAlignment = HorizontalAlignment.Center;
            unselectedAssumptionsPanel.Children.Add(usaLabel);
            UnselectedAssumptions = new ListBox();
            UnselectedAssumptions.MaxWidth = 200;
            UnselectedAssumptions.MinWidth = 200;
            UnselectedAssumptions.MinHeight = 500;
            UnselectedAssumptions.MaxHeight = 500;
            UnselectedAssumptions.SelectionMode = SelectionMode.Multiple;
            unselectedAssumptionsPanel.Children.Add(UnselectedAssumptions);

            StackPanel addRemPanel = new StackPanel();
            addRemPanel.Margin = new Thickness(10, 0, 10, 0);
            addRemPanel.VerticalAlignment = VerticalAlignment.Center;
            Button addBtn = new Button();
            addBtn.Content = ">>>";
            addBtn.Click += new RoutedEventHandler(AddAssumptionButton_Click);
            addBtn.Margin = new Thickness(0, 0, 0, 10);
            addRemPanel.Children.Add(addBtn);
            Button remBtn = new Button();
            remBtn.Content = "<<<";
            remBtn.Click += new RoutedEventHandler(RemoveAssumptionButton_Click);
            addRemPanel.Children.Add(remBtn);

            StackPanel selectedAssumptionsPanel = new StackPanel();
            TextBlock ssaLabel = new TextBlock();
            ssaLabel.Text = "In Group";
            ssaLabel.HorizontalAlignment = HorizontalAlignment.Center;
            selectedAssumptionsPanel.Children.Add(ssaLabel);
            SelectedAssumptions = new ListBox();
            SelectedAssumptions.MaxWidth = 200;
            SelectedAssumptions.MinWidth = 200;
            SelectedAssumptions.MinHeight = 500;
            SelectedAssumptions.MaxHeight = 500;
            SelectedAssumptions.SelectionMode = SelectionMode.Multiple;
            selectedAssumptionsPanel.Children.Add(SelectedAssumptions);

            Grid.SetColumn(unselectedAssumptionsPanel, 0);
            Grid.SetRow(unselectedAssumptionsPanel, 0);
            grid.Children.Add(unselectedAssumptionsPanel);
            Grid.SetColumn(addRemPanel, 1);
            Grid.SetRow(addRemPanel, 0);
            grid.Children.Add(addRemPanel);
            Grid.SetColumn(selectedAssumptionsPanel, 2);
            Grid.SetRow(selectedAssumptionsPanel, 0);
            grid.Children.Add(selectedAssumptionsPanel);

            return grid;
        }

        /// <summary>
        /// Get all non-predefined parse groups.
        /// </summary>
        /// <returns>All non-predefined parse groups.</returns>
        public static List<ParseGroup> GetUserGroups()
        {
            List<ParseGroup> userGroups = new List<ParseGroup>();
            foreach (ParseGroup pg in ParseGroup.GetParseGroups())
            {
                if (!pg.Predefined)
                {
                    userGroups.Add(pg);
                }
            }
            return userGroups;
        }

        /// <summary>
        /// This event is executed whem the Group selection is changed.
        /// The method updates the inner grid with the relevant assumptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GroupSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ParseGroup selected = GroupSelector.SelectedValue as ParseGroup;
            if (selected == null)
                return;

            RefreshAssumptionLists(selected);
        }

        /// <summary>
        /// This event is executed when a Add group button is clicked.
        /// This methiod clears the AddParseGroupWindow field and displays the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGroupButton_Click(object sender, RoutedEventArgs e)
        {
            AddParseGroupWindow.GroupName = "";
            AddParseGroupWindow.Show();
        }
        
        /// <summary>
        /// This event is executed when the AddParseGroupWindow is closed.
        /// The method carries out the action of adding the new group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddGroupWindow_Closed(object sender, EventArgs e)
        {
            if (AddParseGroupWindow.GroupName.Equals(""))
                return;

            ParseGroup.AddParseGroup(AddParseGroupWindow.GroupName);
            GroupSelector.ItemsSource = null;
            GroupSelector.ItemsSource = GetUserGroups();

            ParseGroup selected = GroupSelector.SelectedValue as ParseGroup;
            if (selected == null)
                return;

            RefreshAssumptionLists(selected);
        }

        /// <summary>
        /// This event is executed when the Remove group button is clicked.
        /// The method will remove the currently selected group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveGroupButton_Click(object sender, RoutedEventArgs e)
        {
            ParseGroup selected = GroupSelector.SelectedValue as ParseGroup;
            if (selected == null)
                return;

            ParseGroup.RemoveGroup(selected);
            GroupSelector.ItemsSource = null;
            GroupSelector.ItemsSource = GetUserGroups();

            RefreshAssumptionLists(selected);
        }

        /// <summary>
        /// This event is executed when the add assumption buttin is clicked.
        /// The method will move all selected items in the deselected pane to the selected pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddAssumptionButton_Click(object sender, RoutedEventArgs e)
        {
            ParseGroup selectedGroup = GroupSelector.SelectedValue as ParseGroup;
            if (selectedGroup == null)
                return;

            foreach (object value in UnselectedAssumptions.SelectedItems)
            {
                Assumption a = value as Assumption;
                selectedGroup.Assumptions.Add(a);
            }

            RefreshAssumptionLists(selectedGroup);
        }

        /// <summary>
        /// This event is executed when the remove assumption buttin is clicked.
        /// The method will move all selected items in the selected pane to the deselected pane.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveAssumptionButton_Click(object sender, RoutedEventArgs e)
        {
            ParseGroup selectedGroup = GroupSelector.SelectedValue as ParseGroup;
            if (selectedGroup == null)
                return;

            foreach (object value in SelectedAssumptions.SelectedItems)
            {
                Assumption a = value as Assumption;
                selectedGroup.Assumptions.Remove(a);
            }

            RefreshAssumptionLists(selectedGroup);
        }

        /// <summary>
        /// This method refreshes the selected and deselected assumption panes.
        /// </summary>
        /// <param name="selected">The current ParseGroup that is selected in the groups pane.</param>
        private void RefreshAssumptionLists(ParseGroup selected)
        {
            //Update selected pane
            SelectedAssumptions.ItemsSource = null;
            SelectedAssumptions.ItemsSource = selected.Assumptions;

            //Get unselected assumptions
            List<Assumption> unselectedAssumptions = new List<Assumption>();
            foreach (Assumption a in Assumption.GetAssumptions().Values)
            {
                if (!selected.Assumptions.Contains(a))
                {
                    unselectedAssumptions.Add(a);
                }
            }

            //Update unselected pane
            UnselectedAssumptions.ItemsSource = null;
            UnselectedAssumptions.ItemsSource = unselectedAssumptions;
        }
    }
}
