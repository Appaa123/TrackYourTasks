using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Authentication;
using System;
using System.Threading.Tasks;

namespace TrackYourTasks.Services
{
    public class GoogleAuthService
    {
        private readonly string _androidClientId;
        private readonly string _iOSClientId;
        private readonly string _windowsClientId;
        private const string Scope = "openid email profile";

        public GoogleAuthService(IConfiguration config)
        {
            _androidClientId = config["GoogleAuth:AndroidClientId"] ?? "";
            _iOSClientId = config["GoogleAuth:iOSClientId"] ?? "";
            _windowsClientId = config["GoogleAuth:WindowsClientId"] ?? "";
        }

        private string ClientId =>

#if ANDROID
            _androidClientId;
#elif IOS
        _iOSClientId;
#else
        _windowsClientId;
#endif

        private string RedirectUri =>
#if ANDROID
            $"com.companyname.trackyourtasks:/oauth2redirect";
#elif IOS
            $"com.companyname.trackyourtasks:/oauth2redirect";
#else
            "http://localhost";
#endif

        public async Task<GoogleUser?> SignInAsync()
        {
            try
            {
                var authUrl = new Uri(
                    $"https://accounts.google.com/o/oauth2/v2/auth" +
                    $"?client_id={ClientId}" +
                    $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
                    $"&response_type=token" +
                    $"&scope={Uri.EscapeDataString(Scope)}");

                var callbackUrl = new Uri(RedirectUri);

                var result = await WebAuthenticator.Default.AuthenticateAsync(authUrl, callbackUrl);

                if (result == null) return null;

                result.Properties.TryGetValue("access_token", out var accessToken);

                if (string.IsNullOrWhiteSpace(accessToken)) return null;

                // Fetch user info from Google
                return await GetUserInfoAsync(accessToken);
            }
            catch (TaskCanceledException)
            {
                // User cancelled the login
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Google Sign-In error: {ex}");
                return null;
            }
        }

        private async Task<GoogleUser?> GetUserInfoAsync(string accessToken)
        {
            using var client = new System.Net.Http.HttpClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetStringAsync("https://www.googleapis.com/oauth2/v3/userinfo");
            var user = System.Text.Json.JsonSerializer.Deserialize<GoogleUser>(response,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (user != null)
                user.AccessToken = accessToken;

            return user;
        }

        public void SignOut()
        {
            // Clear stored session if any
            SecureStorage.Default.Remove("google_access_token");
            SecureStorage.Default.Remove("google_user_email");
            SecureStorage.Default.Remove("google_user_name");
        }

        public async Task SaveSessionAsync(GoogleUser user)
        {
            await SecureStorage.Default.SetAsync("google_access_token", user.AccessToken ?? "");
            await SecureStorage.Default.SetAsync("google_user_email", user.Email ?? "");
            await SecureStorage.Default.SetAsync("google_user_name", user.Name ?? "");
        }

        public async Task<bool> IsSignedInAsync()
        {
            var token = await SecureStorage.Default.GetAsync("google_access_token");
            return !string.IsNullOrWhiteSpace(token);
        }

        public async Task<GoogleUser?> GetSavedUserAsync()
        {
            var token = await SecureStorage.Default.GetAsync("google_access_token");
            var email = await SecureStorage.Default.GetAsync("google_user_email");
            var name = await SecureStorage.Default.GetAsync("google_user_name");

            if (string.IsNullOrWhiteSpace(token)) return null;

            return new GoogleUser
            {
                AccessToken = token,
                Email = email,
                Name = name
            };
        }
    }

    public class GoogleUser
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Picture { get; set; }
        public string? Sub { get; set; } // Google user ID
        public string? AccessToken { get; set; }
    }
}