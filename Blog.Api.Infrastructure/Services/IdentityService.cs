// Blog.Api.Infrastructure/Services/IdentityService.cs

using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Application.Interfaces.Services;
using Blog.Api.Application.Handlers.User.Listar;

namespace Blog.Api.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UsuarioResponse>> ListarUsuariosComRolesAsync()
    {
        var users = _userManager.Users.ToList();
        var userResponses = new List<UsuarioResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            
            // Simplesmente pega a primeira role para demonstração, ou use um padrão 'User' se não houver
            var role = roles.FirstOrDefault() ?? "User"; 

            userResponses.Add(new UsuarioResponse(
                user.Id,
                user.Email ?? "Sem Email",
                role
            ));
        }

        return userResponses;
    }
}