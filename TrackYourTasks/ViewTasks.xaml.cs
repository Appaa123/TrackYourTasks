using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using TrackYourTasks.Models;
using TrackYourTasks.Popups;
using TrackYourTasks.Services;
using Microsoft.Maui.Controls;

namespace TrackYourTasks
{
    public partial class ViewTasks : ContentPage
    {
        private readonly ApiService _api;

        // runtime spinner
        private ActivityIndicator _loadingIndicator;
        private int _loadingCount = 0;

        public ViewTasks(ApiService api)
        {
            InitializeComponent();
            _api = api;

            // Wrap existing content in a Grid and overlay an ActivityIndicator
            // Preserve the existing visual tree created in XAML
            var existingContent = this.Content;
            var rootGrid = new Grid();

            if (existingContent != null)
            {
                // Keep existing content as the base layer
                rootGrid.Children.Add(existingContent);
            }

            _loadingIndicator = new ActivityIndicator
            {
                IsVisible = false,
                IsRunning = false,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            // Overlay spinner on top of existing UI
            rootGrid.Children.Add(_loadingIndicator);

            this.Content = rootGrid;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTasks();
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

        // ✅ LOAD FROM API
        private async Task LoadTasks()
        {
            try
            {
                ShowLoading();

                var tasks = await _api.GetTasksAsync();
                TasksList.ItemsSource = tasks;
            }
            catch (Exception)
            {
                var toast = Toast.Make("Failed to load tasks.", ToastDuration.Short);
                await toast.Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // ✅ DELETE
        private async void OnDeleteTaskClicked(object sender, EventArgs e)
        {
            bool isConfirmed = await OnDeleteButtonClickedConfirmation();
            if (!isConfirmed) return;

            if (sender is SwipeItem swipeItem && swipeItem.BindingContext is TrackTask task)
            {
                try
                {
                    ShowLoading();

                    await _api.DeleteTaskAsync(task.Id);

                    await LoadTasks();

                    var toast = Toast.Make("Task deleted successfully!", ToastDuration.Short);
                    await toast.Show();
                }
                catch (Exception)
                {
                    var toast = Toast.Make("Failed to delete task.", ToastDuration.Short);
                    await toast.Show();
                }
                finally
                {
                    HideLoading();
                }
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

            try
            {
                ShowLoading();

                task.IsCompleted = true;

                await _api.UpdateTaskAsync(task); // 🔥 API call

                await LoadTasks();

                var toast = Toast.Make("Task marked as completed!", ToastDuration.Short);
                await toast.Show();
            }
            catch (Exception)
            {
                var toast = Toast.Make("Failed to mark task completed.", ToastDuration.Short);
                await toast.Show();
            }
            finally
            {
                HideLoading();
            }
        }
    }
}