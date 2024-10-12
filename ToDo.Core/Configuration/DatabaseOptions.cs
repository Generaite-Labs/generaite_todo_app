using System.ComponentModel.DataAnnotations;

namespace ToDo.Core.Configuration
{
    public class DatabaseOptions
    {
        public const string Database = "Database";

        [Required(ErrorMessage = "ConnectionString is required")]
        [MinLength(10, ErrorMessage = "ConnectionString must be at least 10 characters long")]
        public string ConnectionString { get; set; } = string.Empty;
    }
}