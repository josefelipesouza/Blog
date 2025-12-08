
namespace Blog.Api.Application.Handlers.Post.Listar;
public record ListarPostagensResponse(Guid Id, string Titulo, string Conteudo, DateTime DataCriacao, string AutorId);