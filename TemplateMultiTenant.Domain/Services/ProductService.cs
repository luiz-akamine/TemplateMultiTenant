using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Interfaces.Services;
using TemplateMultiTenant.Domain.Models;
using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace TemplateMultiTenant.Domain.Services
{
    public class ProductService : BaseService<Product>, IProductService
    //public class ProductService : BaseService<EntityBase>, IProductService
    {
        private readonly IProductRepository _productRepository;
        
        public ProductService(IBaseRepository<Product> entityRepository, IUnityOfWork unitOfWork) : base(entityRepository, unitOfWork)
        //public ProductService(IBaseRepository<EntityBase> entityRepository, IUnityOfWork unitOfWork) : base(entityRepository, unitOfWork)
        {
            //Necessário criar desta maneira para adquirir as rotinas customizadas diferentes do baseRepository
            _productRepository = ServiceLocator.Current.GetInstance<IProductRepository>();
        }

        public IQueryable<Product> GetByType(int productType)
        {
            ServiceHelper.ValidateParams(new object[] { productType });

            return _productRepository.GetByType(productType);            
        }
    }
}
