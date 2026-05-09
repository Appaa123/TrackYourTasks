using Plugin.LocalNotification;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
	public partial class MainPage : ContentPage
	{
		private readonly ApiService _api;

		public MainPage(ApiService api)
		{
			InitializeComponent();
			_api = api;

			RequestNotificationPermission();
		}

		// 📊 Analytics Page
		private async void OnTaskButtonClicked(object? sender, EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new AnalyticsPage(_api));
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", $"Failed to navigate: {ex.Message}", "OK");
			}
		}

		// ➕ Create Task
		private async void OnCreateTaskButtonClicked(object? sender, EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new CreateTasks(_api));
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", $"Failed to navigate: {ex.Message}", "OK");
			}
		}

		// 📋 View Tasks
		private async void OnViewTaskButtonClicked(object? sender, EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new ViewTasks(_api));
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", $"Failed to navigate: {ex.Message}", "OK");
			}
		}

		// ➕ Daily tracker
		private async void OnDailyTrackerClicked(object? sender, EventArgs e)
		{
			try
			{
				await Navigation.PushAsync(new DailyTrackerPage(_api));
			}
			catch (Exception ex)
			{
				await DisplayAlert("Error", $"Failed to navigate: {ex.Message}", "OK");
			}
		}

		// 🔔 Notification permission
		private async void RequestNotificationPermission()
		{
#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.PostNotifications>();
                }
            }
#endif
		}

		// 🔔 Platform notification
		public void SendPlatformNotification(string timeLabel)
		{
#if ANDROID
            var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity;

            new TrackYourTasks.Platforms.Android.Services.NotificationHelper(context)
                .ShowInteractiveNotification(
                    "Confirm Action",
                    "Do you wish to proceed?",
                    timeLabel);
#endif
		}
	}
}