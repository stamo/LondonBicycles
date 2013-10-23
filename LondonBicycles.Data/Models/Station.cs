using System.Text;

namespace LondonBicycles.Data.Models
{
    public class Station
    {
        public int StationId { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string FreeBikes { get; set; }
        public string FreeDocks { get; set; }

        public override string ToString()
        {
            StringBuilder stationAsString = new StringBuilder();
            stationAsString.AppendFormat("{0}\n", this.Name);
            stationAsString.AppendFormat("Free bikes: {0}\n", this.FreeBikes);
            stationAsString.AppendFormat("Free docks: {0}", this.FreeDocks);

            return stationAsString.ToString();
        }
    }
}
