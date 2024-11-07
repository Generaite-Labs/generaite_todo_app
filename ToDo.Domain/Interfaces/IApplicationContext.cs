namespace ToDo.Domain.Interfaces
{
    /// <summary>
    /// Provides context information for the current execution scope
    /// </summary>
    public interface IApplicationContext
    {
        /// <summary>
        /// The ID of the user performing the action
        /// </summary>
        string UserId { get; }

        /// <summary>
        /// A unique identifier for correlating related operations
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// The timestamp when this context was created
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The culture/locale for the current context
        /// </summary>
        System.Globalization.CultureInfo Culture { get; }
    }
} 