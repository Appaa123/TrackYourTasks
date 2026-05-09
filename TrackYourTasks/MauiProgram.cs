using System.Reflection;
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

			// ✅ Load appsettings.json from EmbeddedResource
			var assembly = Assembly.GetExecutingAssembly();

			using var stream = assembly.GetManifestResourceStream("TrackYourTasks.appsettings.json");

			if (stream == null)
				throw new Exception("appsettings.json not found. Check namespace and file name!");

			var config = new ConfigurationBuilder()
				.AddJsonStream(stream)
				.Build();

			builder.Configuration.AddConfiguration(config);

			// ✅ Register ApiService with BaseUrl from config
			builder.Services.AddSingleton<ApiService>(sp =>
			{
				var configuration = sp.GetRequiredService<IConfiguration>();
				var baseUrl = configuration["ApiSettings:BaseUrl"];

				if (string.IsNullOrEmpty(baseUrl))
					throw new Exception("BaseUrl missing in appsettings.json");

				return new ApiService(baseUrl);
			});

			// ✅ Register Pages
			builder.Services.AddSingleton<MainPage>();
			builder.Services.AddTransient<CreateTasks>();
			builder.Services.AddTransient<ViewTasks>();
			builder.Services.AddTransient<PendingTasksPage>();
			builder.Services.AddTransient<AnalyticsPage>();
			builder.Services.AddTransient<DailyTrackerPage>();

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