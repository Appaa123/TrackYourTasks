using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Popups;
#if ANDROID
using Android.Content;
#endif

namespace TrackYourTasks
{
    public partial class TasksPage : ContentPage
    {
        public ObservableCollection<Models.TrackTask> Tasks { get; set; }

        public TasksPage()
        {
            InitializeComponent();
            bool isStatusChecked = checkTaskStatus().Result;
            var userClickedYes = GetUserClickedYes();
            var userNotificationLabel = GetUserNotificationLabel();

            var userCount = GetUserCount();
            var userLabel = GetUserNotificationLabel();

            Tasks = new ObservableCollection<Models.TrackTask>
        {
            new Models.TrackTask
            {
                IsCompleted = userClickedYes,
                Title = userNotificationLabel
            }
        };

            BindingContext = this;

            // Use the retrieved boolean value as needed in your page
            Console.WriteLine($"User clicked YES?: {userClickedYes}");
            Console.WriteLine($"userNotificationLabel: {userNotificationLabel}");
        }

        private async Task<bool> checkTaskStatus()
        {
            var popup = new ConfirmPopup("Did you complete the task?");
            bool result = await popup.ShowAsync(this); // ✅ Custom ShowAsync to get result

            if (result)
            {
                return true;
                //await DisplayAlert("Confirmed", "Action executed.", "OK");
                // Proceed with the confirmed action
            }
            else
            {
                return false;
                //await DisplayAlert("Cancelled", "Action was cancelled.", "OK");
            }
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
