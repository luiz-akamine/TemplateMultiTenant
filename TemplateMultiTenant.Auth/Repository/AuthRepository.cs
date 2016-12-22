using TemplateMultiTenant.Auth.Context;
using TemplateMultiTenant.Auth.Models;
using TemplateMultiTenant.Auth.ViewModel;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TemplateMultiTenant.Auth.Repository
{
    public class AuthRepository : IDisposable
    {
        private AuthContext _ctx;

        //Classe UserManager que gerencia a autenticação, registro, hash de password, etc referente ao usuário
        private UserManager<IdentityUser> _userManager;

        //Construtor criando o contexto e o UserManager
        public AuthRepository()
        {
            _ctx = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
        }

        //Método que cadastra client
        //você usa a palavra-chave async na declaração de uma função que dependa da palavra-chave await
        public bool RegisterClient(ClientModel clientModel)
        {
            Client client = FindClient(clientModel);

            //Checando se já existe
            if (client != null)
            {
                throw new ApplicationException("client already registered");
            }

            //Criando client
            client = new Client()
            {
                Id = clientModel.Id,
                Name = clientModel.Name,
                CNPJ = clientModel.CNPJ,
                SubscriptionType = clientModel.SubscriptionType,
                DtCreation = DateTime.Now,
                ApplicationType = ApplicationTypes.JavaScript,
                Active = true,
                AllowedOrigin = "*",
                Secret = Helper.GetHash(clientModel.Id),
                RefreshTokenLifeTime = 28800,
                ConnectionString = _ctx.Database.Connection.ConnectionString.Replace(_ctx.Database.Connection.Database + ";", clientModel.Id + ";") + ";Password=amazon87!",
                DtSubscriptionExpiration = GetDtSubscriptionExpiration(clientModel.SubscriptionType)
            };
            _ctx.Clients.Add(client);
            _ctx.SaveChanges();

            return true;
        }

        //Retorna data de expiração da assinatura
        private DateTime GetDtSubscriptionExpiration(SubscriptionType subType)
        {
            switch (subType)
            {
                case SubscriptionType.Free:
                    return DateTime.MinValue;                    
                case SubscriptionType.Basic:
                    return DateTime.Now.AddMonths(1);
                case SubscriptionType.Premium:
                    return DateTime.Now.AddMonths(3);
                default:
                    return DateTime.MinValue;
            }
        }

        //Método que busca client
        public Client FindClient(ClientModel clientModel)
        {

            return (from c in _ctx.Clients
                    where c.Id == clientModel.Id
                    select c).FirstOrDefault();
        }

        public Client FindClientById(string clientId)
        {

            return (from c in _ctx.Clients
                    where c.Id == clientId
                    select c).FirstOrDefault();
        }


        //Método que cadastra usuário
        //você usa a palavra-chave async na declaração de uma função que dependa da palavra-chave await
        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName                
            };            

            //Verificando se existe client
            Client client = FindClientById(userModel.ClientId);
            if (client == null)
            {
                throw new ApplicationException("client not found");
            }            

            // 1) await é um "comando" para o código ficar esperando pela conclusão de uma tarefa até o fim, para aí sim,
            //continuar a execução normal permitindo que outras execuções possam acontecer concomitantemente
            // 2) await só pode ser usado em um método declarado com o modificador async
            var result = await _userManager.CreateAsync(user, userModel.Password);                        

            if (result.Succeeded)
            { 
                //Vinculando usuario ao client
                UserClient userClient = new UserClient()
                {
                    UserName = userModel.UserName,
                    Client = client
                };
                _ctx.UserClients.Add(userClient);
                _ctx.SaveChanges();
            }

            return result;
        }

        //Método que busca usuário
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        //Liberação das conexões e gerenciador da autenticação dos usuários
        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();
        }


        /*********************************************************
                     IMPLEMENTAÇÃO DO REFRESH TOKEN 
        *********************************************************/

        //Método que busca usuário
        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        //Método que adiciona RefreshToken
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            //"Chave" do RefreshToken será usuario (subject) + client (clientid)
            var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

            if (existingToken != null)
            {
                //removendo caso já exista
                var result = await RemoveRefreshToken(existingToken);
            }

            //Adicionando
            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        //Método que remove registro do Refresh Token pelo Id
        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _ctx.RefreshTokens.Remove(refreshToken);
                return await _ctx.SaveChangesAsync() > 0;
            }

            return false;
        }

        //Método que remove registro do Refresh Token pelo próprio objeto
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return await _ctx.SaveChangesAsync() > 0;
        }

        //Método que busca RefreshToken pelo Id
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        //Retorna lista dos RefreshTokens cadastrados
        public List<RefreshToken> GetAllRefreshTokens()
        {
            return _ctx.RefreshTokens.ToList();
        }


        /*********************************************************
                     IMPLEMENTAÇÃO DO LOGIN EXTERNO
        *********************************************************/

        public async Task<IdentityUser> FindAsync(UserLoginInfo loginInfo)
        {
            IdentityUser user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(IdentityUser user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }
    }
}