using TrackYourTasks.Services;

namespace TrackYourTasks
{
	public partial class App : Application
	{
		private readonly IServiceProvider _services;

		public App(IServiceProvider services)
		{
			InitializeComponent();

			_services = services;

			// Temporary loading page (better UX)
			MainPage = new ContentPage
			{
				Content = new ActivityIndicator
				{
					IsRunning = true,
					VerticalOptions = LayoutOptions.Center,
					HorizontalOptions = LayoutOptions.Center
				}
			};

			InitializeApp();

			Services.NotificationScheduler.ScheduleDailyNotifications();
		}

		private async void InitializeApp()
		{
			try
			{
				var api = _services.GetRequiredService<ApiService>();

				var tasks = await api.GetTasksAsync();
				bool isAnyPendingTasks = tasks.Any(t => !t.IsCompleted);

				if (isAnyPendingTasks)
				{
					MainPage = new NavigationPage(new PendingTasksPage(api));
				}
				else
				{
					MainPage = new NavigationPage(new MainPage(api));
				}
			}
			catch
			{
				var api = _services.GetRequiredService<ApiService>();

				MainPage = new NavigationPage(new MainPage(api));
			}
		}
	}
}