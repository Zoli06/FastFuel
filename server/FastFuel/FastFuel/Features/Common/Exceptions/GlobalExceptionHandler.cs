using FastFuel.Features.Common.Exceptions.AppExceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FastFuel.Features.Common.Exceptions;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger,
    IProblemDetailsService problemDetailsService
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Handling exception: {Name} - {ExceptionMessage}", exception.GetType().Name,
                exception.Message);

        var statusCode = exception switch
        {
            // EF Core exceptions
            DbUpdateException => StatusCodes.Status400BadRequest,
            // Specified app exceptions
            MissingRequiredFieldAppException => StatusCodes.Status400BadRequest,
            ResourceNotFoundAppException => StatusCodes.Status404NotFound,
            UnauthorizedAppException => StatusCodes.Status401Unauthorized,
            // Other app exceptions
            AppException => StatusCodes.Status400BadRequest,
            // Other exceptions
            _ => StatusCodes.Status500InternalServerError
        };

        var problem = new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = exception.Message,
                Status = statusCode
            }
        };

        httpContext.Response.StatusCode = statusCode;
        return await problemDetailsService.TryWriteAsync(problem);
    }
}