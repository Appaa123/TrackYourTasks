using TrackYourTasks.Models;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
	public partial class SecondPage : ContentPage
	{
		private readonly ApiService _api;
		private List<TrackTask> _tasks = new();

		public SecondPage(ApiService api)
		{
			InitializeComponent();
			_api = api;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();
			await LoadTasks();
		}

		// ✅ Load from API
		private async Task LoadTasks()
		{
			_tasks = await _api.GetTasksAsync();
			TasksList.ItemsSource = _tasks;
		}

		// ✅ YES → mark completed / create if not exists
		private async void OnYesClicked(object? sender, EventArgs e)
		{
			var button = sender as Button;
			string taskName = button?.CommandParameter?.ToString();

			var task = _tasks.FirstOrDefault(t => t.Title == taskName);

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

		// ❌ NO → mark incomplete
		private async void OnNoClicked(object? sender, EventArgs e)
		{
			var button = sender as Button;
			string taskName = button?.CommandParameter?.ToString();

			var task = _tasks.FirstOrDefault(t => t.Title == taskName);
			if (task == null) return;

			task.IsCompleted = false;
			task.Description = "User clicked NO";

			await _api.UpdateTaskAsync(task);

			await LoadTasks();
		}
	}
}