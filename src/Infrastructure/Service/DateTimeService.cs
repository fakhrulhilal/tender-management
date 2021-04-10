using System;
using TenderManagement.Application.Common.Port;

namespace TenderManagement.Infrastructure.Service
{
    public class DateTimeService : IDateTime
    {
        public DateTime Now => DateTime.Now;
    }
}
