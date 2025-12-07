
namespace Blog.Api.Application.Handlers.Postagem.Inativar;
public class ExcluirPostagemHandler : IRequestHandler<ExcluirPostagemRequest, ErrorOr<bool>>
{
    private readonly IPostagemRepository _repo;
    private readonly IHttpContextAccessor _http;

    public ExcluirPostagemHandler(IPostagemRepository repo, IHttpContextAccessor http)
    {
        _repo = repo;
        _http = http;
    }

    public async Task<ErrorOr<bool>> Handle(ExcluirPostagemRequest request, CancellationToken cancellationToken)
    {
        var usuarioId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (usuarioId == null)
            return Error.Unauthorized("Usuário não autenticado");

        var postagem = await _repo.BuscarPorIdAsync(request.Id, cancellationToken);
        if (postagem == null)
            return Error.NotFound("Postagem não encontrada");

        if (postagem.AutorId != usuarioId)
            return Error.Unauthorized("Somente o autor pode excluir");

        _repo.Remover(postagem);
        await _repo.UnitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}
