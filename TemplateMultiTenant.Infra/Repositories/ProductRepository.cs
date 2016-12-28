using System;
using System.Linq;
using TemplateMultiTenant.Domain.Interfaces.Repositories;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Repositories;

namespace TemplateMultiTenant.Infra.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    //public class ProductRepository : BaseRepository<EntityBase>, IProductRepository
    {
        public IQueryable<Product> GetByType(int productType)
        {
            var products = _context.Products
                .Where(p => p.ProductType == productType);                

            return products;
        }
    }
}