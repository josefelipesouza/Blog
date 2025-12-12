namespace Blog.Api.Application.Handlers.User.Excluir;

/// <summary>
/// Resposta para a exclusão de um usuário.
/// </summary>
public record ExcluirUsuarioResponse(bool Sucesso, string IdUsuarioExcluido);