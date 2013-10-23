using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LondonBicycles.Client.Settings
{
    public class UserSettings
    {
        private int maxNearStations;

        public int MaxNearStations
        {
            get 
            {
                var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

                if (roamingSettings.Values["maxNearStations"] == null)
                {
                    roamingSettings.Values["maxNearStations"] = 5;
                }

                this.maxNearStations = (int)roamingSettings.Values["maxNearStations"];
                return maxNearStations; 
            }

            set 
            {
                var roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

                roamingSettings.Values["maxNearStations"] = value;
                maxNearStations = value; 
            }
        }
        
    }
}
