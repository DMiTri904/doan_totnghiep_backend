using project.Application.ModelsDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Interfaces
{
    public interface IGithubService
    {
        Task<int> GetTotalCommitAsync(string owner,string repo,string userName);
        Task<bool> IsValidRepositoryAsync(string owner, string repo);
        Task<string?> GetRepoOwnerAsync(string owner, string repo);
        Task<PullRequestModel?> GetPRByTaskIdAsync(string owner, string repo, int taskId);
        Task<bool> IsBranchExistAsync(string owner, string repo, int taskId);
    }
    
}
