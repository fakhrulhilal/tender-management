using System.Collections.Generic;
using System.Linq;

namespace TenderManagement.Application.Common.Model
{
    public class Result
    {
        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; }

        public string[] Errors { get; }

        public static Result Success() => new(true, new string[] { });

        public static Result Failure(IEnumerable<string> errors) => new(false, errors);
    }
}
