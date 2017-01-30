using System.Data.Entity.ModelConfiguration;
using TemplateMultiTenant.Domain.Models;

namespace TemplateMultiTenant.Infra.Mappings
{
    class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            Property(x => x.Code).HasMaxLength(10).IsRequired();
            Property(x => x.Name).HasMaxLength(50).IsRequired();
        }
    }
}
