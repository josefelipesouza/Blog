
namespace Blog.Api.Application.Handlers.Postagem.Listar;
public record ListarPostagensRequest() : IRequest<ErrorOr<IEnumerable<ListarPostagensResponse>>>;
