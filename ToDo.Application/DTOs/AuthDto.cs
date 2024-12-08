using System.ComponentModel.DataAnnotations;

namespace ToDo.Application.DTOs
{
  public class RegisterDto
  {
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string FullName { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }
  }

  public class LoginDto
  {
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public required string Password { get; set; }
  }

  public class LoginResponseDto
  {
    public required string Token { get; set; }
    public required string UserId { get; set; }
    public required string Email { get; set; }
  }

  public class RegisterResponseDto
  {
    public required string Message { get; set; }
  }
}
