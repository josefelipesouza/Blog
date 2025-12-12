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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListarUsuarios()
        {
            var result = await _mediator.Send(new ListarUsuariosRequest());

            return result.Match(
                sucesso => Ok(sucesso),
              
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }

   
        
        

    }
}