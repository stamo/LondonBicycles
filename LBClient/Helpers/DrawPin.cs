using Bing.Maps;
using LondonBicycles.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace LondonBicycles.Client.Helpers
{
    public static class DrawPin
    {
         public static void SetMyLocationPin(Location currentLocation, Map map)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("ms-appx:/Assets/pin.png"));
            image.Width = 42;
            image.Height = 65;
            MapLayer.SetPosition(image, currentLocation);
            MapLayer.SetPositionAnchor(image, new Windows.Foundation.Point(21, 65));
            map.Children.Add(image);
        }

        public static void SetStationPin(Location location, Map map, Station station = null)
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri("ms-appx:/Assets/pushpin.gif"));
            image.Width = 24;
            image.Height = 32;
            image.Tag = station;
            if (station != null)
            {
                ToolTip stationTooltip = new ToolTip();
                stationTooltip.Content = station.ToString();
                ToolTipService.SetToolTip(image, stationTooltip);
            }
            
            MapLayer.SetPosition(image, location);
            MapLayer.SetPositionAnchor(image, new Windows.Foundation.Point(12, 32));
            map.Children.Add(image);
        }

    }
}
