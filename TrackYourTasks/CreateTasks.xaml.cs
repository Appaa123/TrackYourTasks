using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using TrackYourTasks.Models;
using TrackYourTasks.Services;
using Microsoft.Maui.Controls;
using System;

#if ANDROID
using Android.App;
using TrackYourTasks.Platforms.Android.Services;
#endif

namespace TrackYourTasks
{
    public partial class CreateTasks : ContentPage
    {
        private readonly ApiService _api;
        private TrackTask _taskBeingEdited;

        // runtime spinner
        private ActivityIndicator _loadingIndicator;
        private int _loadingCount = 0;

        // ✅ CREATE
        public CreateTasks(ApiService api)
        {
            InitializeComponent();
            _api = api; // 🔥 FIX

            // Preserve existing visual tree and overlay spinner
            var existingContent = this.Content;
            var rootGrid = new Grid();

            if (existingContent != null)
            {
                rootGrid.Children.Add(existingContent);
            }

            _loadingIndicator = new ActivityIndicator
            {
                IsVisible = false,
                IsRunning = false,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            rootGrid.Children.Add(_loadingIndicator);
            this.Content = rootGrid;
        }

        // helper to support nested API calls without flicker
        private void ShowLoading()
        {
            _loadingCount++;
            if (_loadingCount > 0)
            {
                _loadingIndicator.IsVisible = true;
                _loadingIndicator.IsRunning = true;
            }
        }

        private void HideLoading()
        {
            _loadingCount = Math.Max(0, _loadingCount - 1);
            if (_loadingCount == 0)
            {
                _loadingIndicator.IsRunning = false;
                _loadingIndicator.IsVisible = false;
            }
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

            try
            {
                ShowLoading();

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
                int notificationId = (task ?? _taskBeingEdited)?.Id?.GetHashCode() ?? new Random().Next();

                var request = new NotificationRequest
                {
                    NotificationId = notificationId,
                    Title = (task ?? _taskBeingEdited)?.Title,
                    Description = message + ((task ?? _taskBeingEdited)?.Description ?? string.Empty),
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    },
                    ReturningData = "PendingTasksPage"
                };

                // Keep plugin notification for in-process behavior
                await LocalNotificationCenter.Current.Show(request);

#if ANDROID
                // Ensure notifyTime is local and schedule an AlarmManager alarm so the reminder fires even if the app is killed
                var localNotify = DateTime.SpecifyKind(notifyTime, DateTimeKind.Local);
                var label = request.Title ?? "TaskReminder";

                global::TrackYourTasks.Platforms.Android.Services.AlarmScheduler.ScheduleAlarm(
                    global::Android.App.Application.Context,
                    localNotify,
                    label,
                    notificationId
                );
#endif

                await Navigation.PopAsync();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to save task.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }
    }
}