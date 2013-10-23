using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LondonBicycles.Data.Models
{
    public class StationFull
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        [JsonProperty("long")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
        public string DistanceWalk { get; set; }
        public string TimeWalk { get; set; }
        public string DistanceCycle { get; set; }
        public string TimeCycle { get; set; }
        public string FreeBikes { get; set; }
        public string FreeDocks { get; set; }
    }
}
