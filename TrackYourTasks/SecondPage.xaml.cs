using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Models;

namespace TrackYourTasks
{
    public partial class SecondPage : ContentPage
    {
        private readonly AppDbContext _db;
        public SecondPage(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadTasks();
        }

        private void LoadTasks()
        {
            TasksList.ItemsSource = _db.Tasks.ToList();
        }
        private async void OnYesClicked(object? sender, EventArgs e)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync();

            var button = sender as Button;
            string taskName = button?.CommandParameter?.ToString();

            if (task == null)
            {
                // First time entry
                task = new TrackTask
                {
                    Title = taskName ?? "Unknown Task",
                    Id = Guid.NewGuid().Variant,
                    IsCompleted = true,
                    IsPartiallyCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    Description = "Tasks is completed by the user."

                };

                _db.Tasks.Add(task);
            }
            else
            {
                // Update existing row
                task.IsCompleted = true;
                task.Description = "User clicked YES";
            }

            await _db.SaveChangesAsync();
        }
        private async void OnNoClicked(object? sender, EventArgs e)
        {
            var task = await _db.Tasks.FirstOrDefaultAsync();

            var button = sender as Button;
            var taskName = button?.CommandParameter?.ToString();

            if (task == null)
                return;

            task.IsCompleted = false;
            task.Title = taskName ?? "Unknown Task";

            await _db.SaveChangesAsync();

            //Navigation.PushAsync(new TasksPage());
        }
    }
}
