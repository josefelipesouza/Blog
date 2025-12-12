using MediatR;
using ErrorOr;
using FluentValidation; // Importante para o Validator

namespace Blog.Api.Application.Handlers.User.Editar;

// --------------------------------------------------------
// REQUEST (Registro público)
// --------------------------------------------------------

/// <summary>
/// Requisição para editar o UserName de um usuário.
/// </summary>
public record EditarUsuarioRequest(string Id, string NovoUserName)
    : IRequest<ErrorOr<EditarUsuarioResponse>>;

// --------------------------------------------------------
// VALIDATOR (Classe pública)
// --------------------------------------------------------

/// <summary>
/// Validador para a requisição de edição de usuário.
/// </summary>
public class EditarUsuarioRequestValidator : AbstractValidator<EditarUsuarioRequest>
{
    public EditarUsuarioRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do usuário é obrigatório.");

        RuleFor(x => x.NovoUserName)
            .NotEmpty().WithMessage("O novo nome de usuário é obrigatório.")
            .MaximumLength(256).WithMessage("O nome de usuário deve ter no máximo 256 caracteres.")
            // Garante que o nome de usuário não contenha espaços em branco
            .Matches(@"^\S+$").WithMessage("O nome de usuário não pode conter espaços em branco."); 
    }
}