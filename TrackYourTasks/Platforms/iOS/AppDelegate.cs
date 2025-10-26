using Foundation;
using UIKit;
using UserNotifications;

namespace TrackYourTasks
{
    //All this code for IOS
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        //For IOS
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // Request notification permissions
            var authOptions = UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound;
            UNUserNotificationCenter.Current.RequestAuthorization(authOptions, (granted, error) =>
            {
                Console.WriteLine($"Permission granted: {granted}");
            });

            // Register delegate for interaction handling
            UNUserNotificationCenter.Current.Delegate = new Platforms.iOS.NotificationReceiver();

            // Register categories here
            RegisterCategories();

            return base.FinishedLaunching(app, options);
        }

        void RegisterCategories()
        {
            var yesAction = UNNotificationAction.FromIdentifier("action_yes", "Yes", UNNotificationActionOptions.Foreground);
            var noAction = UNNotificationAction.FromIdentifier("action_no", "No", UNNotificationActionOptions.Destructive);

            var category = UNNotificationCategory.FromIdentifier(
                "confirmCategory",
                new UNNotificationAction[] { yesAction, noAction },
                new string[] { }, UNNotificationCategoryOptions.None);

            UNUserNotificationCenter.Current.SetNotificationCategories(
                new NSSet<UNNotificationCategory>(category));
        }
    }
}
