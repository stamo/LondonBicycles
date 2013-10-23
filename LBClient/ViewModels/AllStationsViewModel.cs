using LondonBicycles.Client.Behavior;
using LondonBicycles.Client.Common;
using LondonBicycles.Client.Helpers;
using LondonBicycles.Client.Views;
using LondonBicycles.Data;
using LondonBicycles.Data.LocationProvider;
using LondonBicycles.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace LondonBicycles.Client.ViewModels
{
    class AllStationsViewModel : BindableBase
    {
        private ObservableCollection<StationShort> allStations;
        private bool inProgress;
        private ICommand refreshCommand;
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
                return branding; 
            }

            set 
            {
                branding = value;
                OnPropertyChanged("Branding");
            }
        }

        public ICommand Refresh
        {
            get
            {
                if (this.refreshCommand == null)
                {
                    this.refreshCommand = new DelegateCommand<AllStationsViewModel>(this.HandleRefreshCommand);
                }
                return this.refreshCommand;
            }
        }

        public IEnumerable<StationShort> AllStations
        {
            get
            {
                if (this.allStations == null)
                {
                    this.allStations = new ObservableCollection<StationShort>();
                }

                return this.allStations;
            }
            set
            {
                if (this.allStations == null)
                {
                    this.allStations = new ObservableCollection<StationShort>();
                }

                this.SetObservableValues(this.allStations, value);
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

         public AllStationsViewModel()
        {
            this.GetData();
        }

        private void HandleRefreshCommand(AllStationsViewModel parameter)
        {
            this.GetData();
        }

        private async void GetData()
        {
            if (this.allStations != null)
            {
                this.allStations.Clear();
            }

            this.InProgress = true;
            StationShortModel stationsData = new StationShortModel();

            try
            {
                stationsData = await DataPersister.GetAllStations();
                this.AllStations = stationsData.Stations;
                if (stationsData.Timespan != 0)
                {
                    DateTime dateTime = Conversion.UnixTimeStampToDateTime(stationsData.Timespan);
                    this.SetBranding(dateTime);
                }
            }
            catch (Exception)
            {
                Notification.ShowMessage("Sorry! Could not load data! Try to reconnect to Internet and then press Refresh!");
            }
            

            this.InProgress = false;
        }

        private void SetBranding(DateTime dateTime)
        {

            string date = dateTime.ToString("d");
            string time = dateTime.ToString("t");
            this.Branding = String.Format("Barclays Cycle Hire data supplied at {1} on {0} by Transport for London", date, time);
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
            foreach (var station in this.allStations)
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

        internal async void NavigateToDetails(StationShort selectedStation)
        {
            this.allStations.Clear();
            this.InProgress = true;

            try
            {
                Location locator = new Location();
                Point myLocation = await locator.CurrentPosition;

                StationFullModel stationModel = await GetStation(selectedStation.StationId, myLocation);
                string model = JsonConvert.SerializeObject(stationModel.Stations.First());
                Frame rootFrame = Window.Current.Content as Frame;

                this.InProgress = false;
                if (rootFrame != null)
                {
                    rootFrame.Navigate(typeof(DetailsPage), model);
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
            
        }

        private Task<StationFullModel> GetStation(int stationId, Point myLocation)
        {
            return DataPersister.GetStationById(stationId, myLocation);
        }
    }
}
