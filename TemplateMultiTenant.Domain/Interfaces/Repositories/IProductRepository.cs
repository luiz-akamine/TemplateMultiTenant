using TemplateMultiTenant.Domain.Models;
using System.Linq;

namespace TemplateMultiTenant.Domain.Interfaces.Repositories
{
    public interface IProductRepository 
    {
        IQueryable<Product> GetByType(int productType);
    }
}
