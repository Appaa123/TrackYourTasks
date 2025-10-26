using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using TrackYourTasks.Platforms.Android.Services;

namespace TrackYourTasks
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        void ScheduleRepeatingNotifications(Context context)
        {
            Console.WriteLine("Listening......");
            ScheduleAlarm(context, 9, 0, "Excercise");
            ScheduleAlarm(context, 13, 0, "Food");
            ScheduleAlarm(context, 21, 0, "Sleep");
            ScheduleAlarm(context, 23, 47, "Sleep");
            ScheduleAlarm(context, 23, 48, "Sleep");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Console.WriteLine("OnCreate Started.......");
            base.OnCreate(savedInstanceState);

            // Schedule your repeating notifications once app starts
            ScheduleRepeatingNotifications(this);
        }

        void ScheduleAlarm(Context context, int hourOfDay, int minute, string label)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);
            var intent = new Intent(context, typeof(NotificationAalarmReceiver));
            intent.PutExtra("NotificationLabel", label);

            Console.WriteLine("NotificationLabel : " + label);

            var pendingIntent = PendingIntent.GetBroadcast(context, label.GetHashCode(), intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

            // Compute calendar for next time.
            var calendar = Java.Util.Calendar.Instance;
            calendar.Set(Java.Util.CalendarField.HourOfDay, hourOfDay);
            calendar.Set(Java.Util.CalendarField.Minute, minute);
            calendar.Set(Java.Util.CalendarField.Second, 0);

            if (calendar.TimeInMillis < Java.Lang.JavaSystem.CurrentTimeMillis())
                calendar.Add(Java.Util.CalendarField.DayOfYear, 1); // Next day if already passed

            alarmManager.SetRepeating(
                AlarmType.RtcWakeup,
                calendar.TimeInMillis,
                AlarmManager.IntervalDay,
                pendingIntent
            );
        }

    }
}
