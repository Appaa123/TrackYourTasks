using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
        private INotificationService _notificationService;

        // Constructor for "create new task"
        public CreateTasks(AppDbContext db,INotificationService notificationService)
        {
            InitializeComponent();
            _db = db;
            _notificationService = notificationService;
        }

        // Constructor for "edit existing task"
        public CreateTasks(AppDbContext db,INotificationService notificationService, TrackTask taskToEdit) : this(db, notificationService)
        {
            _taskBeingEdited = taskToEdit;

            // Pre-fill UI fields
            TitleEntry.Text = taskToEdit.Title;
            DescEntry.Text = taskToEdit.Description;

            AddTaskButton.Text = "Update Task";
        }

        private async void OnCancelTaskClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new MainPage(_notificationService));

        }
        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            if (_taskBeingEdited == null)
            {
                var task = new TrackTask
                {
                    Title = TitleEntry.Text,
                    Description = DescEntry.Text,
                    IsCompleted = false
                };

                _db.Tasks.Add(task);
                await _db.SaveChangesAsync();

                await Toast.Make("Task created!", ToastDuration.Short).Show();
            }
            else
            {
                _taskBeingEdited.Title = TitleEntry.Text;
                _taskBeingEdited.Description = DescEntry.Text;

                _db.Tasks.Update(_taskBeingEdited);
                await _db.SaveChangesAsync();

                await Toast.Make("Task updated!", ToastDuration.Short).Show();
            }

            await Navigation.PopAsync(); // go back to ViewTasks
        }
    }

}
