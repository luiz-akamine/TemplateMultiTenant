using Microsoft.Practices.ServiceLocation;
using System.Linq;
using TemplateMultiTenant.Domain.Interfaces.Infra;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Interfaces.Services;
using TemplateMultiTenant.Domain.Models;

namespace TemplateMultiTenant.Domain.Services
{
    public class ProductService : BaseService<Product>, IProductService    
    {
        private readonly IProductRepository _productRepository;
        
        public ProductService(IBaseRepository<Product> entityRepository, IUnityOfWork unitOfWork) : base(entityRepository, unitOfWork)        
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
