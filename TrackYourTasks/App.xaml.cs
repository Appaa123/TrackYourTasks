using TrackYourTasks.Data;

namespace TrackYourTasks
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            //MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}