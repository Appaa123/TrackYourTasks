using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddDbContext<AppDbContext>();
            builder.UseMauiApp<App>()
                .UseMauiCommunityToolkit();
            builder.Services.AddSingleton<INotificationService, NotificationService>();

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
