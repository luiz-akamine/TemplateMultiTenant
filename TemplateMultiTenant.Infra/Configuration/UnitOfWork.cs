using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Infra.Context;
using Microsoft.Practices.ServiceLocation;

namespace TemplateMultiTenant.Infra.Configuration
{
    public class UnitOfWork : IUnityOfWork
    {
        private TemplateMultiTenantContext _context;

        public void Commit()
        {
            _context.SaveChanges();
        }

        public void BeginTrans()
        {
            //merda alterando var repositoryManager = ServiceLocator.Current.GetInstance<IRepositoryManager>() as RepositoryManager;            
            _context = RepositoryManager.Context;
        }        
    }
}