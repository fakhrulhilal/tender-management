using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TenderManagement.Application.Common.Mappings;
using TenderManagement.Application.Common.Model;
using TenderManagement.Application.Common.Port;

namespace TenderManagement.Application.Tender.Query
{
    public class GetTenderListQuery : PagedQuery, IRequest<PagedList<GetTenderListQuery.Response>>
    {
        public class Response : IMapDef<Domain.Entity.Tender>
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string RefNumber { get; set; } = string.Empty;
            public DateTime ReleaseDate { get; set; }
            public DateTime ClosingDate { get; set; }
        }

        public class Handler : IRequestHandler<GetTenderListQuery, PagedList<Response>>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<PagedList<Response>> Handle(GetTenderListQuery query, CancellationToken cancellationToken) =>
                await _dbContext.Tenders.AsNoTracking().ProjectTo<Response>(_mapper.ConfigurationProvider)
                    .PagedListAsync(query.PageNumber ?? 1, query.PageSize ?? DefaultPagingSize);
        }
    }
}