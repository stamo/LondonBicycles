using Callisto.Controls;
using LondonBicycles.Client.Helpers;
using LondonBicycles.Client.Views;
using LondonBicycles.Data;
using System;
using System.Text;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace LondonBicycles.Client
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private string unhandledExceptionMessage;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
            this.unhandledExceptionMessage = null;
            
        }

        void App_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            throw new NotImplementedException();
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Message: {0}\n", e.Message);
            sb.AppendFormat("StackTrace: {0}\n", e.Exception.StackTrace);
            this.unhandledExceptionMessage = sb.ToString();
            Notification.UserChoice("Something went wrong! Please, help us improve this application, by sending error report",
                "Send Report",
                "Cancel",
                new UICommandInvokedHandler(this.CommandInvokedHandler));
        }

        public async void CommandInvokedHandler(IUICommand command)
        {
            if (this.unhandledExceptionMessage != null)
            {
                var result = await DataPersister.SendReport(this.unhandledExceptionMessage);
                this.unhandledExceptionMessage = null;
            }
            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                //Initialisation on normal start

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(HomePage), args.Arguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
            // Ensure the current window is active
            Window.Current.Activate();
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
        }

        protected async override void OnSearchActivated(SearchActivatedEventArgs args)
        {
             
            // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
            // event in OnWindowCreated to speed up searches once the application is already running

            // If the Window isn't already using Frame navigation, insert our own Frame
            var previousContent = Window.Current.Content;
            var frame = previousContent as Frame;

            // If the app does not contain a top-level frame, it is possible that this 
            // is the initial launch of the app. Typically this method and OnLaunched 
            // in App.xaml.cs can call a common method.
            if (frame == null)
            {
                // Create a Frame to act as the navigation context and associate it with
                // a SuspensionManager key
                frame = new Frame();
                LondonBicycles.Client.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");

                if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    // Restore the saved session state only when appropriate
                    try
                    {
                        await LondonBicycles.Client.Common.SuspensionManager.RestoreAsync();
                    }
                    catch (LondonBicycles.Client.Common.SuspensionManagerException)
                    {
                        //Something went wrong restoring state.
                        //Assume there is no state and continue
                    }
                }
            }

            frame.Navigate(typeof(Views.SearchView), args.QueryText);
            Window.Current.Content = frame;

            // Ensure the current window is active
            Window.Current.Activate();
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
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }

        private void InitializeData()
        {
 
        }

        private void GetSettings()
        {
 
        }

        public void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var privacyPolicy = new SettingsCommand("privacy policy", "Privacy policy", (handler) =>
            {
                Color background = Color.FromArgb(255, 182, 227, 246);
                var settings = new SettingsFlyout();
                settings.Content = new PrivacyStatement();
                settings.HeaderBrush = new SolidColorBrush(background);
                settings.Background = new SolidColorBrush(background);
                settings.HeaderText = "Privacy policy";
                settings.IsOpen = true;
            });

            args.Request.ApplicationCommands.Add(privacyPolicy);
        }
    }
}
