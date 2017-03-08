using Microsoft.Owin.Security.Facebook;
using SimpleFacebookClient;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TemplateMultiTenant.Auth.Providers
{
    public class FacebookAuthProvider : FacebookAuthenticationProvider
    {
        public override Task Authenticated(FacebookAuthenticatedContext context)
        {
            //Guardaremos o Token do Facebook em uma claim nomeada "ExternalAccessToken" do contexto
            //context.Identity.AddClaim(new Claim("ExternalAccessToken", context.AccessToken));
            var accessTokenClaim = new Claim("ExternalAccessToken", context.AccessToken, "urn:facebook:access_token");
            context.Identity.AddClaim(accessTokenClaim);

            var email = GetFacebookEmail(accessTokenClaim);
            context.Identity.AddClaim(new Claim(ClaimTypes.Email, email));

            return Task.FromResult<object>(null);
        }

        private static string GetFacebookEmail(Claim accessToken)
        {
            var fb = new FacebookClient(accessToken.Value);

            Dictionary<string, string> parameters = new Dictionary<string, string>()
            {
                { "fields", "email,name" }                
            };

            dynamic results = fb.Get<dynamic>("/me", parameters);

            return Convert.ToString(results["email"]);
        }
    }
}