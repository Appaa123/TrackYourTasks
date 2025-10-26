using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
// Android-specific code here
#endif

namespace TrackYourTasks
{
    public partial class TasksPage : ContentPage
    {
        public TasksPage()
        {
            InitializeComponent();

            var userClickedYes = GetUserClickedYes();
            var userNotificationLabel = GetUserNotificationLabel();

            // Use the retrieved boolean value as needed in your page
            Console.WriteLine($"User clicked YES?: {userClickedYes}");
        }

        private bool GetUserClickedYes()
        {
#if ANDROID
        var context = Android.App.Application.Context;
        var prefs = context.GetSharedPreferences("GlobalData", FileCreationMode.Private);
        
        int excerciseCount = 0;
        bool userSelectedValue = prefs.GetBoolean("UserClickedYes", false);
        string displayValue = userSelectedValue ? "Excercise Done" : "Excercise Not Done";
        excerciseCount = displayValue == "Excercise Done" ? excerciseCount++ : excerciseCount--;
        return userSelectedValue;
#else
            // Provide fallback for non-Android platforms if needed
            return false;
#endif
        }
        private int GetUserCount()
        {
#if ANDROID
        var context = Android.App.Application.Context;
        var prefs = context.GetSharedPreferences("GlobalData", FileCreationMode.Private);
        
        int excerciseCount = 0;
        bool userSelectedValue = prefs.GetBoolean("UserClickedYes", false);
        string displayValue = userSelectedValue ? "Excercise Done" : "Excercise Not Done";
        excerciseCount = displayValue == "Excercise Done" ? excerciseCount++ : excerciseCount--;
        return excerciseCount;
#else
            // Provide fallback for non-Android platforms if needed
            return 0;
#endif
        }

        private string GetUserNotificationLabel()
        {
#if ANDROID
            var context = Android.App.Application.Context;
        var prefs = context.GetSharedPreferences("GlobalData", FileCreationMode.Private);
            var selectedLabel = prefs.GetString("UserNotificationLabel", "No Label");
            return selectedLabel ?? "No Label";
#else
            // Provide fallback for non-Android platforms if needed
            return "None";
#endif

        }

        //private void OnYesClicked(object? sender1, EventArgs e)
        //{
        //    Navigation.PushAsync(new SecondPage());
        //}
        //private void OnNoClicked(object? sender2, EventArgs e)
        //{
        //    Navigation.PushAsync(new SecondPage());
        //}
    }
}
