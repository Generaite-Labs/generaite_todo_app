using System;

namespace ToDo.Application.Exceptions
{
    public class UnauthorizedTodoItemAccessException : TodoItemServiceException
    {
        public UnauthorizedTodoItemAccessException(string userId, Guid todoItemId) 
            : base($"User {userId} is not authorized to access TodoItem with id {todoItemId}.") { }
    }
}