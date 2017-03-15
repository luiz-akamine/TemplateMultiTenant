using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using TemplateMultiTenant.Auth.Models;
using TemplateMultiTenant.Auth.Repository;
using TemplateMultiTenant.Auth.Results;
using TemplateMultiTenant.Auth.ViewModel;

namespace TemplateMultiTenant.Auth.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;

        //Contrutor do controller criando o repository que realiza os cadastros e busca de usuários
        public AccountController()
        {
            _repo = new AuthRepository();
        }

        //API para registro/cadastro de client
        // POST api/Account/RegisterClient
        [AllowAnonymous]
        [Route("RegisterClient")]
        public IHttpActionResult RegisterClient([FromBody] ClientModel clientModel)
        {
            //ModelState é um dicionário disponível na classe base do controlador que armazena as informações adicionais e de estado sobre o modelo
            //O ModelState têm dois propósitos:
            //1.Armazenar o valor submetido ao servidor
            //2.Armazenar os erros de validação associados com esses valores
            if (!ModelState.IsValid)
            {                
                return BadRequest(ModelState);
            }

            //Tentando registrar/cadastrar usuário
            try
            {            
                if (_repo.RegisterClient(clientModel))
                {
                    return Ok();
                }
                else
                {
                    return InternalServerError();
                }
            }
            catch (ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }
        }

        //API para registro/cadastro de usuário
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            //ModelState é um dicionário disponível na classe base do controlador que armazena as informações adicionais e de estado sobre o modelo
            //O ModelState têm dois propósitos:
            //1.Armazenar o valor submetido ao servidor
            //2.Armazenar os erros de validação associados com esses valores
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            { 
                //Tentando registrar/cadastrar usuário
                IdentityResult result = await _repo.RegisterUser(userModel);

                //Verificando se há algum erro
                IHttpActionResult errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }
            }
            catch(ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }

            return Ok();
        }


        //API para alteração de senha de usuário
        // POST api/Account/ChangePassword
        [AllowAnonymous]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordModel changePasswordModel)
        {
            //ModelState é um dicionário disponível na classe base do controlador que armazena as informações adicionais e de estado sobre o modelo
            //O ModelState têm dois propósitos:
            //1.Armazenar o valor submetido ao servidor
            //2.Armazenar os erros de validação associados com esses valores
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Tentando alterar senha de usuário
            try
            {
                IdentityResult result = await _repo.ChangePassword(changePasswordModel);

                //Verificando se há algum erro
                IHttpActionResult errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }                               
            }
            catch (ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }

            return Ok();
        }

        //API para reset (simples) de senha de usuário
        // POST api/Account/ResetPasswordSimple
        [AllowAnonymous]
        [Route("ResetPasswordSimple")]
        public async Task<IHttpActionResult> ResetPasswordSimple(ResetPasswordSimpleModel resetPasswordModel)
        {
            //ModelState é um dicionário disponível na classe base do controlador que armazena as informações adicionais e de estado sobre o modelo
            //O ModelState têm dois propósitos:
            //1.Armazenar o valor submetido ao servidor
            //2.Armazenar os erros de validação associados com esses valores
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Tentando alterar senha de usuário
            try
            {
                IdentityResult result = await _repo.ResetPasswordSimple(resetPasswordModel);

                //Verificando se há algum erro
                IHttpActionResult errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }
            }
            catch (ApplicationException ae)
            {
                return BadRequest(ae.Message);
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }

            return Ok();
        }

        /******************************************************************
                           IMPLEMENTAÇÃO LOGIN EXTERNO
         *****************************************************************/

        //API para registro/cadastro de usuário externo!
        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //Checando token externo é mesmo de nossa aplicação do Facebook
            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            //Buscando se há cadastro do usuário no sistema
            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            //cadastrando "Client" que terá o mesmo nome do email            
            using (var repAuth = new AuthRepository())
            {
                try
                {
                    repAuth.RegisterClient(
                        new ClientModel()
                        {
                            Id = model.Email,
                            CNPJ = 0,
                            Name = model.UserName,
                            SubscriptionType = 0
                        }
                    );
                }
                catch
                {
                    //nothing
                }

                //cadastrando novo usuário
                user = await repAuth.RegisterUserExt(
                    new UserModel()
                    {
                        ClientId = model.Email,                
                        Email = model.Email,
                        UserName = model.Email
                    }
                );
            }

            /*
            //Cadastrando novo usuário
            user = new IdentityUser() { UserName = model.Email };
            //Salvando usuário sem password (AspNetUsers)
            IdentityResult result = await _repo.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }
            */

            var info = new ExternalLoginInfo()
            {
                //Criando registro em AspNetUserLogins
                DefaultUserName = model.Email,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
                //Login = new UserLoginInfo(model.Provider, model.UserName)
            };

            IdentityResult result = await _repo.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //Gerando token de acesso para devolver a nossa aplicação 
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.Email);

            return Ok(accessTokenResponse);
        }       

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]                                        //ignorando bearer tokens
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)] //acessado externamente pelos cookies
        [AllowAnonymous]                                                //pode ser acessado anonimamente
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                //Caso usuário não esteja autenticado, (401 - Unauthorized), será chamado o Challenge Result definido em ChallengeResult.ExecuteAsync
                return new ChallengeResult(provider, this);
            }

            //Validação da requisição do client solicitando login externo
            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);
            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            //adquirindo informações do login externo
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            //Buscando usuário nas tabelas do sistema
            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            //Montando URI para redirecionamento que será fornecida para a aplicação 
            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}&email={5}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName,
                                            externalLogin.Email);

            return Redirect(redirectUri);
        }

        //API para obter token de acesso via login externo
        [AllowAnonymous]
        [HttpGet]
        [Route("ObtainLocalAccessToken")]
        public async Task<IHttpActionResult> ObtainLocalAccessToken(string provider, string externalAccessToken)
        {
            //Necessário enviar provider (Facebook, por ex) e token externo
            if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(externalAccessToken))
            {
                return BadRequest("Provider or external access token is not sent");
            }

            //Validação do token externo (do Facebook)
            var verifiedAccessToken = await VerifyExternalAccessToken(provider, externalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            //Checando se está registrado no sistema
            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(user.UserName);

            return Ok(accessTokenResponse);

        }
         
        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            //if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority).Replace("https", "http"), StringComparison.OrdinalIgnoreCase))
            if (!client.AllowedOrigin.Equals("*"))
            { 
                if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
                {
                    return string.Format("The given URL '{1}' is not allowed by Client_id '{0}' configuration.", clientId, redirectUri.GetLeftPart(UriPartial.Authority));
                }
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        //Método que valida Token Externo
        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_token here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook
                var appToken = "336513216749674|2UJtXxqQwlgYzCYGMLgfD_yDPSc";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    //Checando se app_id é exatamente o mesmo que está registrado no nosso codigo-fonte
                    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
            }

            return parsedToken;
        }

        //Método que retorna token local ?
        private JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            //buscando string de conexao
            string connectionString = "";
            using (AuthRepository _repo = new AuthRepository())
            {
                connectionString = _repo.FindClient(userName).ConnectionString;
            }
            identity.AddClaim(new Claim("connectionstring", connectionString));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );
    
            return tokenResponse;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        //classe que adquire informações referente a autenticação externa 
        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string ExternalAccessToken { get; set; }
            public string Email { get; set; }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer) || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name),
                    ExternalAccessToken = identity.FindFirstValue("ExternalAccessToken"),
                    Email = identity.FindFirstValue(ClaimTypes.Email)
                };
            }
        }
    }        
}
