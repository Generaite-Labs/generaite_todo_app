namespace ToDo.Domain.Interfaces {
    /// <summary>
    /// Coordinates database operations and domain event dispatching within a single transaction boundary.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Saves all changes and dispatches collected domain events within a transaction.
        /// </summary>
        /// <param name="cancellationToken">Optional cancellation token.</param>
        /// <returns>True if any changes were saved, false otherwise.</returns>
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

}