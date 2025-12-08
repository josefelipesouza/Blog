namespace Blog.Api.Application.Handlers.Post.Cadastrar;
public record CadastrarPostagemResponse(Guid Id, string Titulo, string Conteudo, DateTime DataCriacao);
