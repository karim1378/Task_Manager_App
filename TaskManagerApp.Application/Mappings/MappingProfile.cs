using AutoMapper;
using TaskManagerApp.Application.DTOs;
using TaskManagerApp.Domain.Entities;
using TaskManagerApp.TaskManagerApp.Application.DTOs;
using Task = TaskManagerApp.Domain.Entities.Task;

namespace TaskManagerApp.Infrastructure.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProfileDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, GetProfileDto>();

            CreateMap<User, GetUserDto>();


            CreateMap<CreateTaskDto, Task>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateTaskDto, Task>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Task, GetTaskDto>();

            

            CreateMap<CreateProjectDto, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<UpdateProjectDto, Project>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Project, GetProjectDto>();
                
           
        }
    }
}

