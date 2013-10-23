using LondonBicycles.Client.Behavior;
using LondonBicycles.Client.Common;
using LondonBicycles.Client.Helpers;
using LondonBicycles.Client.Settings;
using LondonBicycles.Client.Views;
using LondonBicycles.Data;
using LondonBicycles.Data.LocationProvider;
using LondonBicycles.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LondonBicycles.Client.ViewModels
{
    public class HomeViewModel : BindableBase
    {
        private ObservableCollection<StationFull> nearestStations;
        private Point location;
        private bool inProgress;
        private UserSettings settings;
        private ICommand refreshCommand;
        private ICommand showNearestCommand;
        private ICommand showAllCommand;
        private ICommand showOnMapCommand;
        private string branding;

        public bool InProgress
        {
            get 
            { 
                return inProgress; 
            }

            set 
            {
                inProgress = value;
                OnPropertyChanged("InProgress");
            }
        }

        public string Branding
        {
            get 
            { 
                return this.branding; 
            }

            set 
            {
                this.branding = value;
                OnPropertyChanged("Branding");
            }
        }

        public ICommand Refresh
        {
            get
            {
                if (this.refreshCommand == null)
                {
                    this.refreshCommand = new DelegateCommand<HomeViewModel>(this.HandleRefreshCommand);
                }
                return this.refreshCommand;
            }
        }

        public ICommand ShowNearest
        {
            get
            {
                if (this.showNearestCommand == null)
                {
                    this.showNearestCommand = new DelegateCommand<string>(this.HandleShowNearestCommand);
                }
                return this.showNearestCommand;
            }
        }

        public ICommand ShowAll
        {
            get
            {
                if (this.showAllCommand == null)
                {
                    this.showAllCommand = new DelegateCommand<HomeViewModel>(this.HandleShowAllCommand);
                }
                return this.showAllCommand;
            }
        }

        public ICommand ShowOnMap
        {
            get
            {
                if (this.showOnMapCommand == null)
                {
                    this.showOnMapCommand = new DelegateCommand<string>(this.HandleShowOnMapCommand);
                }
                return this.showOnMapCommand;
            }
        }

        public IEnumerable<StationFull> NearestStations
        {
            get
            {
                if (this.nearestStations == null)
                {
                    this.nearestStations = new ObservableCollection<StationFull>();
                }

                return this.nearestStations;
            }
            set
            {
                if (this.nearestStations == null)
                {
                    this.nearestStations = new ObservableCollection<StationFull>();
                }

                this.SetObservableValues(this.nearestStations, value);
            }

        }

        public HomeViewModel()
        {
            this.GetData();
        }

        private async void GetData()
        {
            if (this.nearestStations != null)
            {
                this.nearestStations.Clear();
            }

            this.InProgress = true;
            Location locator = new Location();
            this.settings = new UserSettings();
            StationFullModel stationsData = new StationFullModel();

            try
            {
                this.location = await locator.CurrentPosition;
                stationsData = await DataPersister.GetNearestStations(this.location, this.settings.MaxNearStations);
                this.NearestStations = stationsData.Stations;
                if (stationsData.Timespan != 0)
                {
                    DateTime dateTime = Conversion.UnixTimeStampToDateTime(stationsData.Timespan);
                    this.SetBranding(dateTime);
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(UnauthorizedAccessException))
                {
                    Notification.ShowMessage("Sorry! Couldn't get your position! No relevant information can be provided! Please go to Settings - Permissions, allow usage of location and then press Refresh button.");
                }
                else
                {
                    Notification.ShowMessage("Sorry! Could not load data! Try to reconnect to Internet and then press Refresh!"); 
                }
                
            }
                        
            this.InProgress = false;
        }

        private void HandleShowOnMapCommand(string parameter)
        {
            string model = GetSerializedStationModel();
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(MapView), model);
            }
        }
  
        private string GetSerializedStationModel()
        {
            List<Station> stations = new List<Station>();
            foreach (var station in this.NearestStations)
            {
                Station currentStation = new Station()
                {
                    StationId = station.StationId,
                    Name = station.Name,
                    Longitude = station.Longitude,
                    Latitude = station.Latitude,
                    FreeBikes = station.FreeBikes,
                    FreeDocks = station.FreeDocks
                };

                stations.Add(currentStation);
            }

            return JsonConvert.SerializeObject(stations.AsEnumerable());
        }

        private void HandleShowAllCommand(HomeViewModel parameter)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(AllStationsPage));
            }
        }

        private void HandleShowNearestCommand(string maxStations)
        {
            this.settings.MaxNearStations = int.Parse(maxStations);
            this.GetData();
        }

        private void HandleRefreshCommand(HomeViewModel parameter)
        {
            this.GetData();
        }

        private void SetBranding(DateTime dateTime)
        {

            string date = dateTime.ToString("d");
            string time = dateTime.ToString("t");
            this.Branding = String.Format("Barclays Cycle Hire data supplied at {1} on {0} by Transport for London", date, time);
        }

        internal void NavigateToDetails(StationFull selectedStation)
        {
            string model = JsonConvert.SerializeObject(selectedStation);
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(DetailsPage), model);
            }
        }
    }
}