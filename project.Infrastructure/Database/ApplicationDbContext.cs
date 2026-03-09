using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<UserApp> User { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<ActivityLog> ActivityLog { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<GroupMem> GroupMem { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<Label> Label { get; set; }
        public DbSet<TaskHistory> TaskHistories { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<TaskLabel> TaskLabel { get; set; }
        public DbSet<WorkTask> WorkTask { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
