//using Blog.Api.Authentication.Requests.Register;
using Blog.Api.Authentication.Requests.Login;
using Blog.Api.Authentication.Requests.Logout;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Blog.Api.Application.Handlers.User.Listar;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;

namespace Blog.Api.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IMediator mediator, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

          

   

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Blog.Api.Authentication.Requests.Register.RegisterUserRequest request)
        {
            var response = await _mediator.Send(request);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value ?? "";

            var response = await _mediator.Send(new LogoutUserRequest
            {
                UserId = userId
            });

            return Ok(response);
        }

        
        [HttpGet("users")]
        [Authorize(Roles = "Admin")] // Protege o endpoint
        public async Task<IActionResult> ListarUsuarios()
        {
            var result = await _mediator.Send(new ListarUsuariosRequest());

            return result.Match(
                sucesso => Ok(sucesso),
                // Se falhar, pode ser um erro de permissão (403 Forbidden) ou outro erro
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }

        // Endpoint temporário para debug - verificar roles do usuário
        [HttpGet("debug/roles")]
        public async Task<IActionResult> DebugRoles([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email é obrigatório");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Usuário não encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { Email = email, Roles = roles, RoleCount = roles.Count });
        }

        // Endpoint temporário para atribuir role Admin ao usuário
        [HttpPost("debug/assign-admin")]
        public async Task<IActionResult> AssignAdminRole([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest("Email é obrigatório");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("Usuário não encontrado");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Admin"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    var updatedRoles = await _userManager.GetRolesAsync(user);
                    return Ok(new { Message = "Role Admin atribuída com sucesso", Email = email, Roles = updatedRoles });
                }
                return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
            }

            return Ok(new { Message = "Usuário já possui role Admin", Email = email, Roles = roles });
        }
        
        

    }
}