namespace IKM_Retro.Exceptions.Base;

public class NotFoundException : Exception
{
    public NotFoundException()
    {
    }

    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public NotFoundException(string entityType, int id, string additionalInfo = "Please check correctness of entered ID") : base($"Sorry, but {entityType} with ID {id} not found. {additionalInfo}")
    {
    }
}