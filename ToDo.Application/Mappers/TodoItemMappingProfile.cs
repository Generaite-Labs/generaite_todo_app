using AutoMapper;
using ToDo.Domain.Entities;
using ToDo.Application.DTOs;
using ToDo.Domain.ValueObjects;

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

            // Update mappings for TodoItemStatus
            CreateMap<TodoItemStatus, TodoItemStatus>().ConvertUsing((src, dest) => src);
            CreateMap<TodoItemStatus?, TodoItemStatus>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<TodoItemStatus, TodoItemStatus?>().ConvertUsing((src, dest) => src);
        }
    }
}