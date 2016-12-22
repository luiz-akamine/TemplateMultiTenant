using TemplateMultiTenant.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1TemplateMultiTenant.Infra.Mappings
{
    class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            Property(x => x.Code).HasMaxLength(10).IsRequired();
            Property(x => x.Name).HasMaxLength(10).IsRequired();
        }
    }
}
