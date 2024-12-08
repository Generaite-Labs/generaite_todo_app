using System;

namespace ToDo.Application.Exceptions
{
  public class TodoItemServiceException : Exception
  {
    public TodoItemServiceException() : base() { }

    public TodoItemServiceException(string message) : base(message) { }

    public TodoItemServiceException(string message, Exception innerException) : base(message, innerException) { }
  }
}
