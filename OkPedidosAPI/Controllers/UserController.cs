using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Models.DTOs.Request.User;
using OkPedidos.Models.DTOs.Response.User;
using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidosAPI.Controllers
{
    [ApiController]
    //[ApiExplorerSettings(GroupName = "admin")]
    [Route("admin/v1/users")]
    [Authorize(Roles = $"{nameof(UserRole.ADMIN)},{nameof(UserRole.MANAGER)}")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>Cadastra um novo usuário.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<CreateUserResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<CreateUserResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Result<CreateUserResponse>))]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Create(value);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Atualiza um usuário.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateUserResponse>))]
        public async Task<IActionResult> Update(int id, [FromBody] CreateUserRequest value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.Update(id, value);
            return Ok(result);
        }

        /// <summary>Consulta usuário por ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateUserResponse>))]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetById(id);
            return Ok(result);
        }

        /// <summary>Consulta todos os usuários.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultPaged<List<CreateUserResponse>>))]
        public async Task<IActionResult> GetAll([FromQuery] Dictionary<string, string> filters)
        {
            var result = await _userService.GetAll(filters);
            return Ok(result);
        }

        /// <summary>Deleta (soft delete) um usuário.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateUserResponse>))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }
    }
}
