using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Alerts;
using TrackYourTasks.Models;
using TrackYourTasks.Services;
using Microsoft.Maui.Controls;
using CommunityToolkit.Maui.Core;

namespace TrackYourTasks
{
    public partial class DailyTrackerPage : ContentPage
    {
        private readonly ApiService _api;
        public ObservableCollection<DailyTask> DailyTasks { get; } = new();

        // runtime spinner
        private ActivityIndicator _loadingIndicator;
        private int _loadingCount = 0;

        public DailyTrackerPage(ApiService api)
        {
            InitializeComponent();
            _api = api;

            // overlay spinner while preserving the XAML visual tree
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

            TasksCollection.ItemsSource = DailyTasks;

            AddButton.Clicked += async (_, __) => await OnAddClicked();
            DeleteSelectedButton.Clicked += async (_, __) => await OnDeleteSelectedClicked();
            CompleteSelectedButton.Clicked += async (_, __) => await OnCompleteSelectedClicked();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadDailyTasks();
        }

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

        private async Task LoadDailyTasks()
        {
            try
            {
                ShowLoading();
                var tasks = await _api.GetDailyTasksAsync();
                DailyTasks.Clear();
                foreach (var t in tasks) DailyTasks.Add(t);

                EmptyLabel.IsVisible = !DailyTasks.Any();
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

        private async Task OnAddClicked()
        {
            var title = await DisplayPromptAsync("New Daily Task", "Title", initialValue: string.Empty, maxLength: 200);
            if (string.IsNullOrWhiteSpace(title)) return;

            var newTask = new DailyTask
            {
                Title = title,
                CreatedAt = DateTime.UtcNow,
                IsCompleted = false
            };

            try
            {
                ShowLoading();

                // call API which returns the created object (with Id assigned by server)
                var created = await _api.CreateDailyTaskAsync(newTask);

                // If API returned the created item, add it to the collection; otherwise reload
                if (created != null && !string.IsNullOrWhiteSpace(created.Id))
                {
                    DailyTasks.Insert(0, created);
                }
                else
                {
                    await LoadDailyTasks();
                }

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

        private async void OnTitleTapped(object sender, EventArgs e)
        {
            if (!(sender is Label lbl) || !(lbl.BindingContext is DailyTask task)) return;

            var newTitle = await DisplayPromptAsync("Edit Task", "Title", initialValue: task.Title, maxLength: 200);
            if (string.IsNullOrWhiteSpace(newTitle) || newTitle == task.Title) return;

            task.Title = newTitle;
            try
            {
                ShowLoading();
                await _api.UpdateDailyTaskAsync(task);
                await LoadDailyTasks();
                await Toast.Make("Task updated", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to update task.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // single completed toggle handler
        private async void OnItemCompletedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!(sender is CheckBox cb) || !(cb.BindingContext is DailyTask task)) return;

            task.IsCompleted = e.Value;
            try
            {
                ShowLoading();
                await _api.UpdateDailyTaskAsync(task);
                await LoadDailyTasks();
            }
            catch (Exception)
            {
                await Toast.Make("Failed to update task.", ToastDuration.Short).Show();
            }
            finally
            {
                HideLoading();
            }
        }

        // swipe -> single delete
        private async void OnItemDeleteSwipe(object sender, EventArgs e)
        {
            if (!(sender is SwipeItem si) || !(si.BindingContext is DailyTask task)) return;

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

        // swipe -> single complete
        private async void OnItemCompleteSwipe(object sender, EventArgs e)
        {
            if (!(sender is SwipeItem si) || !(si.BindingContext is DailyTask task)) return;

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

        private async Task OnDeleteSelectedClicked()
        {
            var ids = DailyTasks.Where(t => t.IsSelected).Select(t => t.Id).Where(id => !string.IsNullOrWhiteSpace(id)).ToList();
            if (!ids.Any())
            {
                await Toast.Make("No tasks selected", ToastDuration.Short).Show();
                return;
            }

            bool ok = await DisplayAlert("Delete selected", $"Delete {ids.Count} selected tasks?", "Delete", "Cancel");
            if (!ok) return;

            try
            {
                ShowLoading();
                await _api.BulkDeleteDailyTasksAsync(ids);
                await LoadDailyTasks();
                await Toast.Make("Deleted selected", ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                // fallback: try per-item deletion if server bulk endpoint not implemented
                try
                {
                    foreach (var id in ids) await _api.DeleteDailyTaskAsync(id);
                    await LoadDailyTasks();
                    await Toast.Make("Deleted selected", ToastDuration.Short).Show();
                }
                catch
                {
                    await Toast.Make("Failed to delete selected.", ToastDuration.Short).Show();
                }
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
    }
}