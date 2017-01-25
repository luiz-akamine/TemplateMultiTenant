using CommonServiceLocator.SimpleInjectorAdapter;
using Microsoft.Practices.ServiceLocation;
using SimpleInjector;
using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Interfaces.Services;
using TemplateMultiTenant.Domain.Services;
using TemplateMultiTenant.Infra.Configuration;
using TemplateMultiTenant.Infra.Repositories;

namespace TemplateMultiTenant.Infra.IoC
{
    public static class Bindings
    {
        public static void Start(Container container)
        {
            //Infra
            //merda alterando container.Register<IRepositoryManager, RepositoryManager>();
            container.Register<IUnityOfWork, UnitOfWork>(Lifestyle.Scoped);
            container.Register(typeof(IBaseRepository<>), typeof(BaseRepository<>), Lifestyle.Scoped);
            container.Register(typeof(IProductRepository), typeof(ProductRepository), Lifestyle.Scoped);

            //Services            
            container.Register(typeof(IBaseService<>), typeof(BaseService<>), Lifestyle.Scoped);
            container.Register(typeof(IProductService), typeof(ProductService), Lifestyle.Scoped);

            //Service Locator
            ServiceLocator.SetLocatorProvider(() => new SimpleInjectorServiceLocatorAdapter(container));
        }
    }
}
