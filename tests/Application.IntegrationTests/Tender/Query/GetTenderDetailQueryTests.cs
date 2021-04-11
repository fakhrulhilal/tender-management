using System.Threading.Tasks;
using NUnit.Framework;
using TenderManagement.Application.Common.Exception;
using TenderManagement.Application.Tender.Query;
using static TenderManagement.Application.IntegrationTests.Testing;

namespace TenderManagement.Application.IntegrationTests.Tender.Query
{
    [TestFixture]
    public class GetTenderDetailQueryTests : TestBase
    {
        [SetUp]
        public async Task Setup() => await RunAsDefaultUserAsync();

        [Test]
        public void WhenLookingForNonExistenceIdThenItWillThrowValidationError()
        {
            var query = new GetTenderDetailQuery { Id = int.MaxValue };

            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(query));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(GetTenderDetailQuery.Id)));
            Assert.That(exception.Errors[nameof(GetTenderDetailQuery.Id)],
                Is.EqualTo(new[] { $"No Tender with ID {query.Id}" }));
        }

        [Test]
        public async Task WhenLookingForExistenceIdThenItWillGiveTheDetail()
        {
            var data = (await GenerateSampleData(1))[0];

            var result = await SendAsync(new GetTenderDetailQuery {Id = data.Id});

            Assert.That(result, Is.Not.Null);
            var expected = new GetTenderDetailQuery.Response
            {
                Id = data.Id,
                Name = data.Name,
                RefNumber = data.RefNumber,
                Details = data.Details,
                ReleaseDate = data.ReleaseDate,
                ClosingDate = data.ClosingDate,
                CreatorId = data.CreatedBy
            };
            Assert.That(result, Is.EqualTo(expected).Using(ClassComparer.ByPublicProperty));
        }

        [Test]
        public void WhenInvalidIdPassedThenItWillThrowValidationError()
        {
            var exception = Assert.ThrowsAsync<ValidationException>(async () => await SendAsync(new GetTenderDetailQuery()));

            Assert.That(exception, Is.Not.Null);
            Assert.That(exception.Errors, Does.ContainKey(nameof(GetTenderDetailQuery.Id)));
            Assert.That(exception.Errors[nameof(GetTenderDetailQuery.Id)],
                Does.Contain($"'{nameof(GetTenderDetailQuery.Id)}' must be greater than '0'."));
        }
    }
}
