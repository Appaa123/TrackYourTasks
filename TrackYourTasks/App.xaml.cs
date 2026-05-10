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

            // Show spinner while checking auth
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
                var authService = _services.GetRequiredService<GoogleAuthService>();
                var api = _services.GetRequiredService<ApiService>();

                // Step 1: Check if user is already signed in
                bool isSignedIn = await authService.IsSignedInAsync();

                if (!isSignedIn)
                {
                    // Not signed in → show Login page
                    MainPage = new NavigationPage(new LoginPage(authService, api));
                    return;
                }

                // Step 2: User is signed in → go to the right page
                await NavigateToMainApp(api);
            }
            catch
            {
                // On any error → fall back to Login page
                var authService = _services.GetRequiredService<GoogleAuthService>();
                var api = _services.GetRequiredService<ApiService>();
                MainPage = new NavigationPage(new LoginPage(authService, api));
            }
        }

        // Call this after successful login from LoginPage
        public async Task NavigateToMainApp(ApiService api)
        {
            try
            {
                var tasks = await api.GetTasksAsync();
                bool isAnyPendingTasks = tasks.Any(t => !t.IsCompleted);

                if (isAnyPendingTasks)
                    MainPage = new NavigationPage(new PendingTasksPage(api));
                else
                    MainPage = new NavigationPage(new MainPage(api));
            }
            catch
            {
                MainPage = new NavigationPage(new MainPage(api));
            }
        }
    }
}
