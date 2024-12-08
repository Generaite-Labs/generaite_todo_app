using System.Net;
using System.Text.Json;
using Serilog;

namespace ToDo.Api
{
  public class GlobalExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        await HandleExceptionAsync(context, ex);
      }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
      context.Response.ContentType = "application/json";
      var response = context.Response;

      var errorResponse = new ErrorResponse
      {
        Success = false,
        Message = null  // Initialize as null
      };

      switch (exception)
      {
        case CustomException ex:
          response.StatusCode = ex.StatusCode;
          errorResponse.Message = ex.Message;
          break;
        case ApplicationException ex:
          response.StatusCode = (int)HttpStatusCode.BadRequest;
          errorResponse.Message = ex.Message;
          break;
        default:
          response.StatusCode = (int)HttpStatusCode.InternalServerError;
          errorResponse.Message = "Internal server error. Please try again later.";
          break;
      }

      Log.Error(exception, "An error occurred: {ErrorMessage}", errorResponse.Message);

      var result = JsonSerializer.Serialize(errorResponse);
      return context.Response.WriteAsync(result);
    }
  }

  public class ErrorResponse
  {
    public bool Success { get; set; }
    public string? Message { get; set; }  // Make Message nullable
  }
}
