namespace FinancialServices.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RouteBuilderConfigurationAttribute() : Attribute
    {
        public int Version { get; set; } = 1;
        public string Path { get; set; } = "";
        public string RouteGroup { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
