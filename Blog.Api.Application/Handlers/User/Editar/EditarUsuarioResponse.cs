namespace Blog.Api.Application.Handlers.User.Editar;

/// <summary>
/// Resposta após a edição bem-sucedida do usuário.
/// </summary>
public record EditarUsuarioResponse(string Id, string NovoUserName, string Email);