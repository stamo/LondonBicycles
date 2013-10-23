using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LondonBicycles.Data.Models;
using Windows.Devices.Geolocation;

namespace LondonBicycles.Data.LocationProvider
{
    public class Location
    {
        private Geolocator locator;
        private Geoposition position;
        private Task<Point> currentPosition;
        private Task<String> currentTown;

        public Task<Point> CurrentPosition
        {
            get
            {
                if (locator == null)
                {
                    locator = new Geolocator();
                }

                this.currentPosition = this.GetCurrentPositionAsync();
                return this.currentPosition; 
            }
        }

        public Task<String> CurrentTown
        {
            get
            {
                if (locator == null)
                {
                    locator = new Geolocator();
                }

                this.currentTown = this.GetCurrentCityAsync();
                return this.currentTown; 
            }
        }

        private async Task<string> GetCurrentCityAsync()
        {
            this.position = await locator.GetGeopositionAsync();
            string city = this.position.CivicAddress.City;

             return city;
        }

        private async Task<Point> GetCurrentPositionAsync()
        {
            Point currentCoordinates = new Point();

            this.position = await locator.GetGeopositionAsync();
            currentCoordinates.Latitude = this.position.Coordinate.Latitude;
            currentCoordinates.Longitude = this.position.Coordinate.Longitude;

             return currentCoordinates;
        }



    }
}
