namespace Datamarka_MVC.Contracts.Exceptions
{
    public class DbInitializationException : Exception
    {
        public DbInitializationException(string? message) : base(message)
        {
        }

        public DbInitializationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
