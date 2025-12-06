using Blog.Api.Authentication.Requests.Register;
using Blog.Api.Authentication.Requests.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            return Ok(await _mediator.Send(request));
        }
    }
}
