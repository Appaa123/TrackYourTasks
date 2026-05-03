using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using TrackYourTasks.Models;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class CreateTasks : ContentPage
    {
        private readonly ApiService _api;
        private TrackTask _taskBeingEdited;

        // ✅ CREATE
        public CreateTasks(ApiService api)
        {
            InitializeComponent();
            _api = api; // 🔥 FIX
        }

        // ✅ EDIT
        public CreateTasks(ApiService api, TrackTask taskToEdit) : this(api)
        {
            if (taskToEdit == null) return;

            _taskBeingEdited = taskToEdit;

            TitleEntry.Text = taskToEdit.Title;
            DescEntry.Text = taskToEdit.Description;

            AddTaskButton.Text = "Update Task";
        }

        // ✅ CANCEL
        private async void OnCancelTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // 🔥 clean navigation
        }

        // ✅ SAVE (CREATE / UPDATE)
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleEntry.Text))
            {
                await DisplayAlert("Validation", "Title is required", "OK");
                return;
            }

            var notifyTime = TaskDatePicker.Date + TaskTimePicker.Time;

            string message = "TYT - Reminder - ";

            TrackTask task;

            if (_taskBeingEdited == null)
            {
                // 🔥 CREATE
                task = new TrackTask
                {
                    Title = TitleEntry.Text ?? string.Empty,
                    Description = DescEntry.Text ?? string.Empty,
                    IsCompleted = false,
                    IsSkipped = false,
                    IsPartiallyCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _api.CreateTaskAsync(task);

                await Toast.Make("Task created!", ToastDuration.Short).Show();
            }
            else
            {
                // 🔥 UPDATE
                _taskBeingEdited.Title = TitleEntry.Text ?? string.Empty;
                _taskBeingEdited.Description = DescEntry.Text ?? string.Empty;

                await _api.UpdateTaskAsync(_taskBeingEdited);

                task = _taskBeingEdited;

                await Toast.Make("Task updated!", ToastDuration.Short).Show();
            }

            // 🔥 Notification fix (string → int)
            int notificationId = task.Id?.GetHashCode() ?? new Random().Next();

            var request = new NotificationRequest
            {
                NotificationId = notificationId,
                Title = task.Title,
                Description = message + task.Description,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = notifyTime
                },
                ReturningData = "PendingTasksPage"
            };

            await LocalNotificationCenter.Current.Show(request);

            await Navigation.PopAsync();
        }
    }
}