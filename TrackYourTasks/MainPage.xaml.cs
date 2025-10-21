using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;

namespace TrackYourTasks
{
    public partial class MainPage : ContentPage
    {
        private INotificationService _notificationService;
        int count = 0;

        public MainPage(INotificationService notificationService)
        {
            InitializeComponent();
            _notificationService = notificationService;
        }
        private async void OnTaskButtonClicked(object? sender, EventArgs e)
        {
            await _notificationService.ShowNotification("Alert", "This is a MAUI notification!");
            await Navigation.PushAsync(new TasksPage());
        }
        private async void OnCreateTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateTasks(new AppDbContext()));
        }
        private async void OnViewTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewTasks(new AppDbContext()));
        }
    }
}
