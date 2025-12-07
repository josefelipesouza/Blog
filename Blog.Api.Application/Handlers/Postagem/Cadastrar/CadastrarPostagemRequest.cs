using MediatR;
using ErrorOr;
using FluentValidation;

namespace Blog.Api.Application.Handlers.Postagem.Cadastrar;
    // Request
    public record CadastrarPostagemRequest(string Titulo, string Conteudo)
        : IRequest<ErrorOr<CadastrarPostagemResponse>>;

    // Validator
    public class CadastrarPostagemRequestValidator : AbstractValidator<CadastrarPostagemRequest>
    {
        public CadastrarPostagemRequestValidator()
        {
            RuleFor(x => x.Titulo)
                .NotEmpty().WithMessage("O título é obrigatório.")
                .MaximumLength(200).WithMessage("O título deve ter no máximo 200 caracteres.");

            RuleFor(x => x.Conteudo)
                .NotEmpty().WithMessage("O conteúdo é obrigatório.");
        }
    }

