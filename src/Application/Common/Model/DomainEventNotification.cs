using MediatR;
using TenderManagement.Domain.Common;

namespace TenderManagement.Application.Common.Model
{
    public class DomainEventNotification<TDomainEvent> : INotification where TDomainEvent : DomainEvent
    {
        public DomainEventNotification(TDomainEvent domainEvent) => DomainEvent = domainEvent;

        public TDomainEvent DomainEvent { get; }
    }
}
