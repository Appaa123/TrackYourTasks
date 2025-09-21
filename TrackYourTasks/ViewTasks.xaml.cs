using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Models;

namespace TrackYourTasks
{
    public partial class ViewTasks : ContentPage
    {
        private readonly AppDbContext _db;

        public ViewTasks(AppDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadTasks();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasks();  // refresh list every time page becomes visible
        }

        private void LoadTasks()
        {
            TasksList.ItemsSource = _db.Tasks.ToList();
        }

        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is TrackTask task)
            {
                _db.Tasks.Remove(task);
                _db.SaveChanges();
                LoadTasks();
                // ✅ Show success toast
                var toast = Toast.Make("Task deleted successfully!", ToastDuration.Short, 14);
                await toast.Show();
            }
        }
        private async void OnEditTaskClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is TrackTask task)
            {
                await Navigation.PushAsync(new CreateTasks(_db, task));
            }
        }
        private async void OnBackTaskClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new MainPage());

        }


    }
}
