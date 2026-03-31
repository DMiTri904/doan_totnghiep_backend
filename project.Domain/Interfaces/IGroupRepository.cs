using project.Domain.Models;
using System.Text.RegularExpressions;

namespace project.Domain.Interfaces
{
    public interface IGroupRepository : IGenericRepository<Groups>
    {
        Task<Groups?> GetByIdWithMemberAsync(int id);
        Task<Groups?> GetByIdWithDetailAsync(int id);
        Task<List<Groups>> GetAllGroupsByUserIdAsync(int userId);

    }
}
