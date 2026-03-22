using Microcharts;
using SkiaSharp;
using TrackYourTasks.Data;

namespace TrackYourTasks;

public partial class AnalyticsPage : ContentPage
{
    public AnalyticsPage()
    {
        InitializeComponent();
        LoadChart();
    }

    private void LoadChart()
    {
        using var db = new AppDbContext();

        var completed = db.Tasks.Count(t => t.IsCompleted);
        var pending = db.Tasks.Count(t => !t.IsCompleted && !t.IsSkipped);
        var skipped = db.Tasks.Count(t => t.IsSkipped);
        var partial = db.Tasks.Count(t => t.IsPartiallyCompleted);

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