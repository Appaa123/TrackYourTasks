using TrackYourTasks.Data;
using TrackYourTasks.Interfaces; // Add this using directive

namespace TrackYourTasks
{
    public partial class App : Application
    {
        private readonly INotificationService _notificationService; // Use the interface from the correct namespace
        public App(INotificationService notificationService)
        {
            InitializeComponent();

            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            Services.NotificationScheduler.ScheduleDailyNotifications();
            _notificationService = notificationService;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new MainPage(_notificationService)));
        }
    }
}