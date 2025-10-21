using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Interfaces
{
    public interface INotificationService
    {
        Task ShowNotification(string title, string message);

    }
}
