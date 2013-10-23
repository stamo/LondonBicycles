
using Newtonsoft.Json;
namespace LondonBicycles.Data.Models
{
    public class StationShort
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        [JsonProperty("long")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
        public string FreeBikes { get; set; }
        public string FreeDocks { get; set; }
    }
}
