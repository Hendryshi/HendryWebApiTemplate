using Common.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Common.Application.Services.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private static IHttpContextAccessor _context;

        public TokenHelper(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetToken()
        {
            var bearer = _context.HttpContext?.Request?.Headers?.Authorization.ToString();
            var token = bearer?.Replace("Bearer ", string.Empty);
            return token;
        }

        public string GetClientIdFromToken()
        {
            var token = this.GetToken();
            if (token.IsNullOrEmpty()) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var clientId = jwtSecurityToken.Claims.SingleOrDefault(x => x.Type == "azp");

            return clientId?.Value;
        }
    }
}
