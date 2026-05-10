using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using TrackYourTasks.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Diagnostics;

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
            var json = await _http.GetStringAsync("api/dailytasks");
            Debug.WriteLine("GetDailyTasks raw JSON:");
            Debug.WriteLine(json);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var root = JsonSerializer.Deserialize<JsonElement>(json, options);
                if (root.ValueKind != JsonValueKind.Array)
                    return new List<DailyTask>();

                var list = new List<DailyTask>();
                foreach (var item in root.EnumerateArray())
                {
                    // case A: { "task": { ... } }
                    if (item.ValueKind == JsonValueKind.Object && item.TryGetProperty("task", out var taskEl))
                    {
                        var task = taskEl.Deserialize<DailyTask>(options);
                        if (task != null) list.Add(task);
                        continue;
                    }

                    // case B: item is the task object itself
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        var task = item.Deserialize<DailyTask>(options);
                        if (task != null) list.Add(task);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Deserialization failed: {ex}");
                return new List<DailyTask>();
            }
        }

        // envelope used only for deserialization of the API shape
        private class DailyTaskEnvelope
        {
            public DailyTask? Task { get; set; }
        }

        // Return the created DailyTask returned by the API (so client can get the assigned Id)
        public async Task<DailyTask> CreateDailyTaskAsync(DailyTask task)
        {
            // send envelope { "task": { ... } } because API expects that shape
            var payload = new { task };
            var res = await _http.PostAsJsonAsync("api/dailytasks", payload);
            res.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            // Try to parse the response robustly: either { "task": { ... } } or the task object directly
            try
            {
                var root = await res.Content.ReadFromJsonAsync<JsonElement>(options);
                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("task", out var taskEl))
                    {
                        var created = taskEl.Deserialize<DailyTask>(options);
                        return created ?? task;
                    }

                    var direct = root.Deserialize<DailyTask>(options);
                    return direct ?? task;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateDailyTaskAsync: failed to parse JsonElement response: {ex}");
            }

            // fallback: try reading directly as DailyTask
            try
            {
                var direct = await res.Content.ReadFromJsonAsync<DailyTask>(options);
                return direct ?? task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"CreateDailyTaskAsync: fallback deserialization failed: {ex}");
            }

            // if all else fails, return the original local object
            return task;
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
