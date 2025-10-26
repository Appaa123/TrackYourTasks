using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Models
{
    public class UserData
    {
        public string userLabel { get; set; } = string.Empty;
        public int userCount { get; set; }
        public bool UserClickedYes { get; set; }
    }
}
