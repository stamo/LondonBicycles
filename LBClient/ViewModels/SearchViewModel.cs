using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LondonBicycles.Client.Common;
using System.Collections.ObjectModel;
using LondonBicycles.Data.Models;
using LondonBicycles.Data;
using LondonBicycles.Client.Helpers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using LondonBicycles.Client.Views;
using LondonBicycles.Data.LocationProvider;
using Newtonsoft.Json;

namespace LondonBicycles.Client.ViewModels
{
    public class SearchViewModel : BindableBase
    {
        private string queryText;
        private ObservableCollection<StationShort> resultStations;
        private bool inProgress;
        private string branding;

        public string QueryText
        {
            get 
            { 
                return this.queryText; 
            }
            set 
            { 
                this.queryText = value;
                this.OnPropertyChanged("QueryText");
                this.LoadResults(); 
            }
        }

        public IEnumerable<StationShort> ResultStations
        {
            get
            {
                if (this.resultStations == null)
                {
                    this.resultStations = new ObservableCollection<StationShort>();
                }

                return this.resultStations;
            }
            set
            {
                this.resultStations.Clear();

                foreach (var item in value)
                {
                    this.resultStations.Add(item);
                }
            }
        }

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

        private void SetBranding(DateTime dateTime)
        {

            string date = dateTime.ToString("d");
            string time = dateTime.ToString("t");
            this.Branding = String.Format("Barclays Cycle Hire data supplied at {1} on {0} by Transport for London", date, time);
        }
  
        private async void LoadResults()
        {
            if (this.QueryText == String.Empty)
            {
                this.NavigateToHome();
            }
            else
            {
                if (this.resultStations != null)
                {
                    this.resultStations.Clear();
                }

                this.InProgress = true;
                StationShortModel stationsData = new StationShortModel();

                try
                {
                    stationsData = await DataPersister.GetAllStations();
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
            
                this.ResultStations = from s in stationsData.Stations
                                      where s.Name.ToLower().Contains(this.QueryText.ToLower())
                                      select s;
                this.InProgress = false;
            }
            
        }

        private void NavigateToHome()
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(HomePage), null);
            }
        }

        private Task<StationFullModel> GetStation(int stationId, Point myLocation)
        {
            return DataPersister.GetStationById(stationId, myLocation);
        }

        internal async void NavigateToDetails(StationShort selectedStation)
        {
            this.resultStations.Clear();
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
    }
}
