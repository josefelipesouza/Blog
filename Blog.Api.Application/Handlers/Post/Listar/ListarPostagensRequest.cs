
using ErrorOr;
using MediatR;

namespace Blog.Api.Application.Handlers.Post.Listar;
public record ListarPostagensRequest() : IRequest<ErrorOr<IEnumerable<ListarPostagensResponse>>>;
