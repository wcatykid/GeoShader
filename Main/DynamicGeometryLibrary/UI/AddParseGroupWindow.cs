using System.Windows;
using System.Windows.Controls;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A window that accepts the name for a new parse group.
    /// </summary>
    public class AddParseGroupWindow : ChildWindow
    {
        private TextBox NameInput;

        /// <summary>
        /// The name of the new parse group.
        /// </summary>
        public string GroupName
        {
            get
            {
                return NameInput.Text;
            }
            set
            {
                NameInput.Text = value;
            }
        }

        /// <summary>
        /// Create the window.
        /// </summary>
        public AddParseGroupWindow()
        {
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = "Add Assumption Group";
            this.MaxHeight = 600;
            this.MaxWidth = 800;
            this.HasCloseButton = false;
        }

        /// <summary>
        /// Layout the content of the window.
        /// </summary>
        private void LayoutDesign()
        {
            StackPanel panel = new StackPanel();

            //Add the text label
            TextBlock label = new TextBlock();
            label.Text = "Name the new group:";
            panel.Children.Add(label);

            //Add the text box
            NameInput = new TextBox();
            NameInput.MaxWidth = 200;
            NameInput.MinWidth = 200;
            NameInput.Margin = new Thickness(0, 0, 0, 10);
            panel.Children.Add(NameInput);

            //Add the button
            Button okBtn = new Button();
            okBtn.Content = "Add";
            okBtn.HorizontalAlignment = HorizontalAlignment.Center;
            okBtn.Width = 75;
            okBtn.Click += new RoutedEventHandler(OKButton_Click);
            panel.Children.Add(okBtn);

            //Set the newly designed layout as the content of the window.
            this.Content = panel;
        }

        /// <summary>
        /// This event is executed when the OK button is clicked.
        /// The method closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
