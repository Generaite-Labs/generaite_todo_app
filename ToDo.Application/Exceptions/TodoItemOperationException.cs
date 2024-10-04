using System;

namespace ToDo.Application.Exceptions
{
    public class TodoItemOperationException : TodoItemServiceException
    {
        public TodoItemOperationException(string operation, string reason) 
            : base($"TodoItem operation '{operation}' failed: {reason}") { }

        public TodoItemOperationException(string operation, string reason, Exception innerException) 
            : base($"TodoItem operation '{operation}' failed: {reason}", innerException) { }
    }
}