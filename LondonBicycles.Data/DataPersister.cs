using LondonBicycles.Data.HttpRequester;
using LondonBicycles.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LondonBicycles.Data
{
    public static class DataPersister
    {
        const string BaseUrl = "http://locatebc.eu01.aws.af.cm/"; 

        public async static Task<HttpResponseMessage> SendReport(string message)
        {
            StringBuilder url = new StringBuilder(BaseUrl);
            url.Append("sendErrorReport.php");
            return await HttpRequest.Post(url.ToString(), message);
        }

        public async static Task<StationShortModel> GetAllStations()
        {
            StringBuilder url = new StringBuilder(BaseUrl);
            url.Append("getStations.php");
            return await HttpRequest.Get<StationShortModel>(url.ToString());
        }

        public async static Task<StationFullModel> GetNearestStations(Point currentPosition, int limit)
        {
            StringBuilder url = new StringBuilder(BaseUrl);
            url.Append("getStations.php?");
            url.AppendFormat("long={0}&lat={1}&max={2}", currentPosition.Longitude, currentPosition.Latitude, limit);
            return await HttpRequest.Get<StationFullModel>(url.ToString());
        }

        public async static Task<RoutePointsModel> GetRoutePoints(Point origin, Point destination)
        {
            StringBuilder url = new StringBuilder(BaseUrl);
            url.Append("getRoute.php?");
            url.AppendFormat("origin_long={0}&origin_lat={1}&destination_long={2}&destination_lat={3}",
                origin.Longitude, origin.Latitude, destination.Longitude, destination.Latitude);
            return await HttpRequest.Get<RoutePointsModel>(url.ToString());
        }

        public async static Task<StationFullModel> GetStationById(int stationId, Point myLocation)
        {
            StringBuilder url = new StringBuilder(BaseUrl);
            url.Append("getStationById.php?");
            url.AppendFormat("long={0}&lat={1}&id={2}",
                myLocation.Longitude, myLocation.Latitude, stationId);
            return await HttpRequest.Get<StationFullModel>(url.ToString());
        }
    }
}
