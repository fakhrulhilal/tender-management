using FluentValidation;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using TenderManagement.Application.Common.Mappings;
using TenderManagement.Application.Common.Port;
using TenderManagement.Domain.Event;

namespace TenderManagement.Application.Tender.Command
{
    public class CreateTenderCommand : BaseTenderEntity, IRequest<CreateTenderCommand.Response>, IMapDef<Domain.Entity.Tender>
    {
        public class Response
        {
            public int CreatedId { get; init; }
        }

        public class Validator : AbstractValidator<CreateTenderCommand>
        {
            public Validator(IDateTime clock) => Include(new BaseValidator(clock));
        }

        public class Handler : IRequestHandler<CreateTenderCommand, Response>
        {
            private readonly IApplicationDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(IApplicationDbContext dbContext, IMapper mapper)
            {
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
                _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            }

            public async Task<Response> Handle(CreateTenderCommand command, CancellationToken cancellationToken)
            {
                var tender = _mapper.Map<Domain.Entity.Tender>(command);
                tender.DomainEvents.Add(new TenderCreatedEvent(tender));
                _dbContext.Tenders.Add(tender);
                await _dbContext.SaveChangesAsync(cancellationToken);
                return new Response {CreatedId = tender.Id};
            }
        }
    }
}
