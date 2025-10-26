using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Interfaces;
using INotificationService = TrackYourTasks.Interfaces.INotificationService;

namespace TrackYourTasks.Services
{
    public class NotificationService : INotificationService
    {
        public async Task ShowNotification(string title, string message)
        {
            var toast = Toast.Make($"{title}: {message}", ToastDuration.Short);
            await toast.Show();
        }

        public async Task ShowNotificationWithActions()
        {
            var request = new NotificationRequest
            {
                NotificationId = 1001,
                Title = "Confirm Action",
                Description = "Do you want to proceed?",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(2)
                },
                Android = new AndroidOptions
                { 
                    LaunchAppWhenTapped = true,
                   Ongoing = true               
                 
                },
                
            };

            await LocalNotificationCenter.Current.Show(request);
        }


    }
}
