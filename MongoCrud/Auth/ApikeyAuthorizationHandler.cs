using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace MongoCrudPeopleApi.Auth
{


    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiOptions>  
    {
        private const string ApiKeyHeaderName = "x-api-key";
        private readonly string ApiKeyValue;
     //   private readonly ILogger log ;


        public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder , IConfiguration conf) : base(options, logger, encoder)
        {
            ApiKeyValue = conf["API_KEY"]!;
           // log = logger.CreateLogger<ApiKeyAuthenticationHandler>();
        }


        protected override  Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            //var httpContext = Context;

            //log.LogWarning("{}IP failed to authenticate", httpContext.Connection.RemoteIpAddress?.ToString());

           
            //Response.StatusCode = 401;
            //Response.ContentType = "text/plain";
            //return Response.WriteAsync("Authorization failed: No API key provided");

            return base.HandleChallengeAsync(properties);
  
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {

           
            var keyexist = Request.Headers.Keys.Any(x=>x == "x-api-key");

            if (!keyexist)
            {
                return AuthenticateResult.NoResult();
            }

            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedApiKey))
            {
                return AuthenticateResult.NoResult();
            }

            if (!ApiKeyValue.Equals(providedApiKey,StringComparison.OrdinalIgnoreCase))
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
     
    }






}
