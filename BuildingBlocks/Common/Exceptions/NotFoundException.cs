namespace BuildingBlocks.Common.Exceptions;

public class NotFoundException: Exception
{
    public bool Succeeded { get; } = false;

    public new string Message { get; }

    public NotFoundException()
        : base()
    {
        Message = "Entity was not found.";
    }

    public NotFoundException(string message)
        : base(message)
    {
        Message = message;
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
        Message = message;
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
        Message = $"Entity \"{name}\" ({key}) was not found.";
    }
}

