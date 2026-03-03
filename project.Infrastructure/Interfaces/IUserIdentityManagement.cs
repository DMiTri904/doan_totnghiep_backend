using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Interfaces
{
    public interface IUserIdentityManagement
    {
        Task<User> CreateUserAsync(User user, string? password);
        Task UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(string userId);
    }
}
