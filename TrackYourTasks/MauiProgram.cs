using CommunityToolkit.Maui;
using Microcharts.Maui;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;
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
                .UseLocalNotification()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            //builder.Services.AddSingleton<INotificationService, NotificationService>();
            builder.Services.AddDbContext<AppDbContext>();
            builder.Services.AddTransient<CreateTasks>();
            builder.Services.AddSingleton<MainPage>();
            builder.UseMicrocharts();

            using (var scope = builder.Services.BuildServiceProvider().CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();
            }

#if DEBUG
            builder.Logging.AddDebug();
#endif


            LocalNotificationCenter.Current.NotificationActionTapped += async (e) =>
            {
                if (e.Request.ReturningData == "PendingTasksPage")
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var navigation = Application.Current?.MainPage?.Navigation;
                        if (navigation != null)
                        {
                            await navigation.PushAsync(new PendingTasksPage(new AppDbContext()));
                        }
                    });
                }
            };

            return builder.Build();
        }
    }
}
