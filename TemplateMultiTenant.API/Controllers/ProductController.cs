using ClassLibrary1TemplateMultiTenant.Infra.Context;
using TemplateMultiTenant.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace TemplateMultiTenant.API.Controllers
{
    public class ProductController : ApiController
    {
        //[AcceptVerbs("GET")]
        [Authorize]
        public HttpResponseMessage Get()
        {
            
            var identity = User.Identity as ClaimsIdentity;

            var claims = identity.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).Where(_ => _.Type == "connectionstring").ToList();
            var connectionString = claims.Select(_ => _.Value).FirstOrDefault();

            //return Request.CreateResponse(HttpStatusCode.OK, claims.ToList());            

            
            TemplateMultiTenantContext context = new TemplateMultiTenantContext(connectionString);
            /*
            var prodNew = new Product()
            {                
                Code = "P02",
                Name = "Produto 02",
                Price = 200
            };

            context.Products.Add(prodNew);

            context.SaveChanges();
            */

            var products = from p in context.Products
                           select p;
            

            return Request.CreateResponse(HttpStatusCode.OK, products.ToList());            
        }
    }
}
