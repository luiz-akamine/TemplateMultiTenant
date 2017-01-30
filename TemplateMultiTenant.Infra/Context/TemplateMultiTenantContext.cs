using System.Data.Entity;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Mappings;

namespace TemplateMultiTenant.Infra.Context
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class TemplateMultiTenantContext : DbContext
    {
        //String de conexão dinâmica
        public TemplateMultiTenantContext(string connectionString) : base(connectionString) { }
        //construtor padrão teste para migrations
        public TemplateMultiTenantContext() : base("server=localhost;user id=root;password=mysql87!;persistsecurityinfo=True;database=templatemultitenant") { }
        /*
        static TemplateMultiTenantContext()
        {
            // static constructors are guaranteed to only fire once per application.
            // I do this here instead of App_Start so I can avoid including EF
            // in my MVC project (I use UnitOfWork/Repository pattern instead)
            DbConfiguration.SetConfiguration(new MySql.Data.Entity.MySqlEFConfiguration());
        }
        */

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {            
            modelBuilder.Configurations.Add(new ProductMap());

            base.OnModelCreating(modelBuilder);
        }

    }
}
