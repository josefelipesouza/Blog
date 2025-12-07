
namespace Blog.Api.Application.Handlers.Postagem.Inativar;
public record ExcluirPostagemRequest(Guid Id)
    : IRequest<ErrorOr<bool>>;
