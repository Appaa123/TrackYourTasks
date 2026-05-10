using Android.App;
using Android.Content;
using Android.OS;
using System;

namespace TrackYourTasks.Platforms.Android.Services
{
    public static class AlarmScheduler
    {
        /// <summary>
        /// Schedule an exact alarm that will fire the existing BroadcastReceiver in your Android platform project.
        /// Uses SetExactAndAllowWhileIdle on API >= M so Doze won't prevent the alarm.
        /// </summary>
        public static void ScheduleAlarm(global::Android.Content.Context context, DateTime notifyTimeLocal, string label, int requestCode)
        {
            try
            {
                var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

                // Use the same broadcast receiver type used in MainActivity.ScheduleAlarm
                var intent = new Intent(context, typeof(TrackYourTasks.Platforms.Android.Services.NotificationAalarmReceiver));
                intent.PutExtra("NotificationLabel", label ?? string.Empty);

                var pendingIntent = PendingIntent.GetBroadcast(context, requestCode, intent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);

                var calendar = Java.Util.Calendar.Instance;
                calendar.Set(Java.Util.CalendarField.Year, notifyTimeLocal.Year);
                calendar.Set(Java.Util.CalendarField.Month, notifyTimeLocal.Month - 1); // Java months are 0-based
                calendar.Set(Java.Util.CalendarField.DayOfMonth, notifyTimeLocal.Day);
                calendar.Set(Java.Util.CalendarField.HourOfDay, notifyTimeLocal.Hour);
                calendar.Set(Java.Util.CalendarField.Minute, notifyTimeLocal.Minute);
                calendar.Set(Java.Util.CalendarField.Second, notifyTimeLocal.Second);
                calendar.Set(Java.Util.CalendarField.Millisecond, notifyTimeLocal.Millisecond);

                long triggerAtMillis = calendar.TimeInMillis;

                // If the computed time is in the past, move it forward a little (or adjust as needed)
                if (triggerAtMillis <= Java.Lang.JavaSystem.CurrentTimeMillis())
                {
                    triggerAtMillis = Java.Lang.JavaSystem.CurrentTimeMillis() + 1000;
                }

                if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
                {
                    alarmManager.SetExactAndAllowWhileIdle(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }
                else
                {
                    alarmManager.SetExact(AlarmType.RtcWakeup, triggerAtMillis, pendingIntent);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AlarmScheduler.ScheduleAlarm failed: {ex}");
            }
        }
    }
}