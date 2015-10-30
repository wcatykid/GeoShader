using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using GeometryTutorLib;

namespace DynamicGeometry.UI
{
    /// <summary>
    /// A ListBox that has toggled visibility and that provides utilities for asyncrhonously publishing strings to itself from threads other than the UI thread.
    /// The use of this window is for the AI backend to publish strings to the UI.
    /// </summary>
    public class AIDebugWindow : ListBox
    {
        private static List<string> ListItems = new List<string>();

        /// <summary>
        /// Create a new AIDebugWindow.
        /// </summary>
        public AIDebugWindow()
        {
            this.SelectionMode = SelectionMode.Single;
            this.ItemsSource = ListItems;
        }

        /// <summary>
        /// Toggle Window visibility.
        /// </summary>
        public bool Visible
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
            set
            {
                this.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                Settings.Instance.ShowAIDebugWindow = value;
            }
        }
        
        /// <summary>
        /// Create a new UIDebugPublisher. This object can publish strings to the window or clear the window.
        /// The publish and clear operations can be called from any thread but will be asyncrhonously invoked on the UI thread.
        /// </summary
        public void MakeUIDebugPublisher()
        {
            UIDebugPublisher.create(produceString, clearWindow);
        }

        /// <summary>
        /// Add a string to the window. The actual update of the window will asynchronously execute on the UI thread.
        /// </summary>
        /// <param name="str">The string to add</param>
        public void produceString(string str)
        {
            Action produceAction = delegate() { ListItems.Add(str); bindData(); };
            SmartDispatcher.BeginInvoke(produceAction);
        }

        /// <summary>
        /// Clear all strings from the window. The actual update of the window will asynchronously execute on the UI thread.
        /// </summary>
        public void clearWindow()
        {
            Action clearAction = delegate() { ListItems.Clear(); };
            SmartDispatcher.BeginInvoke(clearAction);
        }

        /// <summary>
        /// This method forces the ListBox to update itself and repaint the strings.
        /// If this method is not called the ListBox does not visually update until another UI redraw is performed.
        /// </summary>
        private void bindData()
        {
            this.ItemsSource = null;
            this.ItemsSource = ListItems;
        }
    }
}
