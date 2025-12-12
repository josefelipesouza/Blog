using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Blog.Api.Application.Handlers.User.Editar;
using Blog.Api.Application.Handlers.User.Excluir;

namespace Blog.Api.API.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    [Authorize]
    public class UsuarioController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsuarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public record EditarUsuarioPayload(string NovoUserName);

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Editar(Guid id, [FromBody] EditarUsuarioPayload payload)
        {
            var request = new EditarUsuarioRequest(id.ToString(), payload.NovoUserName);

            var result = await _mediator.Send(request);

            return result.Match(
                sucesso => Ok(sucesso),
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var result = await _mediator.Send(new ExcluirUsuarioRequest(id.ToString()));

            return result.Match<IActionResult>(
                _ => Ok(),
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }
    }
}
