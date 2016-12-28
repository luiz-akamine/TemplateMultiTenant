using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

[assembly: OwinStartup(typeof(TemplateMultiTenant.API.Startup))]

namespace TemplateMultiTenant.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //
            var container = App_Start.SimpleInjectorInitializer.Initialize();

            //var container = GetContainer(); // Initialise container

            HttpConfiguration config = new HttpConfiguration
            {
                DependencyResolver = new SimpleInjector.Integration.WebApi.SimpleInjectorWebApiDependencyResolver(container)
            };


        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        //HttpConfiguration config = new HttpConfiguration();

            ConfigureOAuth(app);            

            WebApiConfig.Register(config);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);

            //
            //App_Start.SimpleInjectorInitializer.Initialize();
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            //Token Consumption
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions
            {
            });
        }
    }
}
