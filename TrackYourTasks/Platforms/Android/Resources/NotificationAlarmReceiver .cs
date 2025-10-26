using Android.App;
using Android.Content;
using Microsoft.Maui;

namespace TrackYourTasks.Platforms.Android.Services
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    public class NotificationAalarmReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var label = intent.GetStringExtra("NotificationLabel") ?? "Unknown";
            // Call your notification method for this label
            // SendPlatformNotification should be accessible here, for example:
            NotificationHelper notificationHelper = new NotificationHelper(context);
            notificationHelper.ShowInteractiveNotification("Reminder", "Do you want to proceed?", label);
        }
    }
}
