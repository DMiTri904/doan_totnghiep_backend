using AutoMapper;
using project.Application.ModelsDto;
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

            CreateMap<WorkTask, TaskModel>()
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(d => d.Priority, opt => opt.MapFrom(src => src.Priority.ToString()))
                .ForMember(d => d.IsAssigned, opt => opt.MapFrom(src => src.IsAssigned()))
                .ForMember(d => d.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()))
                .ForMember(d => d.Duration, opt => opt.MapFrom(src => src.Duration));
                

            CreateMap<Groups, GroupDetailModel>()
                .ForMember(d => d.ActiveMemberCount, opt => opt.MapFrom(src => src.ActiveMemberCount()))
                .ForMember(d => d.TotalTaskCount, opt => opt.MapFrom(src => src.PendingTaskCount()));

            CreateMap<GroupMem, GroupMemModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.User.AvatarUrl))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.UserCode));

            CreateMap<Groups, GroupDetailListModel>()
                .IncludeBase<Groups, GroupDetailModel>()
                .ForMember(d => d.Members, opt => opt.MapFrom(src => src.Members.Where(m => m.IsActive)));

            CreateMap<Groups, GroupModel>()
                .ForMember(d => d.TotalMemberCount, opt => opt.MapFrom(src => src.MemberCount()));

            CreateMap<Comment, CommentModel>()
                .ForMember(d => d.User, opt => opt.MapFrom(src => src.User));

            CreateMap<UserApp, CommentUserModel>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(d => d.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));
                

            CreateMap<Notification, NotificationModel>();
        }
    }
}
