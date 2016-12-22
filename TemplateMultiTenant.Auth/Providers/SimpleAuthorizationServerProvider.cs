using TemplateMultiTenant.Auth.Context;
using TemplateMultiTenant.Auth.Models;
using TemplateMultiTenant.Auth.Repository;
//Classe responsável por prover geração do token
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TemplateMultiTenant.Auth.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        //Reescrevendo método que valida usuário e senha gerando o Token para o usuário acessar as APIs protegidas
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //adquirindo "access control allowed origin" para configurar CORs no Owin
            var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
            if (allowedOrigin == null) allowedOrigin = "*";

            //Tratamento para CORS -> Habilitando acesso de vários domínios
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });            

            //Criando repository para acessar usuário
            using (AuthRepository _repo = new AuthRepository())
            {
                //Tentando buscar usuário
                IdentityUser user = await _repo.FindUser(context.UserName, context.Password);

                if (user == null)
                {
                    //Não encontrou, erro:
                    context.SetError("invalid_grant", "Usuário e senha inválidos");
                    return;
                }

                //Verificando se usuario faz parte do client
                if (!UserInClient(context.UserName, context.ClientId))
                {
                    //Não encontrou, erro:
                    context.SetError("invalid_client", "Usuário não faz parte do Client informado");
                    return;
                }
            }

            //buscando string de conexao
            string connectionString = "";
            using (AuthRepository _repo = new AuthRepository())
            {
                connectionString = _repo.FindClient(context.ClientId).ConnectionString;
            }

            //Caso credenciais sejam válidas, criar ClaimsIdendity, que terá as Claims do usuário
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            //Cada Claim é como se fosse uma característica da identidade do usuário
            //Estas Claims irão criptografadas no hearder de cada requisição para as APIs que necessitam autenticação
            //Quanto mais Claims, maior o tamanho do token gerado
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim("sub", context.UserName)); //"sub" de "subject" -> usuário
            identity.AddClaim(new Claim("role", "user"));
            identity.AddClaim(new Claim("connectionstring", connectionString));

            //Formatando propriedades client_id e username a serem passados como parâmetro para geração do ticket
            var props = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { 
                        "as:client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                    },
                    { 
                        "userName", context.UserName
                    }
                });
            
            var ticket = new AuthenticationTicket(identity, props);
            //GERAÇÃO DO TOKEN!!!
            context.Validated(ticket);
        }

        //Verifica se usuario faz parte do client
        private bool UserInClient(string userName, string clientId)
        {
            using (AuthContext ctx = new AuthContext())
            {
                var userClients = (from uc in ctx.UserClients
                                   where uc.UserName == userName && uc.Client.Id == clientId
                                   select uc).ToList();                                  
                if (userClients != null)
                {
                    return (userClients.Count > 0);
                }
                else
                {
                    return false;
                }
            }
        }

        //Método de validação de autenticação com base nas tabelas de controle criada para gerenciar os Clients
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //variáveis para acesso ao "Client"
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            Client client = null;

            //Tentando adquirir client_id/client_secret usando autenticação básica: base64 encode the (client_id:client_secret) 
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                //Tentnado adquirir client_id/client_secret usando x-www-form-urlencoded”
                context.TryGetFormCredentials(out clientId, out clientSecret);
            }

            //Verificando se encontrou client_id na requisição
            if (context.ClientId == null)
            {
                //Remove the comments from the below line context.SetError, and invalidate context 
                //if you want to force sending clientId/secrects once obtain access tokens. 
                context.Validated();
                //context.SetError("invalid_clientId", "ClientId should be sent.");
                return Task.FromResult<object>(null);
            }

            //Caso foi enviado o client_id, buscar objeto mapeado no E.F.
            using (AuthRepository _repo = new AuthRepository())
            {
                client = _repo.FindClient(context.ClientId);
            }

            //Checando se client_id está registrado na tabela de clients
            if (client == null)
            {
                context.SetError("invalid_clientId", string.Format("Client '{0}' não está registrado no sistema", context.ClientId));
                return Task.FromResult<object>(null);
            }

            //Checando tipo de aplicação cadastrada para o client
            if (client.ApplicationType == Models.ApplicationTypes.NativeConfidential)
            {
                //Checando Secret do client
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    context.SetError("invalid_clientId", "Client secret deveria ser enviado.");
                    return Task.FromResult<object>(null);
                }
                else
                {
                    //Validando Secret
                    if (client.Secret != Helper.GetHash(clientSecret))
                    {
                        context.SetError("invalid_clientId", "Client secret inválido");
                        return Task.FromResult<object>(null);
                    }
                }
            }

            //Checando se client está ativo no sistema
            if (!client.Active)
            {
                context.SetError("invalid_clientId", "Client inativo");
                return Task.FromResult<object>(null);
            }

            //setando no Owin a configuração CORs
            context.OwinContext.Set<string>("as:clientAllowedOrigin", client.AllowedOrigin);
            //setando no Owin a configuração tempo de vida do Token
            context.OwinContext.Set<string>("as:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());

            context.Validated();
            return Task.FromResult<object>(null);
        }


        /*********************************************************
                     IMPLEMENTAÇÃO DO REFRESH TOKEN 
        *********************************************************/


        //Método que faz o processo de geração automática do novo token (RefreshToken)
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            //Adquirindo client_id "original" que foi gravado no campo RefreshTOken.ProtectedTicket
            var originalClient = context.Ticket.Properties.Dictionary["as:client_id"];
            //Adquirindo client_id da requisição atual
            var currentClient = context.ClientId;

            //Verificando se  a requisição veio do mesmo "client"
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token emitido de um client diferente do atual");
                return Task.FromResult<object>(null);
            }

            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            //Exemplo, podemos adicionar ou remover claims
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));            

            //Gerando novo ticket
            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            ////GERAÇÃO DO REFRESH TOKEN!!!
            context.Validated(newTicket);

            return Task.FromResult<object>(null);

            //Após a execução deste método, será executado o método "SimpleRefreshTokenProvider.CreateAsync" 
            //para criação do novo refresh token
        }
    }
}
