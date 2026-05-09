using TrackYourTasks.Models;
using TrackYourTasks.Services;
using Microsoft.Maui.Controls;

namespace TrackYourTasks
{
	public partial class SecondPage : ContentPage
	{
		private readonly ApiService _api;
		private List<TrackTask> _tasks = new();

        // runtime spinner
        private ActivityIndicator _loadingIndicator;
        private int _loadingCount = 0;

		public SecondPage(ApiService api)
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

		protected override async void OnAppearing()
		{
			base.OnAppearing();
            await LoadTasks();
		}

		// ✅ Load from API
		private async Task LoadTasks()
		{
            try
            {
                ShowLoading();
			    _tasks = await _api.GetTasksAsync();
			    TasksList.ItemsSource = _tasks;
            }
            finally
            {
                HideLoading();
            }
		}

		// ✅ YES → mark completed / create if not exists
		private async void OnYesClicked(object? sender, EventArgs e)
		{
			var button = sender as Button;
			string taskName = button?.CommandParameter?.ToString();

			var task = _tasks.FirstOrDefault(t => t.Title == taskName);

            try
            {
                ShowLoading();

                if (task == null)
                {
                    // 🔥 CREATE
                    task = new TrackTask
                    {
                        Title = taskName ?? "Unknown Task",
                        Description = "Task completed by user",
                        IsCompleted = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _api.CreateTaskAsync(task);
                }
                else
                {
                    // 🔥 UPDATE
                    task.IsCompleted = true;
                    task.Description = "User clicked YES";

                    await _api.UpdateTaskAsync(task);
                }

                await LoadTasks();
            }
            finally
            {
                HideLoading();
            }
		}

		// ❌ NO → mark incomplete
		private async void OnNoClicked(object? sender, EventArgs e)
		{
			var button = sender as Button;
			string taskName = button?.CommandParameter?.ToString();

			var task = _tasks.FirstOrDefault(t => t.Title == taskName);
			if (task == null) return;

            try
            {
                ShowLoading();

			    task.IsCompleted = false;
			    task.Description = "User clicked NO";

			    await _api.UpdateTaskAsync(task);

			    await LoadTasks();
            }
            finally
            {
                HideLoading();
            }
		}
	}
}