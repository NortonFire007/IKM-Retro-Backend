using AnimeWebApp.Exceptions.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class ExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        ProblemDetails? problemDetails = null;
        int? statusCode;

        switch (exception)
        {
            case PermissionException:
                statusCode = StatusCodes.Status403Forbidden;
                break;

            case NotFoundException:
                statusCode = StatusCodes.Status404NotFound;
                break;

            case EntityExistsException:
                statusCode = StatusCodes.Status409Conflict;
                break;

            case BusinessException:
                statusCode = StatusCodes.Status400BadRequest;
                break;

            default:
                statusCode = StatusCodes.Status500InternalServerError;
                problemDetails = new()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Sorry, unexpected exception happened.",
                    Detail = "Please, try again later.",
                };

                break;
        }

        problemDetails ??= new()
        {
            Status = statusCode,
            Title = exception.Message,
        };

        httpContext.Response.StatusCode = statusCode.Value;

        return problemDetailsService.TryWriteAsync(new()
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
        });
    }
}