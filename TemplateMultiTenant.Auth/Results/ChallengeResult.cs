//Classe implementada para funcionamento do login externo (Facebook, Google...)
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace TemplateMultiTenant.Auth.Results
{
    //Classe que implementa IHttpActionResult, onde é possível criar próprias HttpResponseMessage customizadas
    public class ChallengeResult : IHttpActionResult
    {
        public string LoginProvider { get; set; }
        public HttpRequestMessage Request { get; set; }

        public ChallengeResult(string loginProvider, ApiController controller)
        {
            LoginProvider = loginProvider;
            Request = controller.Request;
        }

        //Implementação do único método disponibilizado pela interface
        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            Request.GetOwinContext().Authentication.Challenge(LoginProvider);

            //Pelo que entendi, quando for retornado um erro 401 (Unauthorized), será redirecionado para a página 
            //de autenticação do "sistema externo" (Facebook, Google...)
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            response.RequestMessage = Request;
            return Task.FromResult(response);
        }
    }
}