
using Newtonsoft.Json;
namespace LondonBicycles.Data.Models
{
    public class Point
    {
        [JsonProperty("long")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }
}
