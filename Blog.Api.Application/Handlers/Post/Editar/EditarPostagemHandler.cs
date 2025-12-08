using System.Linq;
using System.Security.Claims;
using ErrorOr;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Blog.Api.Application.Errors;
using Blog.Api.Application.Interfaces.Repositories;

namespace Blog.Api.Application.Handlers.Post.Editar;

public class EditarPostagemHandler 
    : IRequestHandler<EditarPostagemRequest, ErrorOr<EditarPostagemResponse>>
{
    private readonly IPostagemRepository _repo;
    private readonly IHttpContextAccessor _http;

    public EditarPostagemHandler(IPostagemRepository repo, IHttpContextAccessor http)
    {
        _repo = repo;
        _http = http;
    }

    public async Task<ErrorOr<EditarPostagemResponse>> Handle(
        EditarPostagemRequest request, CancellationToken cancellationToken)
    {
        var validator = new EditarPostagemRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return validationResult.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        var usuarioId = _http.HttpContext?.User?
            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioId))
            return ApplicationErrors.UsuarioNaoAutenticado;

        var postagem = await _repo.BuscarPorIdAsync(request.Id, cancellationToken);

        if (postagem is null)
            return ApplicationErrors.PostagemNaoEncontrada;

        if (postagem.AutorId != usuarioId)
            return ApplicationErrors.PostagemAcessoNegado;

        postagem.Titulo = request.Titulo;
        postagem.Conteudo = request.Conteudo;
        postagem.AtualizadoEm = DateTime.UtcNow;

        _repo.Atualizar(postagem);
        await _repo.UnitOfWork.CommitAsync(cancellationToken);

        return new EditarPostagemResponse(
            postagem.Id,
            postagem.Titulo,
            postagem.Conteudo,
            postagem.DataCriacao,
            postagem.AtualizadoEm
        );
    }
}
