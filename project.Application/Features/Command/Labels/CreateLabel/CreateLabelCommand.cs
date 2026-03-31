using MediatR;
using project.Application.ModelsDto;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Command.Labels.CreateLabel
{
    public sealed record CreateLabelCommand(int GroupId, string Name, string Color) : IRequest<Result<LabelModel>>
    {
    }
}
