using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Services;
#if ANDROID
using TrackYourTasks.Platforms.Android;
#endif

namespace TrackYourTasks
{
    public partial class MainPage : ContentPage
    {
        private INotificationService _notificationService;
        int count = 0;

        public MainPage(INotificationService notificationService)
        {
            InitializeComponent();
            RequestNotificationPermission();
            _notificationService = notificationService;
        }
        private async void OnTaskButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                //var now = DateTime.Now.TimeOfDay;

                //if (now.Hours == 9)
                //    SendPlatformNotification("9AM");
                //else if (now.Hours == 13)
                //    SendPlatformNotification("1PM");
                //else if (now.Hours == 21)
                //    SendPlatformNotification("9PM");

                //await _notificationService.ShowNotification("Alert", "This is a MAUI notification!");
                Console.WriteLine("Triggering the notifications");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to show notification: {ex.Message}", "OK");
            }
            try
            {
                await Navigation.PushAsync(new TasksPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to navigate to TasksPage: {ex.Message}", "OK");
            }
        }
        private async void RequestNotificationPermission()
        {
#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33)) // Android 13+
        {
            var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                Console.WriteLine($"Notification permission status: {status}");
            }
        }
#endif
        }
        public void SendPlatformNotification(string timeLabel)
        {
            Console.WriteLine("Triggering the notifications Started");
#if ANDROID
        var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;
        new TrackYourTasks.Platforms.Android.Services.NotificationHelper(context)
            .ShowInteractiveNotification("Confirm Action", "Do you wish to proceed?", timeLabel);
#endif
            Console.WriteLine("Triggering the notifications Started");
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
