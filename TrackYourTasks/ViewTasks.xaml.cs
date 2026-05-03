using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using TrackYourTasks.Models;
using TrackYourTasks.Popups;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class ViewTasks : ContentPage
    {
        private readonly ApiService _api;

        public ViewTasks(ApiService api)
        {
            InitializeComponent();
            _api = api;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
        }

        // ✅ LOAD FROM API
        private async Task LoadTasks()
        {
            var tasks = await _api.GetTasksAsync();
            TasksList.ItemsSource = tasks;
        }

        // ✅ DELETE
        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await OnDeleteButtonClickedConfirmation();
            if (!isConfirmed) return;

            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TrackTask task)
            {
                await _api.DeleteTaskAsync(task.Id);

                await LoadTasks();

                var toast = Toast.Make("Task deleted successfully!", ToastDuration.Short);
                await toast.Show();
            }
        }

        // ✅ EDIT
        private async Task EditTask(TrackTask task)
        {
            if (task == null) return;

            await Navigation.PushAsync(new CreateTasks(_api, task));
        }

        // ✅ BACK
        private async void OnBackTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(); // 🔥 better than PushAsync
        }

        // ✅ CONFIRM POPUP
        private async Task<bool> OnDeleteButtonClickedConfirmation()
        {
            var popup = new ConfirmPopup("Are you sure?");
            return await popup.ShowAsync(this);
        }

        // ✅ 3-DOT MENU
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

        // ✅ MARK COMPLETE
        private async Task MarkTaskCompleted(TrackTask task)
        {
            if (task == null || task.IsCompleted) return;

            task.IsCompleted = true;

            await _api.UpdateTaskAsync(task); // 🔥 API call

            await LoadTasks();

            var toast = Toast.Make("Task marked as completed!", ToastDuration.Short);
            await toast.Show();
        }
    }
}