using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartHOA.Application.Common.Exceptions;

namespace SmartHOA.Api.ExceptionHandlers;

public class CustomExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CustomExceptionHandler> _logger;

    public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ProblemDetails problemDetails;
        int statusCode;

        switch (exception)
        {
            case ValidationException validationException:
                var errors = validationException.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray());

                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ValidationProblemDetails(errors)
                {
                    Status = statusCode,
                    Title = "Възникна грешка при валидацията на данните.",
                    Detail = "Моля, проверете изпратените данни и опитайте отново."
                };
                break;

            case BadRequestException badRequestException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Невалидна заявка.",
                    Detail = badRequestException.Message
                };
                break;

            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Ресурсът не е намерен.",
                    Detail = notFoundException.Message
                };
                break;

            case ForbiddenAccessException forbiddenAccessException:
                statusCode = StatusCodes.Status403Forbidden;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Отказан достъп.",
                    Detail = forbiddenAccessException.Message
                };
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                statusCode = StatusCodes.Status401Unauthorized;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Неоторизиран достъп.",
                    Detail = unauthorizedAccessException.Message
                };
                break;

            case InvalidOperationException invalidOperationException:
                statusCode = StatusCodes.Status400BadRequest;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Невалидна операция.",
                    Detail = invalidOperationException.Message
                };
                break;

            default:
                _logger.LogError(exception, "Необработена грешка при изпълнение на заявка: {Message}", exception.Message);
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = "Вътрешна грешка на сървъра.",
                    Detail = "Възникна неочаквана грешка. Моля, свържете се с администратора."
                };
                break;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}