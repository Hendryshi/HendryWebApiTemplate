using Common.Domain.Common.Events;
using Common.Domain.Extensions;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }

        #region domain events
        private readonly List<BaseEvent> _domainEvents = new();

        [NotMapped]
        [JsonIgnore]
        public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

        public List<BaseEvent> GetDomainEvents() => _domainEvents;

        public void AddDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public BaseEvent NewEvent(EventAction action)
        {
            return this.NewEvent(Enum.GetName(action));
        }

        public BaseEvent NewEvent(string action)
        {
            return new BaseEvent()
            {
                Action = action,
                EntityId = this.Id,
                Data = this,
                CallerInfos = System.Environment.StackTrace.TrimStackTrace(),
                EntityType = this.GetType().FullName
            };
        }

        public void RemoveDomainEvent(BaseEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        #endregion
    }
}
