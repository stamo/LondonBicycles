using Bing.Maps;
using LondonBicycles.Client.Behavior;
using LondonBicycles.Client.Common;
using LondonBicycles.Client.Helpers;
using LondonBicycles.Data;
using LondonBicycles.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace LondonBicycles.Client.ViewModels
{
    class DetailsViewModel : BindableBase
    {
        private StationFull station;
        private Map map;
        private RoutePointsModel route;
        private StorageFile gpxFile;
        private ICommand saveRouteFileCommand;

        public StationFull Station
        {
            get 
            { 
                return this.station; 
            }

            set 
            {
                this.station = value;
                OnPropertyChanged("Station");
            }
        }

        public ICommand SaveRouteFile
        {
            get
            {
                if (this.saveRouteFileCommand == null)
                {
                    this.saveRouteFileCommand = new DelegateCommand<string>(this.HandleSaveRouteFileCommand);
                }
                return this.saveRouteFileCommand;
            }
        }

        public DetailsViewModel()
        {
            this.station = new StationFull();
        }

        public DetailsViewModel(string parameter, Map map)
        {
            this.station = JsonConvert.DeserializeObject<StationFull>(parameter);
            
            this.map = map;
            this.CalculateRoute();
        }
  
        private async void HandleSaveRouteFileCommand(string parameter)
        {
            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            savePicker.FileTypeChoices.Add("Navigation File", new List<string>() { ".gpx" });
            savePicker.SuggestedFileName = "Route";

            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                try
                {
                    string xmlAsString = await Windows.Storage.FileIO.ReadTextAsync(this.gpxFile);
                    await Windows.Storage.FileIO.WriteTextAsync(file, xmlAsString);
                    Notification.ShowToast("File saved successfully!", "Assets/Logo.png");
                }
                catch (Exception)
                {
                    Notification.ShowToast("File could not be saved!", "Assets/Logo.png");
                }
                
            }
        }

        private async void CalculateRoute()
        {
            LondonBicycles.Data.Models.Point myLocation = null;
            LondonBicycles.Data.Models.Point destination = new LondonBicycles.Data.Models.Point() {
                Longitude = this.station.Longitude,
                Latitude = this.station.Latitude
            };

            try
            {
                LondonBicycles.Data.LocationProvider.Location locator = new LondonBicycles.Data.LocationProvider.Location();
                myLocation = await locator.CurrentPosition;
                this.route = await DataPersister.GetRoutePoints(myLocation, destination);
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(UnauthorizedAccessException))
                {
                    Notification.ShowMessage("Sorry! Couldn't get your position! No relevant information can be provided! Please go to Settings - Permissions, allow usage of location and then press Refresh button.");
                }
                else
                {
                    Notification.ShowMessage("Sorry! Could not load data! Try to reconnect to Internet and then press Refresh!"); 
                }
                
            }

            if (this.route != null && this.route.RoutePoints.Count() > 0)
            {
                SetRouteOnMap(myLocation, destination);
            }
        }
  
        private void SetRouteOnMap(LondonBicycles.Data.Models.Point myLocation, LondonBicycles.Data.Models.Point destination)
        {
            LocationCollection wayPoints = new LocationCollection();
            Bing.Maps.Location startWaypoint = new Bing.Maps.Location(myLocation.Latitude, myLocation.Longitude);
            Bing.Maps.Location endWaypoint = new Bing.Maps.Location(destination.Latitude, destination.Longitude);
            wayPoints.Add(startWaypoint);

            foreach (var routePoint in this.route.RoutePoints)
            {
                Bing.Maps.Location wayPoint = new Bing.Maps.Location()
                {
                    Latitude = routePoint.Latitude,
                    Longitude = routePoint.Longitude
                };

                wayPoints.Add(wayPoint);
            }

            wayPoints.Add(endWaypoint);

            DrawRoute(wayPoints);
            DrawPin.SetMyLocationPin(startWaypoint, this.map);
            DrawPin.SetStationPin(endWaypoint, this.map);
            this.GenerateGpx(myLocation, destination);
        }
  
        private void DrawRoute(LocationCollection wayPoints)
        {
            MapShapeLayer shapeLayer = new MapShapeLayer();
            MapPolyline polyline = new MapPolyline();
            polyline.Locations = wayPoints;
            polyline.Color = Windows.UI.Colors.Red;
            polyline.Width = 5;
            shapeLayer.Shapes.Add(polyline);
            LocationRect mapRectangle = new LocationRect(wayPoints);
            this.map.SetView(mapRectangle);
            this.map.ShapeLayers.Add(shapeLayer);
        }

        private async void GenerateGpx(LondonBicycles.Data.Models.Point myLocation, LondonBicycles.Data.Models.Point destination)
        {
            if (this.route.RoutePoints.Count() > 0)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement gpx = doc.CreateElement("gpx");

                gpx.SetAttribute("xmlns", "http://www.topografix.com/GPX/1/1");
                gpx.SetAttribute("version", "1.1");
                gpx.SetAttribute("creator", "London Bicycles");
                gpx.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");

                XmlElement wptStart = doc.CreateElement("wpt");

                wptStart.SetAttribute("lon", myLocation.Longitude.ToString());
                wptStart.SetAttribute("lat", myLocation.Latitude.ToString());

                gpx.AppendChild(wptStart);

                XmlElement wptEnd = doc.CreateElement("wpt");

                wptEnd.SetAttribute("lon", destination.Longitude.ToString());
                wptEnd.SetAttribute("lat", destination.Latitude.ToString());

                gpx.AppendChild(wptEnd);

                XmlElement track = doc.CreateElement("trk");
                XmlElement trackSegment = doc.CreateElement("trkseg");

                foreach (var point in this.route.RoutePoints)
                {
                    XmlElement trackPoint = doc.CreateElement("trkpt");
                    XmlAttribute lonTrack = doc.CreateAttribute("lon");
                    lonTrack.Value = point.Longitude.ToString();
                    XmlAttribute latTrack = doc.CreateAttribute("lat");
                    latTrack.Value = point.Latitude.ToString();

                    trackPoint.SetAttribute("lon", point.Longitude.ToString());
                    trackPoint.SetAttribute("lat", point.Latitude.ToString());
                    trackSegment.AppendChild(trackPoint);
                }

                track.AppendChild(trackSegment);
                gpx.AppendChild(track);
                doc.AppendChild(gpx);

                string text = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                text += doc.GetXml();

                StorageFolder sf = Windows.Storage.ApplicationData.Current.RoamingFolder;
                StorageFile file = await sf.CreateFileAsync("route.gpx", CreationCollisionOption.ReplaceExisting);
                Windows.Storage.FileIO.WriteTextAsync(file, text);

                this.gpxFile = file;
                this.RegisterForShare();
            }
        }

        public void RegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, 
                DataRequestedEventArgs>(this.ShareStorageItemsHandler);
        }

        public void DeRegisterForShare()
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            try
            {
                dataTransferManager.DataRequested -= this.ShareStorageItemsHandler;
            }
            catch (Exception) 
            { 
            }
        }

        private async void ShareStorageItemsHandler(DataTransferManager sender, 
            DataRequestedEventArgs e)
        {
            DataRequest request = e.Request;
            request.Data.Properties.Title = "Share Route";
            request.Data.Properties.Description = "Share this route with your navigation software.";
    
            DataRequestDeferral deferral = request.GetDeferral();  

            try
            {
                List<IStorageItem> storageItems = new List<IStorageItem>();
                storageItems.Add(this.gpxFile);
                request.Data.SetStorageItems(storageItems);       
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
