using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Interfaces
{
    public interface INotificationManagerService
    {
        void SendNotification(string title, string message);
    }
}
