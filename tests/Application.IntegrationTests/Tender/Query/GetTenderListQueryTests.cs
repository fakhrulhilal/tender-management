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
        public async Task ShouldReturnAllList()
        {
            const int total = 3;
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
    }
}
