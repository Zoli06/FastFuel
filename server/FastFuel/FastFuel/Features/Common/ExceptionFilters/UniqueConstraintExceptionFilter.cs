using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.ExceptionFilters;

public class UniqueConstraintExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        // TODO: We can do some logging here

        if (context.Exception is not UniqueConstraintException uniqueConstraintException)
            return Task.CompletedTask;

        var problem = new ProblemDetails
        {
            Title = uniqueConstraintException.Message,
            Detail =
                $"A record with the same field(s) '{string.Join(", ", uniqueConstraintException.ConstraintProperties)}' already exists",
            Status = StatusCodes.Status400BadRequest,
            Extensions =
            {
                { "table", uniqueConstraintException.SchemaQualifiedTableName },
                { "entries", uniqueConstraintException.ConstraintProperties }
            }
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}