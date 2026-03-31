using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Domain.Interfaces
{
    public interface ILabelRepository : IGenericRepository<Label>
    {
        Task<IReadOnlyList<Label>> GetByGroupIdAsync(int groupId, int labelId);
    }
}
