using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Domain.Events
{
    [NotMapped]
    public abstract class DomainEvent
    {
        public DateTime OccurredOn { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
        }
    }
}