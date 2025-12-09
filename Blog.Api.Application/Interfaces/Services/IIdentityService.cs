// Blog.Api.Application/Interfaces/Services/IIdentityService.cs
using System.Collections.Generic;
using System.Threading.Tasks;
using Blog.Api.Application.Handlers.User.Listar;

namespace Blog.Api.Application.Interfaces.Services;

public interface IIdentityService
{
    // Retorna uma lista de DTOs de usuário com suas roles (funções)
    Task<IEnumerable<UsuarioResponse>> ListarUsuariosComRolesAsync();
}