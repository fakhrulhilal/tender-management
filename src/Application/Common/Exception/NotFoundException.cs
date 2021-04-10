namespace TenderManagement.Application.Common.Exception
{
    /// <summary>
    /// Indicate that the item is not found in persistent storage of the application
    /// </summary>
    public class NotFoundException : BusinessApplicationException
    {
        public NotFoundException(string message)
            : base(message)
        { }

        public NotFoundException(string message, System.Exception innerException)
            : base(message, innerException)
        { }

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
        }
    }
}
