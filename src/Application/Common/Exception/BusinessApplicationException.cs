namespace TenderManagement.Application.Common.Exception
{
    /// <summary>
    /// Parent of all exception in application layer
    /// </summary>
    public abstract class BusinessApplicationException : System.Exception
    {
        protected BusinessApplicationException()
        { }

        protected BusinessApplicationException(string message) : base(message)
        { }

        protected BusinessApplicationException(string message, System.Exception innerException) 
            : base(message, innerException)
        { }
    }
}
