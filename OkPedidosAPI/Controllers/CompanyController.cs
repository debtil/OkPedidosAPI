using Microsoft.AspNetCore.Mvc;
using OkPedidos.Core.Result;
using OkPedidos.Core.Services.Interfaces;
using OkPedidos.Models.DTOs.Request.Company;
using OkPedidos.Models.DTOs.Response.Company;

namespace OkPedidosAPI.Controllers
{
    [ApiController]
    //[ApiExplorerSettings(GroupName = "admin")]
    [Route("admin/v1/companies")]
    //[Authorize(Roles = $"{nameof(UserRole.ADMIN)},{nameof(UserRole.MANAGER)}")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        /// <summary>Cadastra um novo usuário.</summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Result<CreateCompanyResponse>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(Result<CreateCompanyResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(Result<CreateCompanyResponse>))]
        public async Task<IActionResult> Create([FromBody] CreateCompanyRequest value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _companyService.Create(value);

            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>Atualiza um usuário.</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateCompanyResponse>))]
        public async Task<IActionResult> Update(int id, [FromBody] CreateCompanyRequest value)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _companyService.Update(id, value);
            return Ok(result);
        }

        /// <summary>Consulta usuário por ID.</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateCompanyResponse>))]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _companyService.GetById(id);
            return Ok(result);
        }

        /// <summary>Consulta todos os usuários.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResultPaged<List<CreateCompanyResponse>>))]
        public async Task<IActionResult> GetAll([FromQuery] Dictionary<string, string> filters)
        {
            var result = await _companyService.GetAll(filters);
            return Ok(result);
        }

        /// <summary>Deleta (soft delete) um usuário.</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateCompanyResponse>))]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _companyService.Delete(id);
            return Ok(result);
        }

        /// <summary>Busca os funcionários de uma empresa</summary>
        [HttpGet("{id:int}/employees")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<CreateCompanyResponse>))]
        public async Task<IActionResult> Emplyees(int id)
        {
            var result = await _companyService.GetCompanyEmployees(id);
            return Ok(result);
        }
    }
}

