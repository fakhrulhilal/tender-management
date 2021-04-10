using System;

namespace TenderManagement.Application.Common.Port
{
    public interface IDateTime
    {
        DateTime Now { get; }
    }
}
