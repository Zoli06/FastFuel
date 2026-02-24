using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastFuel.Features.Common.ExceptionFilters;

public class ReferenceConstraintExceptionFilter : ExceptionFilterAttribute
{
    public override Task OnExceptionAsync(ExceptionContext context)
    {
        if (context.Exception is not ReferenceConstraintException referenceConstraintException)
            return Task.CompletedTask;

        var problem = new ProblemDetails
        {
            Title = referenceConstraintException.Message,
            Detail =
                $"A record references a foreign key field(s) '{string.Join(", ", referenceConstraintException.ConstraintProperties)}' that does not exist",
            Status = StatusCodes.Status409Conflict,
            Extensions =
            {
                { "table", referenceConstraintException.SchemaQualifiedTableName },
                { "entries", referenceConstraintException.ConstraintProperties }
            }
        };

        context.Result = new ObjectResult(problem)
        {
            StatusCode = StatusCodes.Status409Conflict
        };

        context.ExceptionHandled = true;

        return Task.CompletedTask;
    }
}