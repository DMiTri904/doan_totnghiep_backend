using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Interfaces
{
    public interface IUserIdentityValidation
    {
        Task<User?> FindUserNameAsync(string userName);
        Task<User?> FindEmailAsync(string email);
        Task<User?> FindByIdAsync(string id);
        Task<bool> CheckPasswordAsync(User user, string password);
    }
    public interface IUserTokenService
    {
        Task<string> GenerateTokenAsync(User user);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
