using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace WMP_UWP_TileGame
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            EnsurePageCreatedAndActivate(args);
        }

        // Modified From https://docs.microsoft.com/en-us/windows/uwp/launch-resume/activate-an-app
        /*  -- Method Header Comment
        Name	:	EnsurePageCreatedAndActivate
        Purpose :	Creates the MainPage if it isn't already created.  Also activates
                    the window so it takes foreground and input focus. Checks how the app was closed,
                    if by user or terminated, data will be resumed
        Inputs	:	LaunchActivatedEventArgs args
        Outputs	:	None
        Returns	:	None
        */
        private void EnsurePageCreatedAndActivate(LaunchActivatedEventArgs args)
        {
            // create the window if it doesn't exist
            if (Window.Current.Content == null)
            {
                Window.Current.Content = new MainPage();
            }
            // activate the window
            Window.Current.Activate();
            // if the app was closed by termination or by the user
            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated ||
    args.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                // resume state
                StateManagement.App_Resuming(Window.Current.Content as MainPage);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }
    }
}
