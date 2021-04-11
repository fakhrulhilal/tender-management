using NUnit.Framework;
using System.Threading.Tasks;
using TenderManagement.Application.Common.Exception;
using TenderManagement.Application.Tender.Command;
using TenderManagement.Application.Tender.Query;
using static TenderManagement.Application.IntegrationTests.Testing;

namespace TenderManagement.Application.IntegrationTests.Tender.Command
{
    [TestFixture]
    public class DeleteTenderCommandTests : TestBase
    {
        [SetUp]
        public async Task Setup() => await RunAsDefaultUserAsync();

        [Test]
        public void WhenDeletingNonExistenceIdThenItWillThrowValidationError()
        {
            var query = new DeleteTenderCommand { Id = int.MaxValue };

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(query));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(DeleteTenderCommand.Id)));
            Assert.That(exception.Errors[nameof(DeleteTenderCommand.Id)],
                Is.EqualTo(new[] { $"No Tender with ID {query.Id}" }));
        }

        [Test]
        public async Task WhenDeletingExistenceIdThenKeepInPersistenceStorageAndFlagAsDeleted()
        {
            var data = (await GenerateSampleData(1))[0];

            await SendAsync(new DeleteTenderCommand { Id = data.Id });

            var existing = await FindWithoutFilterAsync<Domain.Entity.Tender>(t => t.Id == data.Id);
            Assert.That(existing, Is.Not.Null);
            Assert.That(existing.IsDeleted, Is.True);
        }

        [Test]
        public async Task WhenDeletedThenItWilNotBeFound()
        {
            var data = (await GenerateSampleData(1))[0];

            await SendAsync(new DeleteTenderCommand { Id = data.Id });

            var exception =
                Assert.ThrowsAsync<ValidationException>(async () =>
                    await SendAsync(new GetTenderDetailQuery {Id = data.Id}));
            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(GetTenderDetailQuery.Id)));
            Assert.That(exception.Errors[nameof(GetTenderDetailQuery.Id)],
                Is.EqualTo(new[] {$"No Tender with ID {data.Id}"}));
        }

        [Test]
        public void WhenInvalidIdPassedThenItWillThrowValidationError()
        {
            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new DeleteTenderCommand()));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(DeleteTenderCommand.Id)));
            Assert.That(exception.Errors[nameof(DeleteTenderCommand.Id)],
                Does.Contain($"'{nameof(DeleteTenderCommand.Id)}' must be greater than '0'."));
        }
    }
}
