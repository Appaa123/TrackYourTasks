using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls;
using TrackYourTasks.Models;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class DailyTrackerPage : ContentPage
    {
        private readonly ApiService _api;
        private int _loadingCount = 0;

        public ObservableCollection<DailyTask> DailyTasks { get; } = new();

        public DailyTrackerPage(ApiService api)
        {
            InitializeComponent();
            _api = api;

            TasksCollection.ItemsSource = DailyTasks;

            AddButton.Clicked += async (_, __) => await OnAddClicked();
            DeleteSelectedButton.Clicked += async (_, __) => await OnDeleteSelectedClicked();
            CompleteSelectedButton.Clicked += async (_, __) => await OnCompleteSelectedClicked();

            DeleteSelectedButton.IsEnabled = false;
            CompleteSelectedButton.IsEnabled = false;
        }

        // ── Lifecycle ────────────────────────────────────────────────────────────

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDailyTasks();
        }

        // ── Loading helpers ───────────────────────────────────────────────────────

        private void ShowLoading()
        {
            _loadingCount++;
            if (_loadingCount > 0)
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
            }
        }

        private void HideLoading()
        {
            _loadingCount = Math.Max(0, _loadingCount - 1);
            if (_loadingCount == 0)
            {
                LoadingIndicator.IsRunning = false;
                LoadingIndicator.IsVisible = false;
            }
        }

        // ── Data ─────────────────────────────────────────────────────────────────

        private async Task LoadDailyTasks()
        {
            try
            {
                ShowLoading();
                var tasks = await _api.GetDailyTasksAsync();

                DailyTasks.Clear();
                foreach (var t in tasks)
                    DailyTasks.Add(t);

                EmptyLabel.IsVisible = !DailyTasks.Any();
                UpdateBulkActionButtons();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to load daily tasks.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // ── Add ───────────────────────────────────────────────────────────────────

        private async Task OnAddClicked()
        {
            var title = await DisplayPromptAsync(
                "New Daily Task", "Title",
                initialValue: string.Empty, maxLength: 20);

            if (string.IsNullOrWhiteSpace(title)) return;

            var description = await DisplayPromptAsync(
                "Description", "Description",
                initialValue: string.Empty, maxLength: 200);

            var newTask = new DailyTask
            {
                Title = title,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            try
            {
                ShowLoading();
                var created = await _api.CreateDailyTaskAsync(newTask);

                if (created != null && !string.IsNullOrWhiteSpace(created.Id))
                    DailyTasks.Insert(0, created);
                else
                    await LoadDailyTasks();

                EmptyLabel.IsVisible = !DailyTasks.Any();
                UpdateBulkActionButtons();
                await Toast.Make("Task added", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to create task.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // ── Swipe actions ────────────────────────────────────────────────────────

        private async void OnItemDeleteSwipe(object sender, EventArgs e)
        {
            if (sender is not SwipeItem si || si.BindingContext is not DailyTask task) return;

            bool ok = await DisplayAlert("Delete", $"Delete '{task.Title}'?", "Delete", "Cancel");
            if (!ok) return;

            try
            {
                ShowLoading();
                await _api.DeleteDailyTaskAsync(task.Id);
                await LoadDailyTasks();
                await Toast.Make("Deleted", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to delete.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        private async void OnItemCompleteSwipe(object sender, EventArgs e)
        {
            if (sender is not SwipeItem si || si.BindingContext is not DailyTask task) return;

            try
            {
                ShowLoading();
                task.IsCompleted = true;
                await _api.UpdateDailyTaskAsync(task);
                await LoadDailyTasks();
                await Toast.Make("Marked completed", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to mark completed.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // ── Bulk actions ─────────────────────────────────────────────────────────

        private async Task OnDeleteSelectedClicked()
        {
            var ids = DailyTasks
                .Where(t => t.IsSelected && !string.IsNullOrWhiteSpace(t.Id))
                .Select(t => t.Id)
                .ToList();

            if (!ids.Any())
            {
                await Toast.Make("No tasks selected", ToastDuration.Short).Show();
                return;
            }

            bool ok = await DisplayAlert(
                "Delete selected",
                $"Delete {ids.Count} selected task(s)?",
                "Delete", "Cancel");
            if (!ok) return;

            try
            {
                ShowLoading();
                try
                {
                    await _api.BulkDeleteDailyTasksAsync(ids);
                }
                catch
                {
                    // fallback: delete one by one if bulk endpoint unavailable
                    foreach (var id in ids)
                        await _api.DeleteDailyTaskAsync(id);
                }

                await LoadDailyTasks();
                await Toast.Make("Deleted selected", ToastDuration.Short).Show();
            }
            catch (Exception ex)
            {
                await Toast.Make($"Failed to delete: {ex.Message}", ToastDuration.Long).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        private async Task OnCompleteSelectedClicked()
        {
            var selected = DailyTasks.Where(t => t.IsSelected).ToList();

            if (!selected.Any())
            {
                await Toast.Make("No tasks selected", ToastDuration.Short).Show();
                return;
            }

            try
            {
                ShowLoading();
                foreach (var t in selected)
                {
                    t.IsCompleted = true;
                    await _api.UpdateDailyTaskAsync(t);
                }

                await LoadDailyTasks();
                await Toast.Make("Marked selected completed", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to complete selected.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // ── Checkbox ─────────────────────────────────────────────────────────────

        private void OnSelectCheckBoxChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is not CheckBox cb || cb.BindingContext is not DailyTask task) return;

            task.IsSelected = e.Value;
            UpdateBulkActionButtons();
        }

        // ── Helpers ───────────────────────────────────────────────────────────────

        private void UpdateBulkActionButtons()
        {
            var anySelected = DailyTasks.Any(t => t.IsSelected);
            DeleteSelectedButton.IsEnabled = anySelected;
            CompleteSelectedButton.IsEnabled = anySelected;
        }
    }
}
