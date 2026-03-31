using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.ModelsDto
{
    public class ReportModel
    {
        public int Id { get; private set; }
        public int GroupId { get; private set; }
        public int GeneratedBy { get; private set; }
        public string Title { get; private set; }
        public string? FilePath { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public ReportType Status { get; private set; }
    }
}
