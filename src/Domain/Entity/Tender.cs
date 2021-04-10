using System;
using System.Collections.Generic;
using TenderManagement.Domain.Common;

namespace TenderManagement.Domain.Entity
{
    public class Tender : AuditableEntity, IHasDomainEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string RefNumber { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public DateTime ClosingDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<DomainEvent> DomainEvents { get; } = new();
    }
}
