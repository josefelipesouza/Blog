using ErrorOr;
using MediatR;

namespace Blog.Api.Application.Handlers.Post.Excluir;

public record ExcluirPostagemRequest(Guid Id)
    : IRequest<ErrorOr<ExcluirPostagemResponse>>;
