using ClassLibrary1TemplateMultiTenant.Infra.Mappings;
using TemplateMultiTenant.Domain.Models;
using System.Data.Entity;

namespace ClassLibrary1TemplateMultiTenant.Infra.Context
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class TemplateMultiTenantContext : DbContext
    {
        //String de conexão dinâmica
        public TemplateMultiTenantContext(string connectionString) : base(connectionString) { }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            modelBuilder.Configurations.Add(new ProductMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}
