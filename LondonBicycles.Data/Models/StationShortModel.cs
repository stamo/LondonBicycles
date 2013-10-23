using Newtonsoft.Json;
using System.Collections.Generic;

namespace LondonBicycles.Data.Models
{
    public class StationShortModel
    {
        [JsonProperty("timestamp")]
        public double Timespan { get; set; }
        public IEnumerable<StationShort> Stations { get; set; }
    }
}
