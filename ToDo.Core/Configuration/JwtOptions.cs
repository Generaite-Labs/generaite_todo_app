using System.ComponentModel.DataAnnotations;

namespace ToDo.Core.Configuration
{
  public class JwtOptions
  {
    public const string Jwt = "Jwt";

    [Required(ErrorMessage = "JWT Key is required")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT Issuer is required")]
    public string Issuer { get; set; } = string.Empty;

    [Required(ErrorMessage = "JWT Audience is required")]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 365, ErrorMessage = "JWT ExpireDays must be between 1 and 365")]
    public int ExpireDays { get; set; } = 30;
  }
}
