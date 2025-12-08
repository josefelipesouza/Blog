
namespace Blog.Api.Application.Handlers.Post.Editar;
public record EditarPostagemResponse(Guid Id, string Titulo, string Conteudo, DateTime DataCriacao, DateTime? AtualizadoEm);
