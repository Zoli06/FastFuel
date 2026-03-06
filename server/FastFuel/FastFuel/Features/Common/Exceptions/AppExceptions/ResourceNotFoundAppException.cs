namespace FastFuel.Features.Common.Exceptions.AppExceptions;

public class ResourceNotFoundAppException(string resourceName, object resourceId)
    : AppException($"Resource '{resourceName}' with ID '{resourceId}' not found");