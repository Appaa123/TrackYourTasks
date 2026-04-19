//using Plugin.LocalNotification;
//using Plugin.LocalNotification.AndroidOption;
//using System;
//using TrackYourTasks.Interfaces;
//using INotificationService = TrackYourTasks.Interfaces.INotificationService;

//namespace TrackYourTasks.Services
//{
//    public class NotificationService : INotificationService
//    {
//        public void ShowNotificationWithActions(string title, string message)
//        {
//            try
//            {
//                var request = new NotificationRequest
//                {
//                    NotificationId = 1001,
//                    Title = "Confirm Action",
//                    Description = "Do you want to proceed?",
//                    BadgeNumber = 1,
//                    Schedule = new NotificationRequestSchedule
//                    {
//                        NotifyTime = DateTime.Now.AddSeconds(2)
//                    },
//                    Android = new AndroidOptions
//                    {
//                        LaunchAppWhenTapped = true,
//                        Ongoing = true

//                    },
//                };

//                LocalNotificationCenter.Current.Show(request);
//            }
//            catch (Exception ex)
//            {
//                // Fallback: log the notification attempt so app still compiles and behaves predictably
//                System.Diagnostics.Debug.WriteLine($"NotificationService: Failed to send notification. Title='{title}', Message='{message}', Error='{ex.Message}'");
//            }
//        }
//    }
//}
