using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TemplateMultiTenant.Infra;

namespace TemplateMultiTenant.API.Controllers
{
    [Authorize]
    [RoutePrefix("api/DB")]
    public class DBController : ApiController
    {
        [AcceptVerbs("POST")]
        [Route("ExecDBMigration")]
        public HttpResponseMessage ExecDBMigration()
        {
            try
            {
                DBHelper.SetUserDBConnection(User, false);

                return Request.CreateResponse(HttpStatusCode.OK, "Number of Migrations: " + DBHelper.ExecDBMigration().ToString());
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
