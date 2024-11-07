using System.Globalization;
using ToDo.Domain.Interfaces;

namespace ToDo.Infrastructure.Context
{
    public class ApplicationContext : IApplicationContext
    {
        public string UserId { get; }
        public string CorrelationId { get; }
        public DateTime Timestamp { get; }
        public CultureInfo Culture { get; }

        public ApplicationContext(
            string userId,
            string correlationId = null,
            CultureInfo culture = null)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            CorrelationId = correlationId ?? Guid.NewGuid().ToString();
            Timestamp = DateTime.UtcNow;
            Culture = culture ?? CultureInfo.CurrentCulture;
        }
    }
} 