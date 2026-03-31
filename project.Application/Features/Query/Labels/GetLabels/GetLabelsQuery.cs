using MediatR;
using project.Application.ModelsDto;
using project.Domain.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application.Features.Query.Labels.GetLabels
{
    public sealed record GetLabelsQuery(int GroupId) : IRequest<Result<List<LabelModel>>> { }
}
