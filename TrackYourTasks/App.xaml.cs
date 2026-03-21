using TrackYourTasks.Data;
using TrackYourTasks.Interfaces; // Add this using directive

namespace TrackYourTasks
{
    public partial class App : Application
    {
        private readonly INotificationService _notificationService; // Use the interface from the correct namespace
        
        private bool isAnyPendingTasks = false;

        private AppDbContext _db;
        public App(INotificationService notificationService)
        {
            InitializeComponent();
            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            _db = db;
            Services.NotificationScheduler.ScheduleDailyNotifications();
            _notificationService = notificationService;
            isAnyPendingTasks = db.Tasks.Any(task => !task.IsCompleted);
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            if (isAnyPendingTasks)
            {
                return new Window(new NavigationPage(new PendingTasksPage(_db)));
            }
            else
            {
                return new Window(new NavigationPage(new MainPage(_notificationService)));
            }
        }
    }
}