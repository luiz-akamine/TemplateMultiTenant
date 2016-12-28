using TemplateMultiTenant.Domain.Models;
using System.Linq;

namespace TemplateMultiTenant.Domain.Interfaces.Services
{
    public interface IProductService
    {
        IQueryable<Product> GetByType(int productType);
    }
}
