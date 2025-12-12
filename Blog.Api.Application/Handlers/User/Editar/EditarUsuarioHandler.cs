using System.Linq;
using System.Security.Claims;
using ErrorOr;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities; // Para ApplicationUser
using Blog.Api.Application.Errors; 
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Api.Application.Handlers.User.Editar;

public class EditarUsuarioHandler 
    : IRequestHandler<EditarUsuarioRequest, ErrorOr<EditarUsuarioResponse>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _http;

    public EditarUsuarioHandler(UserManager<ApplicationUser> userManager, IHttpContextAccessor http)
    {
        _userManager = userManager;
        _http = http;
    }

    public async Task<ErrorOr<EditarUsuarioResponse>> Handle(
        EditarUsuarioRequest request, CancellationToken cancellationToken)
    {
        // 1. Validação da Requisição (FluentValidation)
        var validator = new EditarUsuarioRequestValidator();
        var validationResult = validator.Validate(request);
        
        if (!validationResult.IsValid)
            return validationResult.Errors
                .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
                .ToList();

        // 2. Autenticação: Obter o ID do usuário logado
        var usuarioLogadoId = _http.HttpContext?.User?
            .Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(usuarioLogadoId))
            return ApplicationErrors.UsuarioNaoAutenticado;

        // --- PONTO 3: AUTORIZAÇÃO RESTRITA (SOMENTE ADMIN PODE EDITAR) ---

        // 3.1 Buscar o ApplicationUser do usuário logado para verificar as roles
        var usuarioLogado = await _userManager.FindByIdAsync(usuarioLogadoId);

        // Se o usuário logado não for encontrado
        if (usuarioLogado is null)
        {
            return ApplicationErrors.UsuarioAcessoNegado;
        }

        // 3.2 Verificar se o usuário logado tem a role "Admin"
        var isUserAdmin = await _userManager.IsInRoleAsync(usuarioLogado, "Admin");

        if (!isUserAdmin)
        {
            // Se o usuário logado NÃO é Admin, nega o acesso.
            // Isso se aplica mesmo se request.Id == usuarioLogadoId.
            return ApplicationErrors.UsuarioAcessoNegado; 
        }
        
        // --- FIM DA AUTORIZAÇÃO ---
        
        // Se chegou aqui, o usuário logado é um Admin e pode prosseguir com a edição.

        // 4. Buscar o usuário que será editado
        var user = await _userManager.FindByIdAsync(request.Id);

        if (user is null)
            return ApplicationErrors.UsuarioNaoEncontrado;

        // 5. Verificar se o novo UserName já está em uso
        var existingUser = await _userManager.FindByNameAsync(request.NovoUserName);
        if (existingUser is not null && existingUser.Id != user.Id)
        {
            return Error.Conflict(code: "Usuario.UsernameEmUso", description: "O nome de usuário já está em uso por outra conta.");
        }

        // 6. Aplicar a mudança
        var result = await _userManager.SetUserNameAsync(user, request.NovoUserName);

        if (!result.Succeeded)
        {
            var identityErrors = result.Errors
                .Select(e => Error.Validation(code: e.Code, description: e.Description))
                .ToList();
            
            return identityErrors;
        }

        // 7. Sucesso
        return new EditarUsuarioResponse(
            user.Id,
            user.UserName!, 
            user.Email!
        );
    }
}