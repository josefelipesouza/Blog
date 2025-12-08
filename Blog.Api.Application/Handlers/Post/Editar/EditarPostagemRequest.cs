using MediatR;
using ErrorOr;
using FluentValidation;

namespace Blog.Api.Application.Handlers.Post.Editar;

    // Request
    public record EditarPostagemRequest(Guid Id, string Titulo, string Conteudo)
        : IRequest<ErrorOr<EditarPostagemResponse>>;

    // Validator
    public class EditarPostagemRequestValidator : AbstractValidator<EditarPostagemRequest>
    {
        public EditarPostagemRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("O Id da postagem é obrigatório.");

            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

            RuleFor(x => x.Conteudo)
                .NotEmpty().WithMessage("O conteúdo é obrigatório.");
        }
    }

