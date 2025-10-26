using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Interfaces;
using UserNotifications;

namespace TrackYourTasks.Platforms.iOS
{
    public class NotificationManagerService : INotificationManagerService
    {
        bool hasPermission;

        public NotificationManagerService()
        {
            UNUserNotificationCenter.Current.Delegate = new NotificationReceiver();

            UNUserNotificationCenter.Current.RequestAuthorization(
                UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                (approved, err) =>
                {
                    hasPermission = approved;
                    Console.WriteLine($"Notification permission granted: {approved}");
                });
        }

        public void SendNotification(string title, string message)
        {
            if (!hasPermission)
            {
                Console.WriteLine("Notification permission not granted");
                return;
            }

            var content = new UNMutableNotificationContent
            {
                Title = title,
                Body = message,
                Sound = UNNotificationSound.Default
            };

            var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(0.1, false);
            var request = UNNotificationRequest.FromIdentifier(Guid.NewGuid().ToString(), content, trigger);

            UNUserNotificationCenter.Current.AddNotificationRequest(request, (err) =>
            {
                if (err != null)
                    Console.WriteLine($"Error scheduling notification: {err}");
            });
        }
    }
}
