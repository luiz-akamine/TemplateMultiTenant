using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;
using TemplateMultiTenant.Infra.IoC;

//[assembly: PostApplicationStartMethod(typeof(SimpleInjectorInitializer), "Initialize")]
namespace TemplateMultiTenant.API.App_Start
{
    // install-package simpleinjector
    // install-package simpleinjector.integration.webapi
    // install-package SimpleInjector.Extensions.ExecutionContextScoping
    // install-package webactivator -version 1.5.3
    public static class SimpleInjectorInitializer
    {
        public static Container Initialize()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            Bindings.Start(container);

            // This is an extension method from the integration package.
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = 
                new SimpleInjectorWebApiDependencyResolver(container);

            return container;
        }
    }
}