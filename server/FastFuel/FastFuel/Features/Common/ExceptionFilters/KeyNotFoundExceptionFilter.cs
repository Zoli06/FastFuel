using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.ExceptionFilters;

public class KeyNotFoundExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not KeyNotFoundException keyNotFoundException)
            return Task.CompletedTask;

        var problem = new ProblemDetails
        {
            Title = "Resource not found",
            Detail = keyNotFoundException.Message,
            Status = StatusCodes.Status404NotFound
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status404NotFound
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}