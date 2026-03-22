using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TrackYourTasks.Data;
using TrackYourTasks.Interfaces;
using TrackYourTasks.Models;

namespace TrackYourTasks;

public partial class PendingTasksPage : ContentPage
{
    private readonly INotificationService _notificationService;
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
                              .Where(t => !t.IsCompleted)
                              .ToListAsync();

        TrackTask skippedTask = _tasksBeingEdited.Find(task => skippedTasks.Contains(task.Id));

        if(_tasksBeingEdited.Count == 0)
        {
            await Toast.Make("No pending tasks found. Enjoy your day!", ToastDuration.Short).Show();
            await Navigation.PushAsync(new MainPage(_notificationService));
        }
        if (skippedTask != null)
        {
            _tasksBeingEdited.Remove(skippedTask);
        }

        TasksCollection.ItemsSource = _tasksBeingEdited;
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
        //await DisplayAlert("Reminder", "Try to complete later", "OK");
        await Toast.Make("Try to complete later!", ToastDuration.Short).Show();
        OnAppearing(); // Refresh list
        //await Navigation.PushAsync(new MainPage(_notificationService));
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
        var button = sender as Button;
        await Toast.Make("Skipping the status check of all the pending tasks", ToastDuration.Short).Show();
        await Navigation.PushAsync(new MainPage(_notificationService));
    }
}