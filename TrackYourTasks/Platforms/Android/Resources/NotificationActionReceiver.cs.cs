using Android.App;
using Android.Content;
using Android.Widget;
using CommunityToolkit.Maui.Alerts;
using Microsoft.Maui;
using Toast = Android.Widget.Toast;



namespace TrackYourTasks.Platforms.Android.Services
{
    [BroadcastReceiver(Enabled = true, Exported = true)]
    [IntentFilter(new[] { "ACTION_YES", "ACTION_NO" })]
    public class NotificationActionReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var prefs = context.GetSharedPreferences("GlobalData", FileCreationMode.Private);
            var editor = prefs.Edit();

            var action = intent.Action;

            if (action == "ACTION_YES")
            {
                editor.PutBoolean("UserClickedYes", true);
                if (intent.HasExtra("NotificationLabel"))
                {
                    var label = intent.GetStringExtra("NotificationLabel");
                    editor.PutString("UserNotificationLabel", label);
                    Console.WriteLine("NotificationLabel-NotificationActionReceiver : " + label);
                }
                editor.Apply(); // saves asynchronously
                Toast.MakeText(context, "You selected YES", ToastLength.Short).Show();
                // Optionally execute logic via MessagingCenter or static services
            }
            else if (action == "ACTION_NO")
            {
                editor.PutBoolean("UserClickedYes", false);
                editor.Apply();
                Toast.MakeText(context, "You selected NO", ToastLength.Short).Show();
            }
        }
    }
}
