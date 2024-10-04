using System;
using System.Collections.Generic;

namespace ToDo.Application.Exceptions
{
    public class TodoItemValidationException : TodoItemServiceException
    {
        public IEnumerable<string> Errors { get; }

        public TodoItemValidationException(IEnumerable<string> errors) 
            : base("One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}