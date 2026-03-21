using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.EntityFrameworkCore;
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

        await DisplayAlert("Done", "Task marked completed", "OK");

        OnAppearing(); // Refresh list
    }

    private async void OnNoClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Reminder", "Try to complete later", "OK");
    }
}