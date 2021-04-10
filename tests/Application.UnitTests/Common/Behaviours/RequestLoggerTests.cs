using System;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;
using TenderManagement.Application.Common.Behaviour;
using TenderManagement.Application.Common.Port;
using TenderManagement.Application.Tender.Command;

namespace TenderManagement.Application.UnitTests.Common.Behaviours
{
    public class RequestLoggerTests
    {
        private readonly Mock<ILogger<CreateTenderCommand>> _logger;
        private readonly Mock<ICurrentUserService> _currentUserService;
        private readonly Mock<IIdentityService> _identityService;


        public RequestLoggerTests()
        {
            _logger = new Mock<ILogger<CreateTenderCommand>>();

            _currentUserService = new Mock<ICurrentUserService>();

            _identityService = new Mock<IIdentityService>();
        }

        [Test]
        public async Task ShouldCallGetUserNameAsyncOnceIfAuthenticated()
        {
            _currentUserService.Setup(x => x.UserId).Returns("Administrator");

            var requestLogger = new LoggingBehaviour<CreateTenderCommand>(_logger.Object, _currentUserService.Object, _identityService.Object);

            var command = new CreateTenderCommand { RefNumber = "ref", Name = "name", Details = "detail", ReleaseDate = DateTime.Now.AddDays(1), ClosingDate = DateTime.Now.AddDays(2) };
            await requestLogger.Process(command, new CancellationToken());

            _identityService.Verify(i => i.GetUserNameAsync(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task ShouldNotCallGetUserNameAsyncOnceIfUnauthenticated()
        {
            var requestLogger = new LoggingBehaviour<CreateTenderCommand>(_logger.Object, _currentUserService.Object, _identityService.Object);

            var command = new CreateTenderCommand { RefNumber = "ref", Name = "name", Details = "detail", ReleaseDate = DateTime.Now.AddDays(1), ClosingDate = DateTime.Now.AddDays(2) };
            await requestLogger.Process(command, new CancellationToken());

            _identityService.Verify(i => i.GetUserNameAsync(null), Times.Never);
        }
    }
}
