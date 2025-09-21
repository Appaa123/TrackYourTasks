using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrackYourTasks.Data;

namespace TrackYourTasks
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new TasksPage());
        }
        private async void OnCreateTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new CreateTasks(new AppDbContext()));
        }
        private async void OnViewTaskButtonClicked(object? sender, EventArgs e)
        {
            await Navigation.PushAsync(new ViewTasks(new AppDbContext()));
        }
    }
}
