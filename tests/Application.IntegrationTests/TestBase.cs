using NUnit.Framework;
using System.Threading.Tasks;

namespace TenderManagement.Application.IntegrationTests
{
    using static Testing;

    public abstract class TestBase
    {
        [SetUp]
        public async Task TestSetUp() => await ResetState();
    }
}
