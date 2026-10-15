namespace SmartHOA.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string name, object key)
        : base($"Обектът \"{name}\" ({key}) не беше намерен.")
    {
    }
}
