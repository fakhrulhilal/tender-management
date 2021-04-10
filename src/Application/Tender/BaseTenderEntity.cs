using FluentValidation;
using System;
using TenderManagement.Application.Common.Port;

namespace TenderManagement.Application.Tender
{
    public abstract class BaseTenderEntity
    {
        public string Name { get; set; } = string.Empty;
        public string RefNumber { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public DateTime ClosingDate { get; set; }

        public class BaseValidator : AbstractValidator<BaseTenderEntity>
        {
            public BaseValidator(IDateTime clock)
            {
                RuleFor(p => p.Name).NotNull().NotEmpty().MaximumLength(100);
                RuleFor(p => p.RefNumber).NotNull().NotEmpty().MaximumLength(50);
                RuleFor(p => p.Details).NotNull().NotEmpty().MaximumLength(500);
                RuleFor(p => p.ReleaseDate).GreaterThan(clock.Now);
                RuleFor(p => p.ClosingDate).GreaterThan(clock.Now).GreaterThan(p => p.ReleaseDate.Date);
            }
        }
    }
}
