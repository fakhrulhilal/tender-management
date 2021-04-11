using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using TenderManagement.Application.Common.Exception;
using TenderManagement.Application.Common.Model;
using TenderManagement.Domain.Event;
using TenderEntity = TenderManagement.Domain.Entity.Tender;
using Request = TenderManagement.Application.Tender.Command.CreateTenderCommand;
using RequestBuilder = TenderManagement.Application.IntegrationTests.Tender.TestBuilder.CreateTenderCommandBuilder;

namespace TenderManagement.Application.IntegrationTests.Tender.Command
{
    using static Testing;

    [TestFixture]
    public class CreateTenderCommandTests : TestBase
    {
        private string _currentUser;

        [SetUp]
        public async Task Setup() => _currentUser = await RunAsDefaultUserAsync();

        [Test]
        public async Task WhenCreatedThenItShouldBeAvailable()
        {
            Request request = RequestBuilder.New;

            var response = await SendAsync(request);

            var result = await FindAsync<TenderEntity>(response.CreatedId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.GreaterThan(0));
            Assert.That(result.Name, Is.EqualTo(result.Name));
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
            Assert.That(exception.Errors[nameof(TenderEntity.RefNumber)], Does.Contain("'Reference Number' must not be empty."));
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
            Assert.That(exception.Errors[nameof(TenderEntity.ReleaseDate)], Does.Contain("Release date must be in the future."));
            Assert.That(exception.Errors, Does.ContainKey(nameof(TenderEntity.ClosingDate)));
            Assert.That(exception.Errors[nameof(TenderEntity.ClosingDate)], Does.Contain("Closing date must be in the future."));
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
            Assert.That(exception.Errors[nameof(TenderEntity.ClosingDate)], Does.Contain("Closing date must be later than release date."));
        }

        [Test]
        public async Task WhenCreatedThenCreatedByWillBeCurrentUserAndCreatedWillBeNow()
        {
            Request request = RequestBuilder.New;
            var now = CurrentDateTime;
            var response = await SendAsync(request);

            var result = await FindAsync<TenderEntity>(response.CreatedId);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.CreatedBy, Is.EqualTo(_currentUser));
            Assert.That(result.Created, Is.EqualTo(now));
        }

        [Test]
        public async Task WhenCreatedThenItWillTriggerEvent()
        {
            Request request = RequestBuilder.New;
            var eventHandler = new Mock<INotificationHandler<DomainEventNotification<TenderCreatedEvent>>>();
            TenderCreatedEvent triggeredEvent = default;
            eventHandler
                .Setup(x => x.Handle(It.IsAny<DomainEventNotification<TenderCreatedEvent>>(),
                    It.IsAny<CancellationToken>()))
                .Callback((DomainEventNotification<TenderCreatedEvent> eventData, CancellationToken _) => triggeredEvent = eventData.DomainEvent);
            ServiceCollection.AddTransient(_ => eventHandler.Object);
            using var scope = ServiceCollection.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var mediator = serviceProvider.GetRequiredService<IMediator>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();

            await mediator.Send(request);

            Assert.That(triggeredEvent, Is.Not.Null);
            var expected = mapper.Map<TenderEntity>(request);
            Assert.That(triggeredEvent.Tender,
                Is.EqualTo(expected).Using(ClassComparer.ByPublicPropertyExcept(nameof(TenderEntity.Id),
                    nameof(TenderEntity.Created), nameof(TenderEntity.CreatedBy), nameof(TenderEntity.LastModified),
                    nameof(TenderEntity.LastModifiedBy), nameof(TenderEntity.DomainEvents))));
        }
    }
}
