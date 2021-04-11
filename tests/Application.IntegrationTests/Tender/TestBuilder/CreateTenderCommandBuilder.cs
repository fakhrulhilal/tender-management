using System;
using TenderManagement.Application.Tender.Command;

namespace TenderManagement.Application.IntegrationTests.Tender.TestBuilder
{
    public class CreateTenderCommandBuilder
    {
        private readonly CreateTenderCommand _inner = new()
        {
            Name = "name",
            Details = "details",
            RefNumber = "ref",
            ReleaseDate = DateTime.Now.AddDays(1),
            ClosingDate = DateTime.Now.AddDays(2)
        };
        private CreateTenderCommandBuilder()
        { }

        public static implicit operator CreateTenderCommand(CreateTenderCommandBuilder builder) => builder._inner;
        public static CreateTenderCommandBuilder New => new();

        public CreateTenderCommandBuilder WithReleaseDate(DateTime releaseDate)
        {
            _inner.ReleaseDate = releaseDate;
            return this;
        }

        public CreateTenderCommandBuilder WithClosingDate(DateTime closingDate)
        {
            _inner.ClosingDate = closingDate;
            return this;
        }

        public CreateTenderCommandBuilder WithoutName()
        {
            _inner.Name = string.Empty;
            return this;
        }

        public CreateTenderCommandBuilder WithoutReferenceNumber()
        {
            _inner.RefNumber = string.Empty;
            return this;
        }

        public CreateTenderCommandBuilder WithoutDetails()
        {
            _inner.Details = string.Empty;
            return this;
        }
    }
}