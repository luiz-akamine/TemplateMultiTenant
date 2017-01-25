using Newtonsoft.Json;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using TemplateMultiTenant.Domain.Models;
using TemplateMultiTenant.Infra.Configuration;

namespace TemplateMultiTenant.API.Controllers
{
    public static class ControllerHelper
    {
        // Método auxiliar para conversão dos parâmetros
        public static T ConvertRequestObject<T>(RequestBase request)
        {
            try
            { 
                if (request.Params != null)
                {
                    return JsonConvert.DeserializeObject<T>(request.Params.ToString());
                }

                throw new ArgumentNullException("Params is null in Request Object");
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        // Método que seta conexão do banco de dados do usuário logado
        public static void SetUserDBConnection(IPrincipal user)
        {
            var identity = user.Identity as ClaimsIdentity;

            var claims = identity.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).Where(_ => _.Type == "connectionstring").ToList();

            var connectionString = claims.Select(_ => _.Value).FirstOrDefault();

            RepositoryManager.SetUserDBConnection(connectionString);
        }
    }
}