using TemplateMultiTenant.Domain.Interfaces.Services;
using TemplateMultiTenant.Domain.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data.Entity.Validation;
using TemplateMultiTenant.Infra;

namespace TemplateMultiTenant.API.Controllers
{
    [Authorize]
    public class BaseController<TEntity>: ApiController where TEntity : EntityBase
    {
        private readonly IBaseService<TEntity> _baseService;        

        public BaseController(IBaseService<TEntity> baseService)
        {
            _baseService = baseService;            
        }

        // API Principal, na qual redireciona para as "APIs" requisitadas
        [AcceptVerbs("POST")]
        [Route("ExecMethod")]
        public HttpResponseMessage ExecBaseMethod(RequestBase request)
        {
            try
            {
                //Definindo conexão do banco de dados de acordo com o usuário logado
                DBHelper.SetUserDBConnection(User, false);            

                //Redirecionando paras as "APIs" requisitadas
                switch (request.MethodName.ToUpper())
                {
                    case "GET":
                        return Get();                    
                    case "GETBYID":
                        return Get(ControllerHelper.ConvertRequestObject<Int32>(request));
                    case "POST":                    
                        return Post(ControllerHelper.ConvertRequestObject<TEntity>(request));
                    case "PUT":
                        return Put(ControllerHelper.ConvertRequestObject<TEntity>(request));
                    case "DELETE":
                        return Delete(ControllerHelper.ConvertRequestObject<TEntity>(request));
                    case "DELETEBYID":
                        return Delete(ControllerHelper.ConvertRequestObject<Int32>(request));                    
                    default:
                        return Request.CreateResponse(HttpStatusCode.BadRequest, "Method not found");
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


        // "APIs" (chamadas quando requisitadas na API "ExecBaseMethod" no campo "MethodName"

        private HttpResponseMessage Get()
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _baseService.GetAll().ToList());
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private HttpResponseMessage Get(int id)
        {
            try
            {
                var obj = _baseService.GetById(id);

                if (obj == null)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, "object not found");
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, obj);
                }
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private HttpResponseMessage Post(TEntity obj)
        {
            try
            {
                _baseService.Post(obj);
                return Request.CreateResponse(HttpStatusCode.Created, obj.Id);
            }
            catch (ArgumentException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private HttpResponseMessage Put(TEntity obj)
        {
            try
            {
                _baseService.Update(obj);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private HttpResponseMessage Delete(TEntity obj)
        {
            try
            {
                _baseService.Delete(obj);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        private HttpResponseMessage Delete(int id)
        {
            try
            {
                _baseService.Delete(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (ArgumentNullException e)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            catch (DbEntityValidationException e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.EntityValidationErrors.ToList());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }        
    }
}
