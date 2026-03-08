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
            ScheduleNotification("Morning Excecise Reminder", "Good morning! Plan your day.", 23, 26);
            ScheduleNotification("Afternoon Food Reminder", "Take a short break!", 23, 30);
            ScheduleNotification("Night Sleep Reminder", "Review your tasks before sleep.", 23, 28);
        } 

        private static void ScheduleNotification(string title, string message, int hour, int minute)
        {
            var notifyTime = DateTime.Today.AddHours(hour).AddMinutes(minute);
            string userCompletedData = string.Empty;
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

            userCompletedData = request.ReturningData;

            LocalNotificationCenter.Current.Show(request);
        }
    }
}
