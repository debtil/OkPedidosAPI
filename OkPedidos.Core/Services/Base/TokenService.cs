using Microsoft.AspNetCore.Http;
using OkPedidos.Models.Models.Base;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidos.Core.Services.Base
{
    public class TokenService
    {
        public static IdentityUserModel? GetIdentity(HttpContext? httpContext)
        {
            if (httpContext == null)
                return null;

            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return null;

            var token = authHeader["Bearer ".Length..].Trim();

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadToken(token) as JwtSecurityToken;
            if (jwt == null)
                return null;

            var claims = jwt.Claims;

            string? Get(string type) =>
                claims.FirstOrDefault(c => c.Type == type)?.Value;

            var idStr = Get(JwtRegisteredClaimNames.Sub) ?? Get("userId");
            int.TryParse(idStr, out var userId);

            var email = Get(JwtRegisteredClaimNames.Email) ?? Get(ClaimTypes.Email);
            var name = Get("userName");

            var roleStr = Get(ClaimTypes.Role) ?? Get("role");
            Enum.TryParse(roleStr, true, out UserRole role);

            int? companyId = null;
            var companyStr = Get("companyId");
            if (int.TryParse(companyStr, out var companyParsed))
                companyId = companyParsed;

            return new IdentityUserModel
            {
                UserId = userId,
                Email = email,
                UserName = name,
                Role = role,
                Company = companyId,
            };
        }
    }
}
