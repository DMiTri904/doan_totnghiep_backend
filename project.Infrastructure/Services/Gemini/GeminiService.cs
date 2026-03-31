using Microsoft.Extensions.Configuration;
using project.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project.Infrastructure.Services.Gemini
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;

        public GeminiService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            var apiKey = configuration["Gemini:ApiKey"];
            _url = $"{configuration["Gemini:Url"]}?key={apiKey}";
        }

        private async Task<string?> CallGeminiAsync(string promt)
        {
            var body = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new
                            {
                                text = promt
                            }
                        }
                    }
                }
            };
            var response = await _httpClient.PostAsJsonAsync(_url, body);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("candidates")[0]
                       .GetProperty("content")
                       .GetProperty("parts")[0]
                       .GetProperty("text")
                       .GetString();
        }
        public Task<string?> GenerateTaskDescriptionAsync(string taskTitle) =>
            CallGeminiAsync($"Viết mô tả chi tiết cho task '{taskTitle}', ngắn gọn 3-5 câu, bằng tiếng Việt.");

        public Task<string?> SuggestSubTasksAsync(string taskTitle) =>
            CallGeminiAsync($"Gợi ý 3-5 công việc con cho task '{taskTitle}' trong dự án nhóm sinh viên, bằng tiếng Việt.");

        public Task<string?> EstimateTimeAsync(string taskTitle) =>
            CallGeminiAsync($"Ước tính thời gian hoàn thành task '{taskTitle}' trong dự án nhóm sinh viên, bằng tiếng Việt.");

        public Task<string?> SuggestPriorityAsync(string taskTitle, string taskDescription) =>
            CallGeminiAsync($"Gợi ý mức độ ưu tiên (Thấp/Trung bình/Cao/Khẩn cấp) cho task '{taskTitle}': '{taskDescription}', bằng tiếng Việt.");

        public Task<string?> ReviewGroupProgressAsync(int total, int completed, int inProgress, int todo, DateTime deadline) =>
            CallGeminiAsync($"Nhận xét tiến độ nhóm: Tổng {total}, Hoàn thành {completed}, Đang làm {inProgress}, Chưa bắt đầu {todo}, Deadline {deadline:dd/MM/yyyy}. Bằng tiếng Việt.");
    }
}
