using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();

			builder
				.UseMauiApp<App>()
				.UseLocalNotification()
				.UseMauiCommunityToolkit()
				.UseMicrocharts()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
					fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				});

			// ✅ Load config file
			builder.Configuration.AddJsonFile("appsettings.json", optional: false);

			// ✅ Register ApiService with BaseUrl
			builder.Services.AddSingleton<ApiService>(sp =>
			{
				var config = sp.GetRequiredService<IConfiguration>();
				var baseUrl = config["ApiSettings:BaseUrl"];

				return new ApiService(baseUrl);
			});

			// ✅ Register Pages
			builder.Services.AddTransient<CreateTasks>();
			builder.Services.AddTransient<PendingTasksPage>();
			builder.Services.AddTransient<ViewTasks>();
			builder.Services.AddTransient<AnalyticsPage>(); // 🔥 you missed this earlier
			builder.Services.AddSingleton<MainPage>();

#if DEBUG
			builder.Logging.AddDebug();
#endif

			var app = builder.Build();

			// 🔔 iOS Notification Permission
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				if (DeviceInfo.Platform == DevicePlatform.iOS)
				{
					await LocalNotificationCenter.Current.RequestNotificationPermission();
				}
			});

			// 🔔 Notification Click Navigation
			LocalNotificationCenter.Current.NotificationActionTapped += async (e) =>
			{
				if (e.Request.ReturningData == "PendingTasksPage")
				{
					await MainThread.InvokeOnMainThreadAsync(async () =>
					{
						var navigation = Application.Current?.MainPage?.Navigation;

						if (navigation != null)
						{
							var api = app.Services.GetRequiredService<ApiService>();

							await navigation.PushAsync(new PendingTasksPage(api));
						}
					});
				}
			};

			return app;
		}
	}
}