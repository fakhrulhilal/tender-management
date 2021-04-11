using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.Application.IntegrationTests.Tender.Query
{
    using static Testing;

    [TestFixture]
    public class GetTenderListQueryTests : TestBase
    {
        [SetUp]
        public async Task Setup() => await RunAsDefaultUserAsync();

        [Test]
        public async Task WhenNotPagedThenItShouldReturnAll()
        {
            const int total = 10;
            var data = await GenerateSampleData(total);

            var result = await SendAsync(new GetTenderListQuery());

            Assert.That(result.Items.Count, Is.EqualTo(total));
            var expected = data.Select(d => new GetTenderListQuery.Response
            {
                Id = d.Id,
                Name = d.Name,
                RefNumber = d.RefNumber,
                ReleaseDate = d.ReleaseDate,
                ClosingDate = d.ClosingDate
            });
            Assert.That(result.Items, Is.EquivalentTo(expected).Using(ClassComparer.ByPublicProperty));
        }

        [Test]
        public async Task WhenPagedThenItShouldReturnExpectedPageSize()
        {
            const int total = 10;
            const int pageSize = total / 2;
            await GenerateSampleData(total);

            var result = await SendAsync(new GetTenderListQuery {PageSize = pageSize});

            Assert.That(result.Items.Count, Is.EqualTo(pageSize));
        }

        [Test]
        public async Task WhenPagedAndCurrentPageNotSpecifiedThenAssumedItIsFirstPage()
        {
            const int total = 10;
            const int pageSize = total / 2;
            await GenerateSampleData(total);

            var result = await SendAsync(new GetTenderListQuery {PageSize = pageSize});

            Assert.That(result.PageIndex, Is.EqualTo(1));
            Assert.That(result.HasNextPage, Is.True);
            Assert.That(result.HasPreviousPage, Is.False);
        }


        [Test]
        public async Task WhenPagedAndCurrentPageSpecifiedThenItShouldReturnGivenPage()
        {
            const int total = 10;
            const int pageSize = total / 2;
            const int requestedPage = 2;
            var data = await GenerateSampleData(total);

            var result = await SendAsync(new GetTenderListQuery {PageSize = pageSize, PageNumber = requestedPage});

            var expected = data.Skip(pageSize).Take(pageSize).Select(d => new GetTenderListQuery.Response
            {
                Id = d.Id,
                Name = d.Name,
                RefNumber = d.RefNumber,
                ReleaseDate = d.ReleaseDate,
                ClosingDate = d.ClosingDate
            });
            Assert.That(result.Items, Is.EquivalentTo(expected).Using(ClassComparer.ByPublicProperty));
            Assert.That(result.PageIndex, Is.EqualTo(requestedPage));
            Assert.That(result.HasNextPage, Is.False);
            Assert.That(result.HasPreviousPage, Is.True);
        }

        private async Task<List<Domain.Entity.Tender>> GenerateSampleData(int total)
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var dayAfterTomorrow = DateTime.Now.AddDays(2);
            var data = new List<Domain.Entity.Tender>();
            for (int i = 1; i <= total; i++)
            {
                data.Add(new Domain.Entity.Tender
                {
                    Id = i,
                    RefNumber = $"ref {i}",
                    Name = $"name {i}",
                    Details = $"details {i}",
                    ReleaseDate = tomorrow,
                    ClosingDate = dayAfterTomorrow
                });
            }

            await AddAsync(true, data.ToArray());
            return data;
        }
    }
}
