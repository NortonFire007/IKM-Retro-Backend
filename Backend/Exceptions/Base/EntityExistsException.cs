namespace AnimeWebApp.Exceptions.Base
{
    public class EntityExistsException : Exception
    {
        public EntityExistsException()
        {
        }

        public EntityExistsException(string message) : base(message)
        {
        }

        public EntityExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}