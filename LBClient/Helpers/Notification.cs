using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace LondonBicycles.Client.Helpers
{
    public static class Notification
    {
        public async static void ShowMessage(string message)
        {
            var messageDialog = new MessageDialog(message);
            await messageDialog.ShowAsync();
        }

        public static void ShowToast(string message, string imageRelativePath = null)
        {
            XmlDocument toastXml;

            if (imageRelativePath != null)
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText01;
                toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");
                ((XmlElement)toastImageAttributes.First()).SetAttribute("src", "ms-appx:///" + imageRelativePath);
                ((XmlElement)toastImageAttributes.First()).SetAttribute("alt", "toast image");
            }
            else
            {
                ToastTemplateType toastTemplate = ToastTemplateType.ToastText01;
                toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
            }
            
            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");

            toastTextElements.First().AppendChild(toastXml.CreateTextNode(message));

            ToastNotification toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static async void UserChoice(string message, string confirmButtonLabel, string rejectButtonLabel, UICommandInvokedHandler handler)
        {
            var messageDialog = new MessageDialog(message);

            messageDialog.Commands.Add(new UICommand(
                confirmButtonLabel, 
                handler));
            messageDialog.Commands.Add(new UICommand(
                rejectButtonLabel, 
                handler));
            messageDialog.DefaultCommandIndex = 0;
            messageDialog.CancelCommandIndex = 1;

            await messageDialog.ShowAsync();
        }
    }
}
