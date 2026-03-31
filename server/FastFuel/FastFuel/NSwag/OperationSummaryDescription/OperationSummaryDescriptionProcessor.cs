using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Contexts;

namespace FastFuel.NSwag.OperationSummaryDescription;

public class OperationSummaryDescriptionProcessor : IOperationProcessor
{
    public bool Process(OperationProcessorContext context)
    {
        var operation = context.OperationDescription.Operation;

        if (string.IsNullOrWhiteSpace(operation.Description) && !string.IsNullOrWhiteSpace(operation.Summary))
            operation.Description = operation.Summary;

        return true;
    }
}