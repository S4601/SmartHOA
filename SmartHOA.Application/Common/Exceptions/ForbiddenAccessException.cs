namespace SmartHOA.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base("Нямате необходимите права за достъп до този ресурс.")
    {
    }

    public ForbiddenAccessException(string message) : base(message)
    {
    }
}
