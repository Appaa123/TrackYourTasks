using System;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using TrackYourTasks.Services;

namespace TrackYourTasks
{
    public partial class LoginPage : ContentPage
    {
        private readonly GoogleAuthService _authService;
        private readonly ApiService _api;

        public LoginPage(GoogleAuthService authService, ApiService api)
        {
            InitializeComponent();
            _authService = authService;
            _api = api;
        }

        private async void OnGoogleSignInClicked(object sender, EventArgs e)
        {
            try
            {
                SetLoading(true);
                ErrorLabel.IsVisible = false;

                var user = await _authService.SignInAsync();

                if (user == null)
                {
                    ShowError("Sign-in was cancelled or failed. Please try again.");
                    return;
                }

                // Save session for auto-login next time
                await _authService.SaveSessionAsync(user);

                // Navigate to main app via App
                if (Application.Current is App app)
                    await app.NavigateToMainApp(_api);
            }
            catch (Exception ex)
            {
                ShowError($"Sign-in failed: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Login error: {ex}");
            }
            finally
            {
                SetLoading(false);
            }
        }

        private void SetLoading(bool isLoading)
        {
            LoadingIndicator.IsVisible = isLoading;
            LoadingIndicator.IsRunning = isLoading;
            GoogleSignInButton.IsEnabled = !isLoading;
        }

        private void ShowError(string message)
        {
            ErrorLabel.Text = message;
            ErrorLabel.IsVisible = true;
        }
    }
}
