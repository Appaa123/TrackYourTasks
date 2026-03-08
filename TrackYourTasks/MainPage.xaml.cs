using Plugin.LocalNotification;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Services;
using Microsoft.Extensions.DependencyInjection; // used for IServiceProvider extensions
using INotificationService = TrackYourTasks.Interfaces.INotificationService;
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
                Console.WriteLine("Triggering the notifications");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to show notification: {ex.Message}", "OK");
            }
            try
            {
                // Prefer Navigation.PushAsync when not using Shell.Current
                var page = new SecondPage(new AppDbContext(), new NotificationService());

                if (Navigation != null)
                    await Navigation.PushAsync(page);
                else if (Application.Current?.MainPage?.Navigation != null)
                    await Application.Current.MainPage.Navigation.PushAsync(page);
                else
                    await DisplayAlert("Error", "Navigation is not available", "OK");

                Console.WriteLine("View Task Button Clicked");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to navigate to SecondPage: {ex.Message}", "OK");
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
            try
            {
                // Shell.Current will be null because the app uses a NavigationPage (see App.CreateWindow).
                // Use Navigation.PushAsync and resolve the page from DI if available; otherwise fall back to constructing it.
                var page = new CreateTasks(new AppDbContext(), new NotificationService());

                if (Navigation != null)
                    await Navigation.PushAsync(page);
                else if (Application.Current?.MainPage?.Navigation != null)
                    await Application.Current.MainPage.Navigation.PushAsync(page);
                else
                    await DisplayAlert("Error", "Navigation is not available", "OK");

                Console.WriteLine("Create Task Button Clicked");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to navigate to CreateTasks: {ex.Message}", "OK");
            }
        }
        private async void OnViewTaskButtonClicked(object? sender, EventArgs e)
        {
            try
            {
                // Prefer Navigation.PushAsync when not using Shell.Current
                var page = new ViewTasks(new AppDbContext(), new NotificationService());

                if (Navigation != null)
                    await Navigation.PushAsync(page);
                else if (Application.Current?.MainPage?.Navigation != null)
                    await Application.Current.MainPage.Navigation.PushAsync(page);
                else
                    await DisplayAlert("Error", "Navigation is not available", "OK");

                Console.WriteLine("View Task Button Clicked");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to navigate to ViewTasks: {ex.Message}", "OK");
            }
        }
    }
}