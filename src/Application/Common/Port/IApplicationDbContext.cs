using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TenderManagement.Application.Common.Port
{
    public interface IApplicationDbContext
    {
        DbSet<Domain.Entity.Tender> Tenders { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
