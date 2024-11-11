using System;

namespace ToDo.Application.Exceptions
{
    public class TodoItemNotFoundException : TodoItemServiceException
    {
        public TodoItemNotFoundException(Guid id) : base($"TodoItem with id {id} was not found.") { }
    }
}