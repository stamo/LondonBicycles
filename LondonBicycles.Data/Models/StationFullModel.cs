using Newtonsoft.Json;
using System.Collections.Generic;

namespace LondonBicycles.Data.Models
{
    public class StationFullModel
    {
        [JsonProperty("timestamp")]
        public double Timespan { get; set; }
        public IEnumerable<StationFull> Stations { get; set; }
    }
}
