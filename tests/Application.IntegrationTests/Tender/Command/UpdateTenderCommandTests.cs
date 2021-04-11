using System;
using System.Threading.Tasks;
using NUnit.Framework;
using TenderManagement.Application.Common.Exception;
using TenderEntity = TenderManagement.Domain.Entity.Tender;
using Request = TenderManagement.Application.Tender.Command.UpdateTenderCommand;
using RequestBuilder = TenderManagement.Application.IntegrationTests.Tender.TestBuilder.UpdateTenderCommandBuilder;
using static TenderManagement.Application.IntegrationTests.Testing;

namespace TenderManagement.Application.IntegrationTests.Tender.Command
{
    [TestFixture]
    public class UpdateTenderCommandTests : TestBase
    {
        private string _currentUser;

        [SetUp]
        public async Task Setup() => _currentUser = await RunAsDefaultUserAsync();

        [Test]
        public async Task WhenUpdatedThenItWillUseNewData()
        {
            var existing = (await GenerateSampleData(1))[0];
            Request request = RequestBuilder.New.WithId(existing.Id);

            await SendAsync(request);

            var result = await FindAsync<TenderEntity>(existing.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(existing.Id));
            Assert.That(result.Name, Is.EqualTo(request.Name));
            Assert.That(result.RefNumber, Is.EqualTo(request.RefNumber));
            Assert.That(result.Details, Is.EqualTo(request.Details));
            Assert.That(result.ClosingDate, Is.EqualTo(request.ClosingDate));
            Assert.That(result.ReleaseDate, Is.EqualTo(request.ReleaseDate));
        }

        [Test]
        public void WhenRequiredFieldsNotSetThenItWillThrowValidationError()
        {
            Request request = RequestBuilder.New.WithoutName().WithoutReferenceNumber().WithoutDetails();

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(request));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors.Keys,
                Is.SupersetOf(new[]
                    {nameof(TenderEntity.Name), nameof(TenderEntity.RefNumber), nameof(TenderEntity.Details)}));
            Assert.That(exception.Errors[nameof(TenderEntity.Name)], Does.Contain("'Name' must not be empty."));
            Assert.That(exception.Errors[nameof(TenderEntity.RefNumber)],
                Does.Contain("'Reference Number' must not be empty."));
            Assert.That(exception.Errors[nameof(TenderEntity.Details)], Does.Contain("'Details' must not be empty."));
        }

        [Test]
        public void WhenClosingOrReleaseDateNotInTheFutureThenItWillThrowValidationError()
        {
            var sameDay = CurrentDateTime.AddMinutes(1);
            Request request = RequestBuilder.New.WithReleaseDate(sameDay).WithClosingDate(sameDay);

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(request));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(TenderEntity.ReleaseDate)));
            Assert.That(exception.Errors[nameof(TenderEntity.ReleaseDate)],
                Does.Contain("Release date must be in the future."));
            Assert.That(exception.Errors, Does.ContainKey(nameof(TenderEntity.ClosingDate)));
            Assert.That(exception.Errors[nameof(TenderEntity.ClosingDate)],
                Does.Contain("Closing date must be in the future."));
        }

        [Test]
        public void WhenClosingDateNotLaterThanReleaseDateThenItWillThrowValidationError()
        {
            var tomorrow = CurrentDateTime.AddDays(1);
            var sameDayAsReleaseDate = tomorrow.AddMinutes(1);
            Request request = RequestBuilder.New.WithReleaseDate(tomorrow).WithClosingDate(sameDayAsReleaseDate);

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(request));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(TenderEntity.ClosingDate)));
            Assert.That(exception.Errors[nameof(TenderEntity.ClosingDate)],
                Does.Contain("Closing date must be later than release date."));
        }

        [Test]
        public async Task WhenUpdatedThenLastModifiedByWillBeCurrentUserAndLastModifiedWillBeNow()
        {
            var existing = (await GenerateSampleData(1))[0];
            Request request = RequestBuilder.New.WithId(existing.Id);
            var now = DateTime.Now.AddDays(1);
            CurrentDateTime = now;

            await SendAsync(request);

            var result = await FindAsync<TenderEntity>(existing.Id);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.LastModifiedBy, Is.EqualTo(_currentUser));
            Assert.That(result.LastModified, Is.Not.Null.And.EqualTo(now));
        }
    }
}