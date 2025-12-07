using MediatR;
using ErrorOr;
using FluentValidation;
using Blog.Api.Domain.Entities;
using Blog.Api.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Blog.Api.Application.Handlers.Postagem.Editar;

    public class EditarPostagemHandler : IRequestHandler<EditarPostagemRequest, ErrorOr<EditarPostagemResponse>>
    {
        private readonly IPostagemRepository _repo;
        private readonly IHttpContextAccessor _http;

        public EditarPostagemHandler(IPostagemRepository repo, IHttpContextAccessor http)
        {
            _repo = repo;
            _http = http;
        }

        public async Task<ErrorOr<EditarPostagemResponse>> Handle(
            EditarPostagemRequest request, CancellationToken cancellationToken)
        {
            // Validação
            var validator = new EditarPostagemRequestValidator();
            var validationResult = validator.Validate(request);
            if (!validationResult.IsValid)
                return validationResult.Errors
                    .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                    .ToList();

            // Captura do usuário logado
            var usuarioId = _http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (usuarioId is null)
                return Errors.Application.UsuarioErrors.UsuarioNaoAutenticado;

            // Busca da postagem
            var postagem = await _repo.BuscarPorIdAsync(request.Id, cancellationToken);
            if (postagem is null)
                return Errors.Application.NotFound;

            // Somente o autor pode editar
            if (postagem.AutorId != usuarioId)
                return Error.Unauthorized("Somente o autor da postagem pode editar.");

            // Atualização da postagem
            postagem.Titulo = request.Titulo;
            postagem.Conteudo = request.Conteudo;

            _repo.Atualizar(postagem);
            await _repo.UnitOfWork.CommitAsync(cancellationToken);

            // Retorno
            return new EditarPostagemResponse(postagem.Id, postagem.Titulo, postagem.Conteudo, postagem.DataCriacao);
        }
    }

