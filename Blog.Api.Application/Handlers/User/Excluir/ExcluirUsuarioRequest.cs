using MediatR;
using ErrorOr;

namespace Blog.Api.Application.Handlers.User.Excluir;

/// <summary>
/// Requisição para excluir um usuário pelo seu ID.
/// </summary>
public record ExcluirUsuarioRequest(string Id)
    : IRequest<ErrorOr<ExcluirUsuarioResponse>>;