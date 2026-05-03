using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Plugin.LocalNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Models;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class CreateTasks : ContentPage
    {
        private readonly AppDbContext _db;
        private TrackTask _taskBeingEdited;

        // Constructor for "create new task"
        public CreateTasks(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
        }

        // Constructor for "edit existing task"
        public CreateTasks(AppDbContext db, TrackTask taskToEdit) : this(db)
        {
            _taskBeingEdited = taskToEdit;

            // Pre-fill UI fields
            TitleEntry.Text = taskToEdit.Title;
            DescEntry.Text = taskToEdit.Description;

            AddTaskButton.Text = "Update Task";
        }

        private async void OnCancelTaskClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new MainPage());

        }
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            var request = new NotificationRequest();

            var selectedDate = TaskDatePicker.Date;       // DateTime (date part)
            var selectedTime = TaskTimePicker.Time;       // TimeSpan (time part)

            // Combine into a full DateTime
            var notifyTime = selectedDate.Date + selectedTime;

            string message = "TYT - Reminder - ";

            if (_taskBeingEdited == null)
            {
                var task = new TrackTask
                {
                    Title = TitleEntry.Text ?? string.Empty,
                    Description = DescEntry.Text ?? string.Empty,
                    IsCompleted = false
                };

                _db.Tasks.Add(task);
                await _db.SaveChangesAsync();

                request = new NotificationRequest
                {
                    NotificationId = task.Id,
                    Title = task.Title,
                    Description = message + task.Description,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime // or any specific DateTime
                    },
                    ReturningData = "PendingTasksPage"
                };

                await Toast.Make("Task created!", ToastDuration.Short).Show();
            }
            else
            {
                _taskBeingEdited.Title = TitleEntry.Text ?? string.Empty;
                _taskBeingEdited.Description = DescEntry.Text ?? string.Empty;

                _db.Tasks.Update(_taskBeingEdited);
                await _db.SaveChangesAsync();

                request = new NotificationRequest
                {
                    NotificationId = _taskBeingEdited.Id,
                    Title = _taskBeingEdited.Title,
                    Description = message + _taskBeingEdited.Description,
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = _taskBeingEdited.CreatedAt // or any specific DateTime
                    },
                    ReturningData = "PendingTasksPage"
                };

                await Toast.Make("Task updated!", ToastDuration.Short).Show();
            }

           

            await LocalNotificationCenter.Current.Show(request);

            await Navigation.PopAsync(); // go back to ViewTasks
        }
    }

}
