using TemplateMultiTenant.Auth.Providers;
using Microsoft.Owin;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Web.Http;

[assembly: OwinStartup(typeof(TemplateMultiTenant.Auth.Startup))]

namespace TemplateMultiTenant.Auth
{
    public class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }
        public static FacebookAuthenticationOptions facebookAuthOptions { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

            //Configurando autenticação OAuth
            ConfigureOAuth(app);

            //HttpConfiguration - usado para configurar rotas da API
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);

            //Habilitando CORs
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);

            //passando configurações ASP.NET Web API para o OWIN
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            //usando temporariamente cookie para armazenar informação referente o usuário logando com login externo
            app.UseExternalSignInCookie(Microsoft.AspNet.Identity.DefaultAuthenticationTypes.ExternalCookie);
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();

            //Configuração da autenticação OAuth
            OAuthAuthorizationServerOptions OAuthServerOptions = new OAuthAuthorizationServerOptions()
            {
                AllowInsecureHttp = true,
                //A geração do token será o local de nossa API + "/token". Ex: "http://localhost:port/token"
                TokenEndpointPath = new PathString("/token"),
                //Tempo de validade do token para acessarmos as APIs que necessitam autenticação
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                //Classe que implementa "OAuthAuthorizationServerProvider", responsável por validar as credenciais de usuários que solicitam o token
                Provider = new SimpleAuthorizationServerProvider(),
                //Classe que implementa "IAuthenticationTokenProvider", responsável por gerenciar os RefreshTokens
                RefreshTokenProvider = new SimpleRefreshTokenProvider()
            };

            // Token Generation
            app.UseOAuthAuthorizationServer(OAuthServerOptions);
            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

            //Configuração Facebook External Login
            facebookAuthOptions = new FacebookAuthenticationOptions()
            {
                //Informações do app criado no site developers.facebook
                AppId = "169113803505203",
                AppSecret = "a80f762ae2ec965978758460528338f8",
                Provider = new FacebookAuthProvider()
            };
            app.UseFacebookAuthentication(facebookAuthOptions);
        }
    }
}
