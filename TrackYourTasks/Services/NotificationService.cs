using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Interfaces;

namespace TrackYourTasks.Services
{
    public class NotificationService: INotificationService
    {
        public async Task ShowNotification(string title, string message)
        {
            var toast = Toast.Make($"{title}: {message}", ToastDuration.Short);
            await toast.Show();
        }


    }
}
