//Este controller é apenas para gerenciamento dos RefreshTokens dos usuários logados
//Por exemplo, com esta API, será possível apagar determinado refresh token, forçando
//determinados usuários a refazerem o login
using TemplateMultiTenant.Auth.Repository;
using System.Threading.Tasks;
using System.Web.Http;

namespace AngularJSAuthentication.API.Controllers
{
    [RoutePrefix("api/RefreshTokens")]
    public class RefreshTokensController : ApiController
    {
        private AuthRepository _repo = null;

        //Construtor criando contexto para acesso a base de dados
        public RefreshTokensController()
        {
            _repo = new AuthRepository();
        }

        //Apenas usuário "Admin" terá acesso a esta API (Apenas um exemplo esta permissão)
        [Authorize(Users = "Admin")]
        [Route("")]
        public IHttpActionResult Get()
        {
            //Retorna lista de todos refresh tokens
            return Ok(_repo.GetAllRefreshTokens());
        }

        //Pode-se descomentar a linha abaixo, para como exemplo, apenas usuário "Admin" tenha acesso a esta API
        //[Authorize(Users = "Admin")]
        [AllowAnonymous]
        [Route("")]
        //API que apaga refresh_token
        [HttpDelete]        
        public async Task<IHttpActionResult> Delete(string tokenId)
        {
            var result = await _repo.RemoveRefreshToken(tokenId);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Token Id does not exist");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
