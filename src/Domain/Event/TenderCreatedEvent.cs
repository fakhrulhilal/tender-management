using System;
using TenderManagement.Domain.Common;
using TenderManagement.Domain.Entity;

namespace TenderManagement.Domain.Event
{
    public class TenderCreatedEvent : DomainEvent
    {
        public Tender Tender { get; }

        public TenderCreatedEvent(Tender tender) => Tender = tender ?? throw new ArgumentNullException(nameof(tender));
    }
}
