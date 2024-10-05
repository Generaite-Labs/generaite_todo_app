using System.ComponentModel.DataAnnotations;

namespace ToDo.Core.Configuration
{
    public class ApplicationSettings
    {
        public const string Application = "Application";

        [Required(ErrorMessage = "ApplicationName is required")]
        public string ApplicationName { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "MaxTodoItems must be between 1 and 100")]
        public int MaxTodoItems { get; set; } = 50;

        [Required(ErrorMessage = "LogLevel is required")]
        public string LogLevel { get; set; } = "Information";
    }
}