using TemplateMultiTenant.Auth.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace TemplateMultiTenant.Auth.Context
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("Auth") { }

        //Tabelas/Entidades do sistema
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserClient> UserClients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>()
                .Property(c => c.Name).HasMaxLength(128).IsRequired();

            modelBuilder.Entity<IdentityUser>().ToTable("AspNetUsers")//I have to declare the table name, otherwise IdentityUser will be created
                .Property(c => c.UserName).HasMaxLength(128).IsRequired();
        }
    }    
}
