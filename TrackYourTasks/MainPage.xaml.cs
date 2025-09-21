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

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
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
