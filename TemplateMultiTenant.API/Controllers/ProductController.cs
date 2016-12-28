using TemplateMultiTenant.Infra.Context;
using TemplateMultiTenant.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using TemplateMultiTenant.Domain.Services;
using TemplateMultiTenant.Domain.Interfaces.Services;

namespace TemplateMultiTenant.API.Controllers
{
    [RoutePrefix("api/Product")]
    public class ProductController : BaseController<Product>    
    {        
        private readonly ProductService _productService;

        //Construtor custom quando houver necessidade de ter métodos diferentes da classe base
        public ProductController(IBaseService<Product> baseService, IProductService productService) : base(baseService)        
        {
            _productService = productService as ProductService;
        }            

        //Rotina custom teste
        [Route("GetByType")]
        public HttpResponseMessage GetByType(int productType)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _productService.GetByType(productType));
        }

        [AcceptVerbs("GET")]
        [Route("APITest")]
        [Authorize]
        public HttpResponseMessage APITest()
        {
            
            var identity = User.Identity as ClaimsIdentity;

            var claims = identity.Claims.Select(c => new
            {
                Type = c.Type,
                Value = c.Value
            }).Where(_ => _.Type == "connectionstring").ToList();
            var connectionString = claims.Select(_ => _.Value).FirstOrDefault();


            TemplateMultiTenantContext context = new TemplateMultiTenantContext(connectionString);
            //TemplateMultiTenantContext context = new TemplateMultiTenantContext();
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
