using System.Linq;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Configuration;

namespace TemplateMultiTenant.Infra.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository    
    {
        public IQueryable<Product> GetByType(int productType)
        {
            var products = RepositoryManager.Context.Products
                .Where(p => p.ProductType == productType);                

            return products;
        }
    }
}