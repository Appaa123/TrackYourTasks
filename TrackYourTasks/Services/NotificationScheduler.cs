using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Services
{
    public static class NotificationScheduler
    {
        public static void ScheduleDailyNotifications()
        {
            ScheduleNotification("Morning Reminder", "Good morning! Plan your day.", 9, 0);
            ScheduleNotification("Afternoon Reminder", "Take a short break!", 13, 0);
            ScheduleNotification("Night Reminder", "Review your tasks before sleep.", 21, 0);
        }

        private static void ScheduleNotification(string title, string message, int hour, int minute)
        {
            var notifyTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
            if (notifyTime < DateTime.Now)
                notifyTime = notifyTime.AddDays(1);

            var request = new NotificationRequest
            {
                NotificationId = new Random().Next(1000, 9999),
                Title = title,
                Description = message,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime,
                    RepeatType = NotificationRepeat.Daily
                }
            };

            LocalNotificationCenter.Current.Show(request);
        }
    }
}
