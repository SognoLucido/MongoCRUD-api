using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MongoCrudPeopleApi.Auth
{


    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiOptions>  
    {
      


        public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder ) : base(options, logger, encoder)
        {

        }



        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

            
           

            var keyexist = Request.Headers.Keys.Any(x=>x == Options.ApiKeyHeaderName);

            if (!keyexist)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.TryGetValue(Options.ApiKeyHeaderName, out var providedApiKey))
            {
                
                return AuthenticateResult.NoResult();
            }

            if (!Options.ApiKey.Equals(providedApiKey,StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.NoResult();
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "apikey-authorized") };
            var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);


        }

    }


    public class ApiOptions : AuthenticationSchemeOptions
    {
        
        public string ApiKeyHeaderName { get; set; }
        public string ApiKey { get; set; }


    }






}
