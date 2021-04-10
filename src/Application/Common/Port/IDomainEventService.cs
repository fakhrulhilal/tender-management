using System.Threading.Tasks;
using TenderManagement.Domain.Common;

namespace TenderManagement.Application.Common.Port
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
