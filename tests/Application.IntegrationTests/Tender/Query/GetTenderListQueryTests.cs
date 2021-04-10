using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using TenderManagement.Application.Tender.Query;

namespace TenderManagement.Application.IntegrationTests.Tender.Query
{
    using static Testing;

    public class GetTenderListQueryTests : TestBase
    {
        [Test]
        public async Task ShouldReturnAllList()
        {
            var tomorrow = DateTime.Now.AddDays(1);
            var dayAfterTomorrow = DateTime.Now.AddDays(2);
            var data = new List<Domain.Entity.Tender>();
            for (int i = 1; i <= 3; i++)
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

            foreach (var tender in data)
            {
                await AddAsync(tender);
            }

            var result = await SendAsync(new GetTenderListQuery());

            Assert.That(result.Items.Count, Is.EqualTo(3));
            Assert.That(result.Items, Is.EquivalentTo(data).Using(ClassComparer.ByPublicProperty));
        }
    }
}
