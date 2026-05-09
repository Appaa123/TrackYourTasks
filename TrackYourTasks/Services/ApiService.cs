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

        public async Task<List<DailyTask>> GetDailyTasksAsync()
        {
            return await _http.GetFromJsonAsync<List<DailyTask>>("api/dailytasks")
                   ?? new List<DailyTask>();
        }

        // Return the created DailyTask returned by the API (so client can get the assigned Id)
        public async Task<DailyTask> CreateDailyTaskAsync(DailyTask task)
        {
            var res = await _http.PostAsJsonAsync("api/dailytasks", task);
            res.EnsureSuccessStatusCode();

            // API returns the created DailyTask (CreatedAtAction or Ok). Read and return it.
            var created = await res.Content.ReadFromJsonAsync<DailyTask>();
            return created ?? task;
        }

        public async Task UpdateDailyTaskAsync(DailyTask task)
        {
            var res = await _http.PutAsJsonAsync($"api/dailytasks/{task.Id}", task);
            res.EnsureSuccessStatusCode();
        }

        public async Task DeleteDailyTaskAsync(string id)
        {
            var res = await _http.DeleteAsync($"api/dailytasks/{id}");
            res.EnsureSuccessStatusCode();
        }

        // optional bulk delete endpoint (backend to implement later)
        public async Task BulkDeleteDailyTasksAsync(List<string> ids)
        {
            var res = await _http.PostAsJsonAsync("api/dailytasks/bulkDelete", ids);
            res.EnsureSuccessStatusCode();
        }
    }
}
