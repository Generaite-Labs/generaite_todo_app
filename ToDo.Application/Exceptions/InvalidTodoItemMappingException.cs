using System;

namespace ToDo.Application.Exceptions
{
    public class InvalidTodoItemMappingException : TodoItemServiceException
    {
        public InvalidTodoItemMappingException(string message) : base(message) { }

        public InvalidTodoItemMappingException(string message, Exception innerException) : base(message, innerException) { }
    }
}