using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace TenderManagement.Application.Common.Exception
{
    /// <summary>
    /// Indicate that there's invalid data passed to the application before processing next step
    /// </summary>
    public class ValidationException : BusinessApplicationException
    {
        public ValidationException()
            : base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }

        /// <summary>
        /// All errors where the key is the related data/property of class
        /// </summary>
        public IDictionary<string, string[]> Errors { get; }
    }
}