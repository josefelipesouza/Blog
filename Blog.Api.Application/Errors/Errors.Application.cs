using ErrorOr;

namespace Blog.Api.Application.Errors;

public static class ApplicationErrors
{
    // Erros de Usuário
    public static readonly Error UsuarioNaoAutenticado =
        Error.Unauthorized(
            code: "Usuario.NaoAutenticado",
            description: "Usuário não autenticado.");
    public static readonly Error UsuarioNaoEncontrado =
        Error.NotFound(
            code: "Usuario.NaoEncontrado",
            description: "Usuário não encontrado.");
    public static readonly Error UsuarioAcessoNegado =
        Error.Unauthorized(
            code: "Usuario.AcessoNegado",
            description: "Somente Admin pode excluir usuário.");

    // Erros de Postagem
    public static readonly Error PostagemNaoEncontrada =
        Error.NotFound(
            code: "Postagem.NaoEncontrada",
            description: "Postagem não encontrada.");

    public static readonly Error PostagemAcessoNegado =
        Error.Unauthorized(
            code: "Postagem.AcessoNegado",
            description: "Somente o autor pode excluir esta postagem.");

            
}
