using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OkPedidos.Core.Services.User;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;

namespace OkPedidosAPI.Controllers
{
    [ApiController]
    [Route("v1/login")]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        /// <summary>Realiza o login do usuário com email e senha.</summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Authenticate([FromBody] LoginRequest request)
        {
            var result = await _loginService.Authenticate(request);
            if (result is null)
                return Unauthorized();

            return Ok(result);
        }
    }
}
