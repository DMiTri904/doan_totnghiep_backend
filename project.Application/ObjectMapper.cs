using AutoMapper;
using project.Application.ModelsDto.DomainModelsDto;
using project.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project.Application
{
    public class ObjectMapper : Profile
    {
        public ObjectMapper()
        {
            CreateMap<UserApp,UserModel>().ReverseMap();
        }
    }
}
