namespace FastFuel.Features.Common.Exceptions.AppExceptions;

public class MissingRequiredFieldAppException(string fieldName) : AppException($"Missing required field: {fieldName}");