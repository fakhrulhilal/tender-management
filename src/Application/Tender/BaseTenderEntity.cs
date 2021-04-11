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

        public abstract class BaseValidator<TTender> : AbstractValidator<TTender> where TTender : BaseTenderEntity
        {
            protected BaseValidator(IDateTime clock)
            {
                RuleFor(p => p.Name).NotNull().NotEmpty().MaximumLength(100);
                RuleFor(p => p.RefNumber).NotNull().NotEmpty().MaximumLength(50).WithName("Reference Number");
                RuleFor(p => p.Details).NotNull().NotEmpty().MaximumLength(500);
                RuleFor(p => p.ReleaseDate).GreaterThanOrEqualTo(clock.Now.Date.AddDays(1))
                    .WithMessage("Release date must be in the future.");
                RuleFor(p => p.ClosingDate)
                    .GreaterThanOrEqualTo(clock.Now.Date.AddDays(1)).WithMessage("Closing date must be in the future.")
                    .GreaterThanOrEqualTo(p => p.ReleaseDate.Date.AddDays(1)).WithMessage("Closing date must be later than release date.");
            }
        }
    }
}
