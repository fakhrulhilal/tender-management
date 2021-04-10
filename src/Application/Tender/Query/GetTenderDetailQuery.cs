using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Imprise.MediatR.Extensions.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using TenderManagement.Application.Common.Mappings;
using TenderManagement.Application.Common.Port;

namespace TenderManagement.Application.Tender.Query
{
    public class GetTenderDetailQuery : IRequest<GetTenderDetailQuery.Response>
    {
        public GetTenderDetailQuery(int id) => Id = id;

        public int Id { get; }

        public class Response : BaseTenderEntity, IMapDef<Domain.Entity.Tender>
        {
            public int Id { get; set; }
            public string CreatorId { get; set; }

            public void Mapping(Profile profile)
            {
                profile.CreateMap<Domain.Entity.Tender, Response>()
                    .ForMember(dst => dst.CreatorId, opt => opt.MapFrom(src => src.CreatedBy));
            }
        }

        public class Validator : AbstractValidator<GetTenderDetailQuery>
        {
            private readonly IApplicationDbContext _dbContext;

            public Validator(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                RuleFor(p => p.Id).GreaterThan(0).MustAsync(Exists).WithMessage((_, id) => $"No Tender with ID {id}");
            }

            private async Task<bool> Exists(int id, CancellationToken cancellationToken) =>
                await _dbContext.Tenders.AsNoTracking().AnyAsync(t => t.Id == id, cancellationToken);
        }

        public class Handler : IRequestHandler<GetTenderDetailQuery, Response>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Response> Handle(GetTenderDetailQuery query, CancellationToken cancellationToken)
            {
                var tender = await _dbContext.Tenders.AsNoTracking().FirstAsync(t => t.Id == query.Id, cancellationToken);
                var response = _mapper.Map<Response>(tender);
                return response;
            }
        }

        public class CacheRegister : DistributedCache<GetTenderDetailQuery, Response>
        {
            public CacheRegister(IDistributedCache distributedCache) : base(distributedCache)
            { }

            protected override string GetCacheKeyIdentifier(GetTenderDetailQuery request) => request.Id.ToString();
        }
    }
}