using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ErrorOr;
using Blog.Api.Application.Interfaces.Repositories;
using Blog.Api.Application.Handlers.Post.Listar;

namespace Blog.Api.Application.Handlers.Post.Listar;

public class ListarPostagensHandler :
    IRequestHandler<ListarPostagensRequest, ErrorOr<IEnumerable<ListarPostagensResponse>>>
{
    private readonly IPostagemRepository _repo;

    public ListarPostagensHandler(IPostagemRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<IEnumerable<ListarPostagensResponse>>> Handle(
        ListarPostagensRequest request, CancellationToken cancellationToken)
    {
        var posts = await _repo.ListarAsync(cancellationToken);

        var response = posts.Select(p =>
            new ListarPostagensResponse(
                p.Id,
                p.Titulo,
                p.Conteudo,
                p.DataCriacao,
                p.AutorId
            )).ToList();

        return response;
    }
}
