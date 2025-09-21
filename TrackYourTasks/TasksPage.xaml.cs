using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks
{
    public partial class TasksPage : ContentPage
    {
        public TasksPage()
        {
            InitializeComponent();
        }

        private void OnYesClicked(object? sender1, EventArgs e)
        {
            Navigation.PushAsync(new SecondPage());
        }
        private void OnNoClicked(object? sender2, EventArgs e)
        {
            Navigation.PushAsync(new SecondPage());
        }
    }
}
