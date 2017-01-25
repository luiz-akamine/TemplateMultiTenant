namespace TemplateMultiTenant.Domain.Models
{
    public class RequestBase
    {
        public string MethodName { get; set; }
        public object Params { get; set; }
    }
}