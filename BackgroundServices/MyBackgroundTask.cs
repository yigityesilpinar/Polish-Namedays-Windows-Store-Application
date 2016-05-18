using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundServices
{
    public sealed class MyBackgroundTask : IBackgroundTask
    {
        // Run method will include async method
        // Since await returns from Run method to system
        // We need to indicate its not the end and not to kill Process
        // Deferral object to indicate when to finish (Complete)

        private BackgroundTaskDeferral _deferral;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            _deferral = taskInstance.GetDeferral();

            var settings = new Settings();

            if (settings.NotificationsEnabled)
                await SendNotificationAsync();

            if (settings.UpdatingLiveTileEnabled)
                await UpdateTileAsync();

            _deferral.Complete();
        }

        // Windows Live Tile 
        private async static Task UpdateTileAsync()
        {
            var todayNames = await NamedayRepository.GetTodaysNamesAsStringAsync();
            if (todayNames == null)
                return;
            // Hardcoded XML template for Tile with 2 bindings
            var template =
@"<tile>
 <visual version=""4"">
  <binding template=""TileMedium"">
   <text hint-wrap=""true"">{0}</text>
  </binding>
  <binding template=""TileWide"">
   <text hint-wrap=""true"">{0}</text>
  </binding>
 </visual>
</tile>";
            var content = string.Format(template,todayNames);
            var doc = new Windows.Data.Xml.Dom.XmlDocument();
            doc.LoadXml(content);

            // Create and Update Tile Notification
            TileUpdateManager.CreateTileUpdaterForApplication().
                Update(new TileNotification(doc));
            

        }

        private async Task SendNotificationAsync()
        {
            var todayNames = await NamedayRepository.GetTodaysNamesAsStringAsync();
            if (todayNames == null)
                return;

            ToastNotifier notifier = ToastNotificationManager.CreateToastNotifier();
            XmlDocument content = ToastNotificationManager.GetTemplateContent(
                ToastTemplateType.ToastText02);

            // XML File content
            var texts = content.GetElementsByTagName("text");

            texts[0].InnerText = todayNames.Contains(",") ?
                "Today's namedays are" : "Today's nameday is";

            texts[1].InnerText = todayNames;

            // Create and show notification
            notifier.Show(new ToastNotification(content));
        }

        // Register Background Task
        public static async void Register()
        {
            // check if task already registered, true if task registered already
            var isRegistered = BackgroundTaskRegistration.AllTasks.Values.Any(
                t => t.Name == nameof(MyBackgroundTask));

            if (isRegistered)
                return;

            //  Access denied
            if (await BackgroundExecutionManager.RequestAccessAsync() == BackgroundAccessStatus.Denied)
                return;

            // object to be registered
            // Nameand and entrypoint(Namespace.ClassName)
            var builder = new BackgroundTaskBuilder
            {
                Name = nameof(MyBackgroundTask),
                TaskEntryPoint = $"{nameof(BackgroundServices)}.{nameof(MyBackgroundTask)}"
            };

            // Set triger 120 minutes
            builder.SetTrigger(new TimeTrigger(120, false));

            // Register the task
            builder.Register();
        }
    }
}
