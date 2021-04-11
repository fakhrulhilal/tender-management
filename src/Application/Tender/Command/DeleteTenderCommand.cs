using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Imprise.MediatR.Extensions.Caching;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TenderManagement.Application.Common.Port;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.Application.Tender.Command
{
    public class DeleteTenderCommand : IRequest
    {
        public int Id { get; init; }

        public class Validator : AbstractValidator<DeleteTenderCommand>
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

        public class Handler : IRequestHandler<DeleteTenderCommand>
        {
            private readonly IApplicationDbContext _dbContext;

            public Handler(IApplicationDbContext dbContext) =>
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            public async Task<Unit> Handle(DeleteTenderCommand request, CancellationToken cancellationToken)
            {
                var tender = await _dbContext.Tenders.FirstAsync(t => t.Id == request.Id, cancellationToken);
                _dbContext.Tenders.Remove(tender);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return Unit.Value;
            }
        }

        public class CacheRemover : CacheInvalidator<DeleteTenderCommand, GetTenderDetailQuery, GetTenderDetailQuery.Response>
        {
            public CacheRemover(ICache<GetTenderDetailQuery, GetTenderDetailQuery.Response> cache) : base(cache)
            { }

            protected override string GetCacheKeyIdentifier(DeleteTenderCommand command) => command.Id.ToString();
        }
    }
}