using System;

namespace ToDo.Application.Exceptions
{
    public class TodoItemNotFoundException : TodoItemServiceException
    {
        public TodoItemNotFoundException(int id) : base($"TodoItem with id {id} was not found.") { }
    }
}