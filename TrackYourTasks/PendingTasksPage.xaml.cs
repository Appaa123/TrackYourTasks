using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Models;

namespace TrackYourTasks;

public partial class PendingTasksPage : ContentPage
{
    private readonly AppDbContext _db;
    private List<TrackTask> _tasksBeingEdited;
    private TrackTask _taskBeingEdited;
    private List<int> skippedTasks = new List<int>();

    public PendingTasksPage(AppDbContext db)
    {
        InitializeComponent();
        _db = db;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        using var db = new AppDbContext();
        _tasksBeingEdited = await db.Tasks
                              .Where(t => !t.IsCompleted && !t.IsSkipped)
                              .ToListAsync();

        TrackTask skippedTask = _tasksBeingEdited.Find(task => skippedTasks.Contains(task.Id));

        if(_tasksBeingEdited.Count == 0)
        {
            await Toast.Make("No pending tasks found. Enjoy your day!", ToastDuration.Short).Show();
            await Navigation.PushAsync(new MainPage());
        }
        if (skippedTask != null)
        {
            
            skippedTask.IsSkipped = true;
            _taskBeingEdited = await db.Tasks.FirstOrDefaultAsync(t => t.Id == skippedTask.Id);

            if (_taskBeingEdited != null)
            {
                _taskBeingEdited.IsSkipped = true;
                await db.SaveChangesAsync();
            }

            skippedTasks.Remove(skippedTask.Id); // for tracking skipped tasks
            _tasksBeingEdited.Remove(skippedTask); // for viewing
        }
        else if(_tasksBeingEdited.All(t => t.IsCompleted))
        {
            skippedTasks.Clear();
            await Navigation.PushAsync(new MainPage());
        }

        TasksCollection.ItemsSource = _tasksBeingEdited;

        if (_tasksBeingEdited == null || _tasksBeingEdited.Count == 0)
        {
            // Show empty message
            EmptyMessage.IsVisible = true;

            // Hide the list
            TasksCollection.IsVisible = false;

            // Change button text
            SkipAllButton.Text = "Continue";
        }
        else
        {
            EmptyMessage.IsVisible = false;
            TasksCollection.IsVisible = true;
            SkipAllButton.Text = "Skip All";
        }

    }

    private async void OnYesClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        int taskId = (int)button.CommandParameter;

        using var db = new AppDbContext();
        var task = await db.Tasks.FindAsync(taskId);
        task.IsCompleted = true;
        await db.SaveChangesAsync();

        await Toast.Make("Task marked completed!", ToastDuration.Short).Show();
        //await DisplayAlert("Done", "Task marked completed", "OK");

        OnAppearing(); // Refresh list
    }

    private async void OnNoClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        int taskId = (int)button.CommandParameter;

        using var db = new AppDbContext();
        var task = await db.Tasks.FindAsync(taskId);
        skippedTasks.Add(taskId);
        await Toast.Make("Try to complete later!", ToastDuration.Short).Show();
        OnAppearing(); // Refresh list
    }
    private async void OnSkipClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        int taskId = (int)button.CommandParameter;

        using var db = new AppDbContext();
        var task = await db.Tasks.FindAsync(taskId);
        skippedTasks.Add(taskId);
        await Toast.Make("Skipping the status check of pending task - " + task?.Title, ToastDuration.Short).Show();
        OnAppearing(); // Refresh list
    }
    private async void OnSkipAllClicked(object sender, EventArgs e)
    {
        if (SkipAllButton.Text == "Continue")
        {
            await Navigation.PushAsync(new MainPage());
        }
        else
        {
            await Toast.Make("Skipping the status check of all the pending tasks", ToastDuration.Short).Show();
            await Navigation.PushAsync(new MainPage());
        }
    }
}