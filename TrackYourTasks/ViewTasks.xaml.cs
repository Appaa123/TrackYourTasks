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
using TrackYourTasks.Popups;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class ViewTasks : ContentPage
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notificationService;
        public ViewTasks(AppDbContext db, INotificationService notificationService)
        {
            InitializeComponent();
            _db = db;
            _notificationService = notificationService;
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
            bool isConfirmed = await OnDeleteButtonClickedConfirmation(sender, e);

            if (isConfirmed)
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
        }
        private async void OnEditTaskClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is TrackTask task)
            {
                await Navigation.PushAsync(new CreateTasks(_db, _notificationService, task));
            }
        }
        private async void OnBackTaskClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new MainPage(_notificationService));

        }

        private async Task<bool> OnDeleteButtonClickedConfirmation(object sender, EventArgs e)
        {
            var popup = new ConfirmPopup();
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


    }
}
