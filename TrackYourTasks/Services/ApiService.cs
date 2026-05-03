using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Models;

namespace TrackYourTasks.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;


        public ApiService(string baseUrl)
        {
            _http = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
        }

        public async Task<List<TrackTask>> GetTasksAsync()
        {
            return await _http.GetFromJsonAsync<List<TrackTask>>("api/tasks")
                   ?? new List<TrackTask>();
        }

        public async Task CreateTaskAsync(TrackTask task)
        {
            var res = await _http.PostAsJsonAsync("api/tasks", task);
            res.EnsureSuccessStatusCode();
        }

        public async Task UpdateTaskAsync(TrackTask task)
        {
            var res = await _http.PutAsJsonAsync($"api/tasks/{task.Id}", task);
            res.EnsureSuccessStatusCode();
        }

        public async Task DeleteTaskAsync(string id)
        {
            var res = await _http.DeleteAsync($"api/tasks/{id}");
            res.EnsureSuccessStatusCode();
        }
    }
}
