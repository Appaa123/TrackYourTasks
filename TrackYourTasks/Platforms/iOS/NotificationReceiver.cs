using System;
using UserNotifications;

namespace TrackYourTasks.Platforms.iOS
{
    public class NotificationReceiver : UNUserNotificationCenterDelegate
    {
        public override void DidReceiveNotificationResponse(
            UNUserNotificationCenter center,
            UNNotificationResponse response,
            Action completionHandler)
        {
            switch (response.ActionIdentifier)
            {
                case "action_yes":
                    Console.WriteLine("User selected YES");
                    // Handle Yes action logic here
                    break;

                case "action_no":
                    Console.WriteLine("User selected NO");
                    // Handle No action logic here
                    break;

                default:
                    Console.WriteLine("Notification tapped");
                    break;
            }

            completionHandler();
        }
    }
}
