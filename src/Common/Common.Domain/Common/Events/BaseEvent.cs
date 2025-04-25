using MediatR;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Domain.Common.Events
{
    [NotMapped]
    public class BaseEvent : INotification
    {
        // not type EventAction in case we want to use other undefined actions
        public string Action { get; set; }

        public Guid EntityId { get; set; }

        public string EntityType { get; set; }

        public string CallerInfos { get; set; }

        public BaseEntity Data { get; set; }
    }

    public enum EventAction
    {
        Create,
        Update,
        Delete
    }
}
