using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.ExceptionFilters;

public class InvalidOperationExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not InvalidOperationException invalidOperationException)
            return Task.CompletedTask;

        var problem = new ProblemDetails
        {
            Title = "Invalid operation",
            Detail = invalidOperationException.Message,
            Status = StatusCodes.Status400BadRequest
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}