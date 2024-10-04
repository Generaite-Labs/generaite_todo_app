using AutoMapper;
using ToDo.Domain.Entities;
using ToDo.Application.DTOs;

namespace ToDo.Application.Mappers
{
    public class TodoItemMappingProfile : Profile
    {
        public TodoItemMappingProfile()
        {
            CreateMap<TodoItem, TodoItemDto>();
            CreateMap<CreateTodoItemDto, TodoItem>();
            CreateMap<UpdateTodoItemDto, TodoItem>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}