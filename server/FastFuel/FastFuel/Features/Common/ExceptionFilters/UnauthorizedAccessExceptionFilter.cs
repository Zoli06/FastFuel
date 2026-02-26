using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.ExceptionFilters;

public class UnauthorizedAccessExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not UnauthorizedAccessException unauthorizedAccessException)
            return Task.CompletedTask;

        var problem = new ProblemDetails
        {
            Title = unauthorizedAccessException.Message,
            Detail = "You do not have permission to perform this action",
            Status = StatusCodes.Status403Forbidden
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status403Forbidden
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}