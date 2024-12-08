using System;

namespace ToDo.Application.Exceptions
{
  public class TodoItemOperationException : TodoItemServiceException
  {
    public string Operation { get; }

    public TodoItemOperationException(string operation, string reason)
        : base($"TodoItem operation '{operation}' failed: {reason}")
    {
      Operation = operation;
    }

    public TodoItemOperationException(string operation, string reason, Exception innerException)
        : base($"TodoItem operation '{operation}' failed: {reason}", innerException)
    {
      Operation = operation;
    }
  }
}
