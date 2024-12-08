using Microsoft.AspNetCore.Builder;

namespace ToDo.Api.Extensions
{
  public static class MiddlewareExtensions
  {
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
      return app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
    }
  }
}
