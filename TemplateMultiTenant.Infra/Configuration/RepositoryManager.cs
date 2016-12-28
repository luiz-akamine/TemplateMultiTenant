using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Infra.Context;
using System.Web;

namespace TemplateMultiTenant.Infra.Configuration
{
    //Classe para adquirir contexto de forma singleton
    public class RepositoryManager : IRepositoryManager
    {
        public const string HttpCtxt = "HttpContext";
        private string connectionString = "server=localhost;user id=root;password=mysql87!;persistsecurityinfo=True;database=templatemultitenant";

        public RepositoryManager() { }

        //merda public RepositoryManager(string connectionString)
        //{
        //    this.connectionString = connectionString;
        //}

        public TemplateMultiTenantContext Context
        {
            get
            {
                
                if (HttpContext.Current.Items[HttpCtxt] == null)
                {
                    HttpContext.Current.Items[HttpCtxt] = new TemplateMultiTenantContext(connectionString);
                }
                return (HttpContext.Current.Items[HttpCtxt] as TemplateMultiTenantContext);                
            }
        }

        public void Dispose()
        {
            if (HttpContext.Current.Items[HttpCtxt] != null)
            {
                (HttpContext.Current.Items[HttpCtxt] as TemplateMultiTenantContext).Dispose();
            }
        }
    }
}