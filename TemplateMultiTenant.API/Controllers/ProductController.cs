using TemplateMultiTenant.Domain.Models;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TemplateMultiTenant.Domain.Services;
using TemplateMultiTenant.Domain.Interfaces.Services;
using System;

namespace TemplateMultiTenant.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/Product")]
    public class ProductController : BaseController<Product>    
    {        
        private readonly ProductService _productService;

        //Construtor custom quando houver necessidade de ter métodos diferentes da classe base
        public ProductController(IBaseService<Product> baseService, IProductService productService) : base(baseService)        
        {
            _productService = productService as ProductService;
        }

        // API Principal, na qual redireciona para as "APIs" requisitadas
        [AcceptVerbs("POST")]
        [Route("ExecMethod")]
        public HttpResponseMessage ExecMethod(RequestBase request)
        {
            try
            {
                //Definindo conexão do banco de dados de acordo com o usuário logado
                ControllerHelper.SetUserDBConnection(User);

                //Redirecionando paras as "APIs" requisitadas
                switch (request.MethodName.ToUpper())
                {
                    case "GETBYTYPE":
                        return GetByType(ControllerHelper.ConvertRequestObject<Int32>(request));                    
                    default:
                        return base.ExecBaseMethod(request);
                }
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (ArgumentException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }


        // "APIs" específicas do Produto

        //Rotina custom teste        
        private HttpResponseMessage GetByType(int productType)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _productService.GetByType(productType));
        }        
    }
}
