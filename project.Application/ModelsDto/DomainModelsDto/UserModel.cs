using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.ModelsDto.DomainModelsDto
{
    public class UserModel
    {
        public int Id { get; private set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public string UserName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? AvatarUrl { get; private set; } = string.Empty;
        public string? StudentCode { get; private set; } = string.Empty;
        public UserRole UserRole { get; private set; }
    }
}
