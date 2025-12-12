using System.Linq;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities; // Para ApplicationUser
using Blog.Api.Application.Errors; // Assumindo que você tem um ApplicationErrors

namespace Blog.Api.Application.Handlers.User.Excluir;

public class ExcluirUsuarioHandler : IRequestHandler<ExcluirUsuarioRequest, ErrorOr<ExcluirUsuarioResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;

    // Nota: O acesso ao HttpContext para verificar se o usuário logado é Administrador
    // ou se o usuário não está excluindo a si mesmo deve ser adicionado aqui,
    // se você precisar dessa lógica de autorização no Handler.

    public ExcluirUsuarioHandler(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ErrorOr<ExcluirUsuarioResponse>> Handle(ExcluirUsuarioRequest request, CancellationToken cancellationToken)
    {
        // 1. Buscar o usuário pelo ID
        var user = await _userManager.FindByIdAsync(request.Id);

        if (user == null)
        {
            // Retorna um erro caso o usuário não seja encontrado
            return ApplicationErrors.UsuarioNaoEncontrado; // Assumindo que você tem este erro definido
        }

        // 2. Excluir o usuário
        var result = await _userManager.DeleteAsync(user);

        // 3. Verificar o resultado da exclusão
        if (!result.Succeeded)
        {
            // Converte os erros do Identity para o formato ErrorOr
            var identityErrors = result.Errors
                .Select(e => Error.Unexpected(code: e.Code, description: e.Description))
                .ToList();
            
            return identityErrors;
        }

        // 4. Sucesso
        return new ExcluirUsuarioResponse(true, request.Id);
    }
}