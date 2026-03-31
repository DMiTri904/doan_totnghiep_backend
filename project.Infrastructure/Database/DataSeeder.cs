using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using project.Application.Interfaces;
using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Database
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _password;

        public DataSeeder(IPasswordHasher password, ApplicationDbContext context)
        {
            _password = password;
            _context = context;
        }
        public async Task SeedAsync()
        {
            await SeedUsersAsync();
            await SeedGroupsAsync();
            await _context.SaveChangesAsync();
        }

        private async Task SeedUsersAsync()
        {
            if (_context.User.Any()) return;

            var users = new List<UserApp>
            {
                UserApp.Create("admin","admin@gmail.com",_password.Hash("admin123"),"ADMIN001",UserRole.Admin),
                UserApp.Create("Hau","haukong1308@gmail.com",_password.Hash("hau123"),"123",UserRole.Student),
                UserApp.Create("Tri","dangminhtri94@gmail.com",_password.Hash("tri123"),"124",UserRole.Student),
                UserApp.Create("XYZ","XYZ@gmail.com",_password.Hash("xyz"),"125",UserRole.Student),
                UserApp.Create("teacher","teacher@gamil.com",_password.Hash("teacher"),"TC1",UserRole.Teacher)
            };

            await _context.User.AddRangeAsync(users);
            await _context.SaveChangesAsync();
        }
        private async Task SeedGroupsAsync()
        {
            if (_context.Groups.Any()) return;

            var leader = await _context.User.FirstAsync(u => u.UserName == "Tri");

            var group = Groups.Create("HTML, CSS tips", "Web Development", leader.Id, 10);
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();

            var members = new List<GroupMem>
        {
            GroupMem.Create(group.Id, leader.Id, GroupMemberRole.Leader),
            // thêm member khác nếu cần
        };

            await _context.GroupMem.AddRangeAsync(members);
        }
    }
}
