using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using TrackYourTasks.Models;
using TrackYourTasks.Services;
using Microsoft.Maui.Controls;

namespace TrackYourTasks;

public partial class PendingTasksPage : ContentPage
{
    private readonly ApiService _api;
    private List<TrackTask> _tasksBeingEdited = new();
    private List<string> skippedTasks = new();

    // runtime spinner
    private ActivityIndicator _loadingIndicator;
    private int _loadingCount = 0;

    public PendingTasksPage(ApiService api)
    {
        InitializeComponent();
        _api = api;

        // Preserve existing visual tree and overlay spinner
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

    // 🔄 Load tasks cleanly
    private async Task LoadTasks()
    {
        try
        {
            ShowLoading();

            var allTasks = await _api.GetTasksAsync();

            _tasksBeingEdited = allTasks
                .Where(t => !t.IsCompleted && !t.IsSkipped)
                .Where(t => !skippedTasks.Contains(t.Id))
                .ToList();

            if (!_tasksBeingEdited.Any())
            {
                await Toast.Make("No pending tasks found. Enjoy your day!", ToastDuration.Short).Show();

                await Navigation.PushAsync(new MainPage(_api)); // 🔥 FIX
                return;
            }

            TasksCollection.ItemsSource = _tasksBeingEdited;

            EmptyMessage.IsVisible = !_tasksBeingEdited.Any();
            TasksCollection.IsVisible = _tasksBeingEdited.Any();
            SkipAllButton.Text = _tasksBeingEdited.Any() ? "Skip All" : "Continue";
        }
        finally
        {
            HideLoading();
        }
    }

    // ✅ YES → complete
    private async void OnYesClicked(object sender, EventArgs e)
    {
        var taskId = (sender as Button)?.CommandParameter?.ToString();
        if (taskId == null) return;

        var task = _tasksBeingEdited.FirstOrDefault(t => t.Id == taskId);
        if (task == null) return;

        task.IsCompleted = true;

        try
        {
            ShowLoading();
            await _api.UpdateTaskAsync(task);
            await Toast.Make("Task marked completed!", ToastDuration.Short).Show();
            await LoadTasks(); // 🔥 FIX
        }
        catch (Exception)
        {
            await Toast.Make("Failed to mark task completed.", ToastDuration.Short).Show();
        }
        finally
        {
            HideLoading();
        }
    }

    // ❌ NO → skip temporarily
    private async void OnNoClicked(object sender, EventArgs e)
    {
        var taskId = (sender as Button)?.CommandParameter?.ToString();
        if (taskId == null) return;

        skippedTasks.Add(taskId);

        await Toast.Make("Try to complete later!", ToastDuration.Short).Show();

        await LoadTasks();
    }

    // ⏭ Skip one
    private async void OnSkipClicked(object sender, EventArgs e)
    {
        var taskId = (sender as Button)?.CommandParameter?.ToString();
        if (taskId == null) return;

        skippedTasks.Add(taskId);

        var task = _tasksBeingEdited.FirstOrDefault(t => t.Id == taskId);

        await Toast.Make($"Skipping: {task?.Title}", ToastDuration.Short).Show();

        await LoadTasks();
    }

    // ⏭ Skip all
    private async void OnSkipAllClicked(object sender, EventArgs e)
    {
        if (SkipAllButton.Text == "Continue")
        {
            await Navigation.PushAsync(new MainPage(_api)); // 🔥 FIX
        }
        else
        {
            await Toast.Make("Skipping all pending tasks", ToastDuration.Short).Show();
            await Navigation.PushAsync(new MainPage(_api)); // 🔥 FIX
        }
    }
}