using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TemplateMultiTenant.Infra.Configuration;

namespace TemplateMultiTenant.Infra
{
    public static class DBHelper
    {
        // Método que seta conexão do banco de dados do usuário logado
        public static void SetUserDBConnection(IPrincipal user, bool execDBMigration)
        {
            var identity = user.Identity as ClaimsIdentity;
            //Adquirindo string de conexão nas claims do usuario autenticado
            var claims = identity.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).Where(_ => _.Type == "connectionstring").ToList();

            var connectionString = claims.Select(_ => _.Value).FirstOrDefault();

            //Setando conexão
            RepositoryManager.SetUserDBConnection(connectionString);

            //Migration
            if (execDBMigration)
            {
                ExecDBMigration();
            }
        }

        // Método para migrar banco do usuário caso necessário
        public static int ExecDBMigration()
        {                     
            var configuration = new DbMigrationsConfiguration();
            var type = RepositoryManager.Context.GetType();

            configuration.MigrationsAssembly = type.Assembly;
            configuration.TargetDatabase = new DbConnectionInfo(RepositoryManager.Context.Database.Connection.ConnectionString, "MySql.Data.MySqlClient");
            configuration.MigrationsNamespace = "TemplateMultiTenant.Infra.Migrations";
            configuration.ContextKey = "TemplateMultiTenant.Infra.Migrations.Configuration";
            configuration.ContextType = type;
            var migrator = new DbMigrator(configuration);

            var pendingMigrations = migrator.GetPendingMigrations().Count();

            migrator.Update();

            return pendingMigrations;
        }        
    }
}
