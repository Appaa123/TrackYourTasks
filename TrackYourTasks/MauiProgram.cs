using CommunityToolkit.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Services;
using INotificationService = TrackYourTasks.Interfaces.INotificationService;

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
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddTransient<CreateTasks>();
            builder.Services.AddSingleton<MainPage>();

            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
