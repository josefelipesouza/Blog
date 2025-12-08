using MediatR;
using ErrorOr;
using Blog.Api.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using Blog.Api.Application.Errors;
using System.Security.Claims;


namespace Blog.Api.Application.Handlers.Post.Cadastrar;

    public class CadastrarPostagemHandler : IRequestHandler<CadastrarPostagemRequest, ErrorOr<CadastrarPostagemResponse>>
    {
        private readonly IPostagemRepository _repo;
        private readonly IHttpContextAccessor _http;

        public CadastrarPostagemHandler(IPostagemRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task<ErrorOr<CadastrarPostagemResponse>> Handle(
            CadastrarPostagemRequest request, CancellationToken cancellationToken)
        {
            // Validação
            var validator = new CadastrarPostagemRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return validationResult.Errors
                    .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                    .ToList();

            // Captura do usuário logado
            var usuarioId = _http.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (usuarioId is null)
                return ApplicationErrors.UsuarioNaoAutenticado;

            // Criação da entidade
            var postagem = new  Blog.Api.Domain.Entities.Postagem
            {
                Id = Guid.NewGuid(),
                Titulo = request.Titulo,
                Conteudo = request.Conteudo,
                AutorId = usuarioId,
                DataCriacao = DateTime.UtcNow
            };

            // Persistência
            await _repo.AdicionarAsync(postagem, cancellationToken);
            await _repo.UnitOfWork.CommitAsync(cancellationToken);

            // Retorno
            return new CadastrarPostagemResponse(postagem.Id, postagem.Titulo, postagem.Conteudo, postagem.DataCriacao);
        }
    }

