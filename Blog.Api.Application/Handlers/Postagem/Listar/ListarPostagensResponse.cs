
namespace Blog.Api.Application.Handlers.Postagem.Listar;
public record ListarPostagensResponse(Guid Id, string Titulo, string Conteudo, DateTime DataCriacao, string AutorId);