namespace FastFuel.NSwag.SwaggerQueryParam;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerQueryParamAttribute(string name, Type? type = null, bool required = false) : Attribute
{
    public string Name { get; } = name;
    public Type Type { get; } = type ?? typeof(string);
    public bool Required { get; } = required;
}