using System;

namespace GeometryTutorLib
{
    /// <summary>
    /// This class is used to asyncrhonously publish strings to the UI.
    /// The strings are displayed to the UI in the AI Debug Window, and the actual action of publishing or clearing
    /// will happen on the UI thread.
    /// 
    /// Follows a variation on the Singleton pattern.
    /// </summary>
    public class UIDebugPublisher
    {
        private static UIDebugPublisher instance = null;

        private Action<String> publishStringAction;
        private Action clearWindowAction;

        /// <summary>
        /// Create a new UIDebugPublisher with publish and clear actions.
        /// </summary>
        /// <param name="publishStringAction">An Action that publishes the given string to the UI. This Action should invoke the actual update operation on the UI thread.</param>
        /// <param name="clearWindowAction">An Action that clears all debug strings from the UI. This Action should invoke the actual update operation on the UI thread.</param>
        private UIDebugPublisher(Action<String> publishStringAction, Action clearWindowAction) 
        {
            this.publishStringAction = publishStringAction;
            this.clearWindowAction = clearWindowAction;
        }

        /// <summary>
        /// Create the publisher instance with the given actions.
        /// This method should only be called once, and before getInstance().
        /// </summary>
        /// <param name="publishStringAction">An Action that publishes the given string to the UI. This Action should invoke the actual update operation on the UI thread.</param>
        /// <param name="clearWindowAction">An Action that clears all debug strings from the UI. This Action should invoke the actual update operation on the UI thread.</param>
        public static void create(Action<String> publishStringAction, Action clearWindowAction)
        {
            System.Diagnostics.Debug.Assert(instance == null); //create should only be called once.
            instance = new UIDebugPublisher(publishStringAction, clearWindowAction);
        }

        /// <summary>
        /// This method returns the singleton instance.
        /// Should be called only after create.
        /// </summary>
        /// <returns>An instance of UIDebugPublisher.</returns>
        public static UIDebugPublisher getInstance()
        {
            System.Diagnostics.Debug.Assert(instance != null); //create should have been called before this.
            return instance;
        }

        /// <summary>
        /// Publish a string to the AI Debug Window.
        /// </summary>
        /// <param name="str">The string to publish</param>
        public void publishString(string str)
        {
            publishStringAction(str);
        }

        /// <summary>
        /// Publish a debug string to the AI Debug Window.
        /// Currently just prepends "DEBUG: ", but may later make perform a different action.
        /// </summary>
        /// <param name="str">The string to publish</param>
        public void publishDebug(string str)
        {
            publishString("DEBUG: " + str);
        }

        /// <summary>
        /// Clear the AI Debug Window of all strings.
        /// </summary>
        public void clearWindow()
        {
            clearWindowAction();
        }
    }
}
