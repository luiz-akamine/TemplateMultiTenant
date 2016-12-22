//Classe responsável por gerenciar a persistência dos RefreshTokens na base de dados do sistema
using TemplateMultiTenant.Auth;
using TemplateMultiTenant.Auth.Models;
using TemplateMultiTenant.Auth.Repository;
using Microsoft.Owin.Security.Infrastructure;
using System;
using System.Threading.Tasks;

public class SimpleRefreshTokenProvider : IAuthenticationTokenProvider
{
    public void Create(AuthenticationTokenCreateContext context)
    {
        throw new NotImplementedException();
    }

    public void Receive(AuthenticationTokenReceiveContext context)
    {
        throw new NotImplementedException();
    }

    //Implementação do método contendo lógica de geração de RefreshTokens
    public async Task CreateAsync(AuthenticationTokenCreateContext context)
    {
        //adquirindo client_id do ticket (definido na autenticação do usuário em SimpleAuthorizationServerProvider.GrantResourceOwnerCredentials
        var clientid = context.Ticket.Properties.Dictionary["as:client_id"];

        if (string.IsNullOrEmpty(clientid))
        {
            return;
        }

        //Gerando Id único para o client_id
        var refreshTokenId = Guid.NewGuid().ToString("n");

        using (AuthRepository _repo = new AuthRepository())
        {
            //adquirindo tempo de vida do ticket (definido na autenticação do usuário em SimpleAuthorizationServerProvider.GrantResourceOwnerCredentials)
            var refreshTokenLifeTime = context.OwinContext.Get<string>("as:clientRefreshTokenLifeTime");

            //criando instancia do RefreshToken a ser persistido no banco de dados
            var token = new RefreshToken()
            {
                //encriptando Id gerado
                Id = Helper.GetHash(refreshTokenId),
                ClientId = clientid,
                Subject = context.Ticket.Identity.Name,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
            };

            //Informando ao ticket o tempo de vida do token
            context.Ticket.Properties.IssuedUtc = token.IssuedUtc;
            context.Ticket.Properties.ExpiresUtc = token.ExpiresUtc;

            //guardando toda as informações acima, encriptada no campo ProtectedTicket
            token.ProtectedTicket = context.SerializeTicket();

            //Inserindo/atualizando token na tabela
            var result = await _repo.AddRefreshToken(token);

            if (result)
            {
                //setando token id gerado
                context.SetToken(refreshTokenId);
            }
        }
    }

    //Método que recebe RefreshToken para criar novos tokens 
    public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
    {
        //Configurando "access control allowed origin" / CORs
        var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");
        context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

        //Adquirindo hash do token, que é como está gravado na tabela RefreshTokens
        string hashedTokenId = Helper.GetHash(context.Token);

        using (AuthRepository _repo = new AuthRepository())
        {
            //Buscando RefreshToken armazenado na tabela
            var refreshToken = await _repo.FindRefreshToken(hashedTokenId);

            if (refreshToken != null)
            {
                //Deserializando ProtectedTicket que contém as informações necessárias para autenticação do refresh token
                //Assim, o context conterá todas estas informações
                context.DeserializeTicket(refreshToken.ProtectedTicket);
                //removendo, pois existirá apenas 1 refreshtoken por client/usuário
                var result = await _repo.RemoveRefreshToken(hashedTokenId);
            }
        }

        //Após a execução deste método, será executado o "SimpleAuthorizationServerProvider.GrantRefreshToken"
    }
}