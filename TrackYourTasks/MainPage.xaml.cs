using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Services;

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
            try
            {
                await _notificationService.ShowNotification("Alert", "This is a MAUI notification!");
                
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", $"Failed to show notification: {ex.Message}", "OK");
            }
            try
            {
                await Navigation.PushAsync(new TasksPage());
            }
            catch(Exception ex)
            {
                await DisplayAlert("Error", $"Failed to navigate to TasksPage: {ex.Message}", "OK");
            }
        }
        private async void OnCreateTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateTasks(new AppDbContext(), new NotificationService()));
        }
        private async void OnViewTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewTasks(new AppDbContext(), new NotificationService()));
        }
    }
}
