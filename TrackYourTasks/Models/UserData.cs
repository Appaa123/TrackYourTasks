using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Models
{
    public class UserData
    {
        public int Id { get; set; } // Primary Key
        public string TaskName { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string UserLabel { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public bool UserClickedYes { get; set; }
    }
}
