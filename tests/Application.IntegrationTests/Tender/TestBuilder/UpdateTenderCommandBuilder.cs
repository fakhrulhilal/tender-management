using System;
using TenderManagement.Application.Tender.Command;

namespace TenderManagement.Application.IntegrationTests.Tender.TestBuilder
{
    public class UpdateTenderCommandBuilder
    {
        private readonly UpdateTenderCommand _inner = new()
        {
            Name = "updated name",
            Details = "updated details",
            RefNumber = "updated ref",
            ReleaseDate = DateTime.Now.AddMonths(1),
            ClosingDate = DateTime.Now.AddMonths(1).AddDays(1)
        };
        private UpdateTenderCommandBuilder()
        { }

        public static implicit operator UpdateTenderCommand(UpdateTenderCommandBuilder builder) => builder._inner;
        public static UpdateTenderCommandBuilder New => new();

        public UpdateTenderCommandBuilder WithReleaseDate(DateTime releaseDate)
        {
            _inner.ReleaseDate = releaseDate;
            return this;
        }

        public UpdateTenderCommandBuilder WithClosingDate(DateTime closingDate)
        {
            _inner.ClosingDate = closingDate;
            return this;
        }

        public UpdateTenderCommandBuilder WithoutName()
        {
            _inner.Name = string.Empty;
            return this;
        }

        public UpdateTenderCommandBuilder WithoutReferenceNumber()
        {
            _inner.RefNumber = string.Empty;
            return this;
        }

        public UpdateTenderCommandBuilder WithoutDetails()
        {
            _inner.Details = string.Empty;
            return this;
        }

        public UpdateTenderCommandBuilder WithId(int id)
        {
            _inner.Id = id;
            return this;
        }
    }
}