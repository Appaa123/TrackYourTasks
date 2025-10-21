using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;

namespace TrackYourTasks
{
    public partial class App : Application
    {
        private readonly INotificationService _notificationService;
        public App(INotificationService notificationService)
        {
            InitializeComponent(); 

            using var db = new AppDbContext();
            db.Database.EnsureCreated();
            _notificationService = notificationService;
            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new MainPage(_notificationService)));
        }
    }
}