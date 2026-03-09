using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Infrastructure.Database.ConfigEntity
{
    public class TaskHistoryConfig : IEntityTypeConfiguration<TaskHistory>
    {
        public void Configure(EntityTypeBuilder<TaskHistory> builder)
        {
            builder.ToTable("TaskHistories");

            builder.HasKey(th => th.Id);

            builder.Property(th => th.OldStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(th => th.NewStatus)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(th => th.Note)
                .HasMaxLength(500);

            builder.Property(th => th.ChangedAt).IsRequired();

            builder.HasOne(th => th.Task)
                .WithMany(t => t.TaskHistories)
                .HasForeignKey(th => th.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(th => th.User)
                .WithMany(u => u.TaskHistories)
                .HasForeignKey(th => th.ChangedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
