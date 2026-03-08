using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackYourTasks.Popups
{

    public partial class ConfirmPopup : Popup
    {
        private TaskCompletionSource<bool> _taskCompletionSource;

        public string Message { get; set; }
        public ConfirmPopup(string message)
        {
            InitializeComponent();
            _taskCompletionSource = new TaskCompletionSource<bool>();
            BindingContext = this;
        }

        // This will be called to show popup and wait for result
        public Task<bool> ShowAsync(Page page)
        {
            page.ShowPopup(this); // Display popup
            return _taskCompletionSource.Task; // Return the result task
        }

        private async void OnYesClicked(object sender, EventArgs e)
        {
            _taskCompletionSource.SetResult(true);
            await CloseAsync(); // Close without passing result
        }

        private async void OnNoClicked(object sender, EventArgs e)
        {
            _taskCompletionSource.SetResult(false);
            await CloseAsync();
        }
    }

}
