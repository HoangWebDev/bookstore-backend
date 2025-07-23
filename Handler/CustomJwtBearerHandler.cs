using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace BookStore.Handler
{
    public class CustomJwtBearerHandler : JwtBearerHandler
    {
        private readonly HttpClient _httpClient;

        public CustomJwtBearerHandler(HttpClient httpClient, IOptionsMonitor<JwtBearerOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock!)
        {
            _httpClient = httpClient;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Get the token from the Authorization header
            if (!Context.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValues))
            {
                return AuthenticateResult.Fail("Authorization header not found.");
            }

            var authorizationHeader = authorizationHeaderValues.FirstOrDefault();
            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Bearer token not found in Authorization header.");
            }

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            // decode token

            // Call the API to validate the token
            // var response = await _httpClient.GetAsync($"BASE_URL/api/Validate?token={token}");
            // check DB

            // Return an authentication failure if the response is not successful
            //if (!response.IsSuccessStatusCode)
            //{
            //    return AuthenticateResult.Fail("Token validation failed.");
            //}

            // Deserialize the response body to a custom object to get the validation result
            //  var validationResult = JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());

            // Return an authentication failure if the token is not valid
            //if (!validationResult)
            //{
            //    return AuthenticateResult.Fail("Token is not valid.");
            //}

            // Set the authentication result with the claims from the API response
            var principal = GetClaims(token);

            return AuthenticateResult.Success(new AuthenticationTicket(principal, "CustomJwtBearer"));
        }


        private ClaimsPrincipal GetClaims(string Token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(Token) as JwtSecurityToken;

            if (jwtToken == null)
                throw new SecurityTokenException("Invalid token");

            var claims = jwtToken.Claims.ToList();

            // Kiểm tra nếu claim "role" chưa tồn tại, có thể phải lấy từ một trường khác
            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role) ??
                            claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");

            if (roleClaim == null)
                throw new SecurityTokenException("Role claim not found");

            var claimsIdentity = new ClaimsIdentity(claims, "Token");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            return claimsPrincipal;
        }
    }
}
