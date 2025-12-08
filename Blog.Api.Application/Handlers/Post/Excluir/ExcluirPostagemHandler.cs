using System.Linq;
using System.Security.Claims;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http;
using Blog.Api.Application.Errors;
using Blog.Api.Application.Interfaces.Repositories;

namespace Blog.Api.Application.Handlers.Post.Excluir;

public class ExcluirPostagemHandler : IRequestHandler<ExcluirPostagemRequest, ErrorOr<ExcluirPostagemResponse>>
{
    private readonly IPostagemRepository _repo;
    private readonly IHttpContextAccessor _http;

    public ExcluirPostagemHandler(IPostagemRepository repo, IHttpContextAccessor http)
    {
        _repo = repo;
        _http = http;
    }

    public async Task<ErrorOr<ExcluirPostagemResponse>> Handle(ExcluirPostagemRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = _http.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioId))
            return ApplicationErrors.UsuarioNaoAutenticado;

        var postagem = await _repo.BuscarPorIdAsync(request.Id, cancellationToken);

        if (postagem == null)
            return ApplicationErrors.PostagemNaoEncontrada;

        if (postagem.AutorId != usuarioId)
            return ApplicationErrors.PostagemAcessoNegado;

        _repo.Remover(postagem);
        await _repo.UnitOfWork.CommitAsync(cancellationToken);

        return new ExcluirPostagemResponse(true, postagem.Id);
    }
}
