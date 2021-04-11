using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Imprise.MediatR.Extensions.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TenderManagement.Application.Common.Mappings;
using TenderManagement.Application.Common.Port;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.Application.Tender.Command
{
    public class UpdateTenderCommand : BaseTenderEntity, IRequest, IMapDef<Domain.Entity.Tender>
    {
        public int Id { get; init; }

        public class Validator : AbstractValidator<UpdateTenderCommand>
        {
            private readonly IApplicationDbContext _dbContext;

            public Validator(IApplicationDbContext dbContext)
            {
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
                RuleFor(p => p.Id).GreaterThan(0).MustAsync(Exists).WithMessage((_, id) => $"No Tender with ID {id}");
            }

            private async Task<bool>
                Exists(UpdateTenderCommand command, int arg2, CancellationToken cancellationToken) =>
                await _dbContext.Tenders.AsNoTracking().AnyAsync(t => t.Id == command.Id, cancellationToken);
        }

        public class Handler : IRequestHandler<UpdateTenderCommand>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Unit> Handle(UpdateTenderCommand command, CancellationToken cancellationToken)
            {
                var tender = await _dbContext.Tenders.FirstAsync(t => t.Id == command.Id, cancellationToken);
                _mapper.Map(command, tender);
                _dbContext.Tenders.Update(tender);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return  Unit.Value;
            }
        }

        public class CacheRemover : CacheInvalidator<UpdateTenderCommand, GetTenderDetailQuery, GetTenderDetailQuery.Response>
        {
            public CacheRemover(ICache<GetTenderDetailQuery, GetTenderDetailQuery.Response> cache) : base(cache)
            { }

            protected override string GetCacheKeyIdentifier(UpdateTenderCommand command) => command.Id.ToString();
        }
    }
}
