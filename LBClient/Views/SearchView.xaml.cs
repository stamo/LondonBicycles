using LondonBicycles.Client.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LondonBicycles.Data.Models;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace LondonBicycles.Client.Views
{
    // TODO: Edit the manifest to enable searching
    //
    // The package manifest could not be automatically updated.  Open the package manifest
    // file and ensure that support for activation for searching is enabled.

    // TODO: Respond to activation for search results
    //
    // The following code could not be automatically added to your application subclass,
    // either because the appropriate class could not be located or because a method with
    // the same name already exists.  Ensure that appropriate code deals with activation
    // by displaying search results for the specified search term.
    //
    //         /// <summary>
    //         /// Invoked when the application is activated to display search results.
    //         /// </summary>
    //         /// <param name="args">Details about the activation request.</param>
    //         protected async override void OnSearchActivated(Windows.ApplicationModel.Activation.SearchActivatedEventArgs args)
    //         {
    //             // TODO: Register the Windows.ApplicationModel.Search.SearchPane.GetForCurrentView().QuerySubmitted
    //             // event in OnWindowCreated to speed up searches once the application is already running
    // 
    //             // If the Window isn't already using Frame navigation, insert our own Frame
    //             var previousContent = Window.Current.Content;
    //             var frame = previousContent as Frame;
    // 
    //             // If the app does not contain a top-level frame, it is possible that this 
    //             // is the initial launch of the app. Typically this method and OnLaunched 
    //             // in App.xaml.cs can call a common method.
    //             if (frame == null)
    //             {
    //                 // Create a Frame to act as the navigation context and associate it with
    //                 // a SuspensionManager key
    //                 frame = new Frame();
    //                 LondonBicycles.Client.Views.Common.SuspensionManager.RegisterFrame(frame, "AppFrame");
    // 
    //                 if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
    //                 {
    //                     // Restore the saved session state only when appropriate
    //                      try
    //                     {
    //                         await LondonBicycles.Client.Views.Common.SuspensionManager.RestoreAsync();
    //                     }
    //                     catch (LondonBicycles.Client.Views.Common.SuspensionManagerException)
    //                     {
    //                         //Something went wrong restoring state.
    //                         //Assume there is no state and continue
    //                     }
    //                 }
    //             }
    // 
    //             frame.Navigate(typeof(SearchView), args.QueryText);
    //             Window.Current.Content = frame;
    // 
    //             // Ensure the current window is active
    //             Window.Current.Activate();
    //         }
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchView : LondonBicycles.Client.Common.LayoutAwarePage
    {
        private SearchViewModel searchVm;

        public SearchView()
        {
            this.InitializeComponent();
            this.searchVm = new SearchViewModel();
            this.DataContext = this.searchVm;
        }

        public void StationClicked(object sender, ItemClickEventArgs e)
        {
            StationShort selectedStation = e.ClickedItem as StationShort;
            this.searchVm.NavigateToDetails(selectedStation);
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var queryText = navigationParameter as String;
            this.searchVm.QueryText = queryText;
            
        }
    }
}
