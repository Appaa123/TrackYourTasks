using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Models;
using TrackYourTasks.Popups;

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
            LoadTasks(); // Refresh every time page appears
        }

        private void LoadTasks()
        {
            TasksList.ItemsSource = _db.Tasks.ToList();
        }

        // ✅ DELETE (Swipe)
        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await OnDeleteButtonClickedConfirmation();

            if (!isConfirmed) return;

            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TrackTask task)
            {
                _db.Tasks.Remove(task);
                _db.SaveChanges();
                LoadTasks();

                var toast = Toast.Make("Task deleted successfully!", ToastDuration.Short, 14);
                await toast.Show();
            }
        }

        // ✅ EDIT (used from popup/menu)
        private async Task EditTask(TrackTask task)
        {
            if (task == null) return;

            await Navigation.PushAsync(new CreateTasks(_db, task));
        }

        // ✅ BACK
        private async void OnBackTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        // ✅ CONFIRM DELETE POPUP
        private async Task<bool> OnDeleteButtonClickedConfirmation()
        {
            var popup = new ConfirmPopup("Are you sure?");
            return await popup.ShowAsync(this);
        }

        // ✅ THREE DOT MENU (MAIN ACTION HANDLER)
        private async void OnMoreClicked(object sender, EventArgs e)
        {
            var task = (sender as BindableObject)?.BindingContext as TrackTask;

            if (task == null) return;

            string action = await DisplayActionSheet(
                "Task Options",
                "Cancel ❌",
                null,
                "✏️ Edit",
                "✅ Mark as Completed"
            );

            switch (action)
            {
                case "✏️ Edit":
                    await EditTask(task);
                    break;

                case "✅ Mark as Completed":
                    await MarkTaskCompleted(task);
                    break;
            }
        }

        // ✅ MARK COMPLETE (centralized logic)
        private async Task MarkTaskCompleted(TrackTask task)
        {
            if (task == null || task.IsCompleted) return;

            task.IsCompleted = true;

            _db.SaveChanges();   // 🔥 Persist to DB
            LoadTasks();         // 🔥 Refresh UI

            var toast = Toast.Make("Task marked as completed!", ToastDuration.Short, 14);
            await toast.Show();
        }
    }
}