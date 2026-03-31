using Azure;
using Microsoft.Extensions.Configuration;
using project.Application.Interfaces;
using project.Application.ModelsDto;
using project.Domain.Exceptions;
using project.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project.Infrastructure.Services.GithubService
{
    public class GithubService : IGithubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public GithubService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", _configuration["App:Name"]);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["Github:Key"]);
        }

        public async Task<int> GetTotalCommitAsync(string userName, string repo, string owner)
        {
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}/commits?author={userName}&per_page=100");
            if (!response.IsSuccessStatusCode) return 0;

            var json = await response.Content.ReadFromJsonAsync<JsonElement[]>() ?? [];
            var commit = json.Length;
            return commit;
        }

        public async Task<string?> GetRepoOwnerAsync(string owner, string repo)
        {
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) throw new DomainException("Không tìm thấy repo");
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var isPrivate = json.GetProperty("private").GetBoolean();
            if (isPrivate) throw new DomainException("Ứng dụng chỉ hỗ trợ repo public");
            return json.GetProperty("owner").GetProperty("login").GetString();
        }

        public async Task<bool> InviteCollaboratorAsync(string owner, string repo, string githubUserName)
        {
            var reponse = await _httpClient.PutAsJsonAsync($"https://api.github.com/repos/{owner}/{repo}/collaborators/{githubUserName}", new {permission = "push"});
            return reponse.IsSuccessStatusCode;
        }

        public async Task<bool> IsValidRepositoryAsync(string owner, string repo)
        {
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}");
            if (!response.IsSuccessStatusCode) return false;
            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            var isPrivate = json.GetProperty("private").GetBoolean();
            if (isPrivate) throw new DomainException("Ứng dụng chỉ hỗ trợ repo public");
            return response.IsSuccessStatusCode;
        }

        private async Task<List<PullRequestModel>> GetPullRequestAsync(string owner, string repo)
        {
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}/pulls?state=all&per_page=100");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PullRequestModel>>(json) ?? [];
        }

        private async Task<PullRequestModel?> GetPullRequestDetailsAsync(string owner, string repo, int prNumber)
        {
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}/pulls/{prNumber}");
            if(!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<PullRequestModel>(json);
        }

        public async Task<PullRequestModel?> GetPRByTaskIdAsync(string owner, string repo, int taskId)
        {
            var list = await GetPullRequestAsync(owner, repo);

            var matched = list.FirstOrDefault(pr => pr.Head?.Ref == $"feature/task-{taskId}");
            if (matched == null) return null;

            return await GetPullRequestDetailsAsync(owner, repo, matched.Number);
        }

        public async Task<bool> IsBranchExistAsync(string owner, string repo, int taskId)
        {
            var branchName = $"feature/task-{taskId}";
            var response = await _httpClient.GetAsync($"https://api.github.com/repos/{owner}/{repo}/branches/{branchName}");
            return response.IsSuccessStatusCode;
        }
    }
}
