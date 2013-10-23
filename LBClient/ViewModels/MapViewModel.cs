using Bing.Maps;
using Bing.Maps.Search;
using LondonBicycles.Client.Helpers;
using LondonBicycles.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LondonBicycles.Client.ViewModels
{
    public class MapViewModel
    {
        public IEnumerable<Station> Stations { get; set; }
        public Map Map { get; set; }

        public MapViewModel()
        {
            this.Stations = new List<Station>();
        }

        public MapViewModel(string parameter, Map map)
        {
            this.Stations = JsonConvert.DeserializeObject<IEnumerable<Station>>(parameter);
            this.Map = map;
            this.SetUpMap();
        }

        private async void SetUpMap()
        {
            LocationCollection locations = new LocationCollection();
            foreach (var station in this.Stations)
            {
                Location location = new Location() { 
                    Latitude = station.Latitude,
                    Longitude = station.Longitude
                };

                locations.Add(location);
                DrawPin.SetStationPin(location, this.Map, station);
            }

            try
            {
                Data.LocationProvider.Location locator = new Data.LocationProvider.Location();
                Point currentLocation = await locator.CurrentPosition;

                if (currentLocation != null)
                {
                    Location myLocation = new Location(currentLocation.Latitude, currentLocation.Longitude);
                    string myCity = await GetCityByLocation(myLocation);
                    DrawPin.SetMyLocationPin(myLocation, this.Map);
                    if (myCity.Contains("London"))
                    {
                        locations.Add(myLocation);
                    }
                    LocationRect mapRectangle = new LocationRect(locations);
                    this.Map.SetView(mapRectangle);
                }
                else
                {
                    Notification.ShowMessage("Sorry! Couldn't get your position! No relevant information can be provided!");
                }
            }
            catch (Exception)
            {
                Notification.ShowMessage("Sorry! Could not load data! Try to reconnect to Internet and then press Refresh!");
            }
        }

        private async Task<string> GetCityByLocation(Location myLocation)
        {
            ReverseGeocodeRequestOptions requestOptions = new ReverseGeocodeRequestOptions(myLocation);
            SearchManager manager = this.Map.SearchManager;
            LocationDataResponse responce = await manager.ReverseGeocodeAsync(requestOptions);
            return responce.LocationData.First().Address.AdminDistrict2;
        }

       
    }
}
