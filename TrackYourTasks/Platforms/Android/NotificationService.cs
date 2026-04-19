//using Android.App;
//using Android.Content;
//using AndroidX.Core.App;
//using TrackYourTasks.Interfaces;
//using TrackYourTasks.Platforms.Android;
//using Application = Android.App.Application;
//using Resource = Microsoft.Maui.Controls.Resource;

//public class NotificationService : INotificationService
//{
//    public void ShowNotificationWithActions(string title, string message)
//    {
//        var context = Application.Context;
//        var channelId = "default";
//        var intentYes = new Intent(context, typeof(YesReceiver));
//        var intentNo = new Intent(context, typeof(NoReceiver));
//        PendingIntent pendingIntentYes = PendingIntent.GetBroadcast(context, 0, intentYes, PendingIntentFlags.UpdateCurrent);
//        PendingIntent pendingIntentNo = PendingIntent.GetBroadcast(context, 1, intentNo, PendingIntentFlags.UpdateCurrent);

//        var builder = new NotificationCompat.Builder(context, channelId)
//            .SetContentTitle(title)
//            .SetContentText(message)
//            .SetSmallIcon(Resource.Drawable.notification_bg_low_normal)
//            .AddAction(Resource.Drawable.ic_yes, "Yes", pendingIntentYes)
//            .AddAction(Resource.Drawable.ic_no, "No", pendingIntentNo);

//        var notificationManager = NotificationManagerCompat.From(context);
//        notificationManager.Notify(1001, builder.Build());
//    }
//}

//[BroadcastReceiver(Enabled = true, Exported = true)]
//public class YesReceiver : BroadcastReceiver
//{
//    public override void OnReceive(Context context, Intent intent)
//    {
//        // Logic to handle "Yes" response
//        Console.WriteLine("User selected YES");
//    }
//}

//[BroadcastReceiver(Enabled = true, Exported = true)]
//public class NoReceiver : BroadcastReceiver
//{
//    public override void OnReceive(Context context, Intent intent)
//    {
//        // Logic to handle "No" response
//        Console.WriteLine("User selected NO");
//    }
//}
