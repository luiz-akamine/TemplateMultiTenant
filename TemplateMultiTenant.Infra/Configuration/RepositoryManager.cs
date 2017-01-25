using System.Web;
using TemplateMultiTenant.Infra.Context;

namespace TemplateMultiTenant.Infra.Configuration
{
    //Classe para adquirir contexto de forma singleton
    public static class RepositoryManager
    {
        public const string HttpCtxt = "HttpContext";
        private static string _connectionString;
    
        public static void SetUserDBConnection(string connectionString)
        {
            HttpContext.Current.Items[HttpCtxt] = null;
            _connectionString = connectionString;
        }

        public static TemplateMultiTenantContext Context
        {
            get
            {
                if (HttpContext.Current.Items[HttpCtxt] == null)
                {                    
                    HttpContext.Current.Items[HttpCtxt] = new TemplateMultiTenantContext(_connectionString);
                }
                return (HttpContext.Current.Items[HttpCtxt] as TemplateMultiTenantContext);                
            }
        }

        public static void Dispose()
        {
            if (HttpContext.Current.Items[HttpCtxt] != null)
            {
                (HttpContext.Current.Items[HttpCtxt] as TemplateMultiTenantContext).Dispose();
            }
        }
    }
}