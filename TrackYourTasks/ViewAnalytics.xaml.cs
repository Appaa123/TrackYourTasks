using Microcharts;
using SkiaSharp;
using TrackYourTasks.Models;
using TrackYourTasks.Services;

namespace TrackYourTasks;

public partial class AnalyticsPage : ContentPage
{
	private readonly ApiService _api;

	public AnalyticsPage(ApiService api)
	{
		InitializeComponent();
		_api = api;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadChart();
	}

	private async Task LoadChart()
	{
		var tasks = await _api.GetTasksAsync();

		var completed = tasks.Count(t => t.IsCompleted);
		var pending = tasks.Count(t => !t.IsCompleted && !t.IsSkipped);
		var skipped = tasks.Count(t => t.IsSkipped);
		var partial = tasks.Count(t => t.IsPartiallyCompleted);

		var entries = new List<ChartEntry>
		{
			new ChartEntry(completed)
			{
				Label = "Completed",
				ValueLabel = completed.ToString(),
				Color = SKColor.Parse("#4CAF50")
			},
			new ChartEntry(pending)
			{
				Label = "Pending",
				ValueLabel = pending.ToString(),
				Color = SKColor.Parse("#FFB74D")
			},
			new ChartEntry(skipped)
			{
				Label = "Skipped",
				ValueLabel = skipped.ToString(),
				Color = SKColor.Parse("#64B5F6")
			},
			new ChartEntry(partial)
			{
				Label = "Partial",
				ValueLabel = partial.ToString(),
				Color = SKColor.Parse("#BA68C8")
			}
		};

		PieChart.Chart = new PieChart
		{
			Entries = entries
		};
	}

	private async void OnBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}