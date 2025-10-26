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
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 00, 41);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 00, 42);
            ScheduleNotification("Morning Excecise Reminder", "Good morning! Plan your day.", 9, 0);
            ScheduleNotification("Afternoon Food Reminder", "Take a short break!", 13, 0);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 21, 0);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 00, 09);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 00, 46);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 00, 45);
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
