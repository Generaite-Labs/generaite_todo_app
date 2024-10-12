namespace ToDo.Infrastructure.Configuration
{
    public class EmailConfiguration
    {
        public required string SmtpServer { get; set; }
        public required int SmtpPort { get; set; }
        public required string SmtpUsername { get; set; }
        public required string SmtpPassword { get; set; }
    }
}
