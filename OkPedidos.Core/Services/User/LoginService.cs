using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OkPedidos.Core.Data;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Base;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;
using OkPedidos.Shared.Constants.Models;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace OkPedidos.Core.Services.User
{
    public class LoginService
    {
        private readonly OkPedidosDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public LoginService(OkPedidosDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task<Result<LoginResponse>> Authenticate(LoginRequest request)
        {
            if(string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return ResultService.OK<LoginResponse>(HttpStatusCode.BadRequest, ErrorMessage.MissingEmailOrPassword);

            var user = await _dbContext.User.FirstOrDefaultAsync(x => x.Email == request.Email && x.Password == request.Password && x.DeletedAt == null);
            if(user == null)
                return ResultService.OK<LoginResponse>(HttpStatusCode.Unauthorized, ErrorMessage.UserNotFound);

            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMinsSection = _configuration.GetSection("JwtConfig:TokenValidityMins");
            if (!int.TryParse(tokenValidityMinsSection.Value, out var tokenValidityMins))
                return ResultService.OK<LoginResponse>(HttpStatusCode.Unauthorized, ErrorMessage.InvalidTokenExpiration);

            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, request.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                }),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha512Signature),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securitytoken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securitytoken);

            return ResultService.OK(HttpStatusCode.OK, new LoginResponse
            {
                AccessToken = accessToken,
                UserId = user.Id,
                Email = request.Email,
                Expiration = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds
            });
        }
    }
}
