using System.Windows;
using System.Windows.Controls;

namespace DynamicGeometry.UI
{
    public class ParseDisambiguationWindow : ChildWindow
    {
        private string message, title;
        public Result PDWDialogResult { get; private set; }

        public enum Result { Yes = 0, No };

        /// <summary>
        /// Create a new ParseDisambiguationWindow with the given title and message
        /// </summary>
        /// <param name="message">The message the window will display</param>
        /// <param name="title">The title of the window</param>
        public ParseDisambiguationWindow(string message, string title)
        {
            this.message = message;
            this.title = title;
            PDWDialogResult = Result.No;
            Initialize();
            LayoutDesign();
        }

        /// <summary>
        /// Set this window's basic properties.
        /// </summary>
        private void Initialize()
        {
            this.Title = title;
            this.MaxHeight = 600;
            this.MaxWidth = 800;
            this.HasCloseButton = false;
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

            //Create message text
            TextBlock messageText = new TextBlock();
            messageText.Text = message;
            messageText.MaxWidth = 300;
            messageText.MinWidth = 300;
            messageText.TextWrapping = TextWrapping.Wrap;
            messageText.Margin = new Thickness(0, 0, 0, 10);

            //Create option buttons
            StackPanel buttons = new StackPanel() { Orientation = Orientation.Horizontal };
            buttons.HorizontalAlignment = HorizontalAlignment.Right;
            Button yesBtn = new Button();
            yesBtn.Content = "Yes";
            yesBtn.Click += new RoutedEventHandler(YesButton_Click);
            yesBtn.Width = 50;
            yesBtn.Margin = new Thickness(0, 0, 10, 0);
            buttons.Children.Add(yesBtn);
            Button noBtn = new Button();
            noBtn.Content = "No";
            noBtn.Click += new RoutedEventHandler(NoButton_Click);
            noBtn.Width = 50;
            buttons.Children.Add(noBtn);

            //Arrange elements in grid and add then to the grid.
            Grid.SetColumn(messageText, 0);
            Grid.SetRow(messageText, 0);
            grid.Children.Add(messageText);
            Grid.SetColumn(buttons, 0);
            Grid.SetRow(buttons, 1);
            grid.Children.Add(buttons);

            //Set the content of this window to be the newly designed layout.
            this.Content = grid;
        }

        /// <summary>
        /// This event is executed when the Yes button is clicked.
        /// The method sets the result to Yes and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            PDWDialogResult = Result.Yes;
            Close();
        }

        /// <summary>
        /// This event is executed when the No button is clicked.
        /// The method sets the result to No and closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            PDWDialogResult = Result.No;
            Close();
        }
    }
}
