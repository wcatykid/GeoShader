using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using GeometryTutorLib.EngineUIBridge;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that allows the user to configure which axioms, defintions, and theorems the back end is permitted to use.
    /// </summary>
    public class ParseOptionsWindow : ChildWindow
    {
        private ParseGroupWindow parseGroupWindow;
        private ListBox visibibleAssumptions;
        private Dictionary<Assumption, CheckBox> assumptionCheckboxes;
        private ComboBox groupCombo;

        /// <summary>
        /// Create the window.
        /// </summary>
        public ParseOptionsWindow()
        {
            Initialize();
            MakeAssumptionCheckBoxes();
            parseGroupWindow = new ParseGroupWindow();
            parseGroupWindow.Closed += new EventHandler(ParseGroupWindow_Closed);
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Parse Options Configuration";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            //Set up grid
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            //Create group selection box and label
            StackPanel comboLabelStack = new StackPanel();
            TextBlock comboLabel = new TextBlock();
            comboLabel.Text = "Select Group:";
            comboLabelStack.Children.Add(comboLabel);
            StackPanel comboStack = new StackPanel() { Orientation = Orientation.Horizontal };
            Button editGroupButton = new Button();
            editGroupButton.Content = "Edit";
            editGroupButton.Margin = new Thickness(0, 0, 5, 0);
            editGroupButton.Click += new RoutedEventHandler(EditGroupButton_Click);
            comboStack.Children.Add(editGroupButton);
            groupCombo = new ComboBox();
            groupCombo.ItemsSource = ParseGroup.GetParseGroups();
            groupCombo.SelectionChanged += new SelectionChangedEventHandler(AssumptionGroupChanged);
            comboStack.Children.Add(groupCombo);
            comboStack.MaxWidth = 400;
            comboLabelStack.Children.Add(comboStack);
            comboLabelStack.Margin = new Thickness(0, 0, 0, 10);

            //Create Assumption checkbox list
            visibibleAssumptions = new ListBox();
            visibibleAssumptions.MaxWidth = 400;
            visibibleAssumptions.MaxHeight = 800;
            visibibleAssumptions.MinWidth = 400;
            visibibleAssumptions.MinHeight = 200;
            visibibleAssumptions.Margin = new Thickness(0, 0, 0, 10);
            groupCombo.SelectedValue = ParseGroup.GetParseGroups().ToArray()[0];

            //Create Select and Deselect All buttons
            StackPanel selectDeselectPanel = new StackPanel() { Orientation = Orientation.Horizontal };
            selectDeselectPanel.HorizontalAlignment = HorizontalAlignment.Center;
            Button selectBtn = new Button();
            selectBtn.Content = "Select All";
            selectBtn.Width = 75;
            selectBtn.Click += new RoutedEventHandler(SelectAllButton_Click);
            selectBtn.Margin = new Thickness(0, 0, 10, 0);
            selectDeselectPanel.Children.Add(selectBtn);
            Button deselectBtn = new Button();
            deselectBtn.Content = "Deselect All";
            deselectBtn.Width = 75;
            deselectBtn.Click += new RoutedEventHandler(DeselectAllButton_Click);
            selectDeselectPanel.Children.Add(deselectBtn);

            //Set element locations in grid and add to grid
            Grid.SetColumn(visibibleAssumptions, 0);
            Grid.SetRow(visibibleAssumptions, 1);
            grid.Children.Add(visibibleAssumptions);
            Grid.SetColumn(comboLabelStack, 0);
            Grid.SetRow(comboLabelStack, 0);
            grid.Children.Add(comboLabelStack);
            Grid.SetColumn(selectDeselectPanel, 0);
            Grid.SetRow(selectDeselectPanel, 2);
            grid.Children.Add(selectDeselectPanel);

            //Set the content of the window to be the newly designed layout
            this.Content = grid;
        }

        /// <summary>
        /// Make a check box for each assumption. These checkboxes will automatically update the "Enabled" field of their corresponding assumption.
        /// </summary>
        private void MakeAssumptionCheckBoxes()
        {
            assumptionCheckboxes = new Dictionary<Assumption, CheckBox>();
            foreach (Assumption assumption in Assumption.GetAssumptions().Values)
            {
                CheckBox cb = new CheckBox();
                cb.IsChecked = assumption.Enabled;
                cb.Checked += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e) { assumption.Enabled = true; });
                cb.Unchecked += new RoutedEventHandler(delegate(object sender, RoutedEventArgs e) { assumption.Enabled = false; });
                cb.Content = assumption.ToString();
                assumptionCheckboxes.Add(assumption, cb);
            }
        }

        /// <summary>
        /// This executes a selection changed event when the group selection combo box has anew item selected.
        /// The method will update the visibile assumptions to match the selected assumption group.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AssumptionGroupChanged(object sender, SelectionChangedEventArgs e)
        {
            ParseGroup selected = groupCombo.SelectedItem as ParseGroup;
            if (selected == null)
                return;
            List<CheckBox> checkBoxes = new List<CheckBox>();
            //Add the check box foreach assumption in the group to the list
            foreach (Assumption a in selected.Assumptions)
            {
                CheckBox value;
                if (assumptionCheckboxes.TryGetValue(a, out value))
                {
                    checkBoxes.Add(value);
                }
            }
            visibibleAssumptions.ItemsSource = null;
            visibibleAssumptions.ItemsSource = checkBoxes;
        }

        /// <summary>
        /// This executes the click event when the Edit button is clicked.
        /// The method opens a new window where Assumption groups can be edited.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EditGroupButton_Click(object sender, RoutedEventArgs e)
        {
            parseGroupWindow.Show();
        }

        /// <summary>
        /// This executes the click event when the Select All Button is clicked.
        /// All Assumptions in the current group will be checked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in visibibleAssumptions.Items)
            {
                cb.IsChecked = true;
            }
        }

        /// <summary>
        /// This executes the click event when the Deelect All Button is clicked.
        /// All Assumptions in the current group will be unchecked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeselectAllButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox cb in visibibleAssumptions.Items)
            {
                cb.IsChecked = false;
            }
        }

        /// <summary>
        /// This executes the closing event when the Edit Assumptions window is closed.
        /// The method refreshes the groups list and rebinds the visible assumptions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ParseGroupWindow_Closed(object sender, EventArgs e)
        {
            //Execute assumption window closing event
            groupCombo.ItemsSource = null;
            groupCombo.ItemsSource = ParseGroup.GetParseGroups();
            groupCombo.SelectedValue = ParseGroup.GetParseGroups().ToArray()[0];
        }
    }
}
