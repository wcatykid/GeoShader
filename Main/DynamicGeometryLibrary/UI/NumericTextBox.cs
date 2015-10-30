using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A Text box that only allows numbers as input.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        /// <summary>
        /// Create a new numeric text box
        /// </summary>
        public NumericTextBox()
            : base()
        {
            KeyDown += new KeyEventHandler(NumericTextBoxFilter);
        }

        /// <summary>
        /// This method will ignore all non-numeric and non-backspace keys when added as a KeyDown event to a TextBox.
        /// This is used to create integer-only text boxes for certain problem characteristics.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NumericTextBoxFilter(object sender, KeyEventArgs e)
        {
            if (!e.Handled && (e.Key < Key.D0 || e.Key > Key.D9) && (e.Key < Key.NumPad0 || e.Key > Key.NumPad9) && e.Key != Key.Back)
            {
                e.Handled = true;
            }
        }
    }
}
