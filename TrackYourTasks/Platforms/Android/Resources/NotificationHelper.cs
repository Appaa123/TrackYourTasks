using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using TrackYourTasks.Services;

namespace TrackYourTasks.Platforms.Android.Services
{
    public class NotificationHelper
    {
        private const string ChannelId = "interactive_channel";
        private readonly Context _context;

        public NotificationHelper(Context context)
        {
            _context = context;
            CreateChannel();
        }

        private void CreateChannel()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(ChannelId, "Interactive", NotificationImportance.High)
                {
                    Description = "Interactive Notifications"
                };
                var manager = (NotificationManager)_context.GetSystemService(Context.NotificationService);
                manager.CreateNotificationChannel(channel);
            }
        }

        public void ShowInteractiveNotification(string title, string message, string timeLabel)
        {
            Console.WriteLine("Entered ShowInteractiveNotification");
            var yesIntent = new Intent(_context, typeof(NotificationActionReceiver));
            yesIntent.SetAction("ACTION_YES");
            yesIntent.PutExtra("NotificationTime", timeLabel); // e.g. "9AM","1PM","9PM"

            var yesPendingIntent = PendingIntent.GetBroadcast(
                _context,
                new Random().Next(1000, 9999), // unique requestCode
                yesIntent,
                PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable
            );

            // Optional NO action
            var noIntent = new Intent(_context, typeof(NotificationActionReceiver));
            noIntent.SetAction("ACTION_NO");
            var noPending = PendingIntent.GetBroadcast(_context, 1, noIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable);

            var builder = new NotificationCompat.Builder(_context, ChannelId)
                .SetContentTitle(title)
                .SetContentText(message)
                .SetSmallIcon(Resource.Drawable.notification_icon)
                .SetPriority(NotificationCompat.PriorityHigh)
                .SetAutoCancel(true)
                .AddAction(Resource.Drawable.ic_yes, "YES", yesPendingIntent)
                .AddAction(Resource.Drawable.ic_no, "NO", noPending)
                .SetPriority((int)NotificationPriority.High);

            var notificationManager = NotificationManagerCompat.From(_context);
            notificationManager.Notify(new Random().Next(100, 999), builder.Build());

            //NotificationManagerCompat.From(_context).Notify(1001, builder.Build());

            Console.WriteLine("Completed ShowInteractiveNotification");
        }
    }
}
