
namespace Blog.Api.Application.Handlers.User.Listar;

public record UsuarioResponse(
    string Id,
    string Email,
    string Role
);