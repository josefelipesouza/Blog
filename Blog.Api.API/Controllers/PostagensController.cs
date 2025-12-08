using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Blog.Api.Application.Handlers.Post.Cadastrar;
using Blog.Api.Application.Handlers.Post.Editar;
using Blog.Api.Application.Handlers.Post.Listar;
using Blog.Api.Application.Handlers.Post.Excluir;

namespace Blog.Api.API.Controllers
{
    [ApiController]
    [Route("api/postagens")]
    public class PostagensController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PostagensController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Criar([FromBody] CadastrarPostagemRequest request)
        {
            var result = await _mediator.Send(request);

            return result.Match(
                sucesso => Ok(sucesso),
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var result = await _mediator.Send(new ListarPostagensRequest());
            return Ok(result.Value);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Editar(Guid id, [FromBody] EditarPostagemRequest request)
        {
            if (id != request.Id)
                return BadRequest("Id do corpo e da rota não conferem.");

            var result = await _mediator.Send(request);

            return result.Match(
                sucesso => Ok(sucesso),
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Excluir(Guid id)
        {
            var result = await _mediator.Send(new ExcluirPostagemRequest(id));

            return result.Match<IActionResult>(
                _ => Ok(),
                erros => Problem(statusCode: 400, detail: erros.First().Description)
            );
        }
    }
}
