using MediatR;
using ErrorOr;
using FluentValidation;
using Blog.Api.Domain.Entities;
using Blog.Api.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.Api.Application.Handlers.Postagem.Cadastrar;

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
            var usuarioId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId is null)
                return Errors.Application.UsuarioErrors.UsuarioNaoAutenticado;

            // Criação da entidade
            var postagem = new Postagem
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

