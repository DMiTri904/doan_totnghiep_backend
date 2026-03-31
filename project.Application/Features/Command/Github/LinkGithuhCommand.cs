using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using project.Application.Interfaces;
using project.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Github
{
    public sealed record LinkGithuhCommand(string Code, string State) : IRequest<LinkGithubResult>
    {
    }
    public sealed class LinkGithubHandler : IRequestHandler<LinkGithuhCommand, LinkGithubResult>
    {
        private readonly IGithubOAuthService _github;
        private readonly IUserRepository _userRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly IUnitOfWork _unitOfWork;

        public LinkGithubHandler(IUnitOfWork unitOfWork, IMemoryCache memoryCache, IUserRepository userRepository, IGithubOAuthService github)
        {
            _unitOfWork = unitOfWork;
            _memoryCache = memoryCache;
            _userRepository = userRepository;
            _github = github;
        }

        public async Task<LinkGithubResult> Handle(LinkGithuhCommand request, CancellationToken cancellationToken)
        {
            if (!_memoryCache.TryGetValue($"github_state_{request.State}", out _)) return new LinkGithubResult(false, "invalid_state");
            _memoryCache.Remove($"github_state_{request.State}");

            var stateData = JsonSerializer.Deserialize<JsonElement>(Convert.FromBase64String(Uri.UnescapeDataString(request.State)));
            var userId = stateData.GetProperty("UserId").GetInt32()!;
            var inputEmail = stateData.GetProperty("Email").GetString()!;

            var accessToken = await _github.GetAccessTokenAsync(request.Code);
            if (accessToken == null) return new LinkGithubResult(false, "Token_error");

            var emails = await _github.GetEmailAsync(accessToken);

            var matched = emails.Any(e => e.Email.Equals(inputEmail, StringComparison.OrdinalIgnoreCase) && e.Verified);

            if (!matched) return new LinkGithubResult(false, "Email không giống nhau");

            var githubUser = await _github.GetUserAsync(accessToken);

            var user = await _userRepository.GetByIdAsync(userId);
            user!.LinkGitHubAccount(githubUser.Login, githubUser.Id);
            user.VerifyGithubAccount();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LinkGithubResult(true);
        }
    }

    public  record LinkGithubResult(bool Success, string? Error = null);
}
