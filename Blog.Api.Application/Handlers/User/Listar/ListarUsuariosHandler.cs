// Blog.Api.Application.Handlers.User.ListarUsuarios/ListarUsuariosHandler.cs

using MediatR;
using ErrorOr; // Necessário
using Blog.Api.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Api.Application.Handlers.User.Listar;

public class ListarUsuariosHandler :
    IRequestHandler<ListarUsuariosRequest, ErrorOr<IEnumerable<UsuarioResponse>>>
{
    private readonly IIdentityService _identityService;

    public ListarUsuariosHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<ErrorOr<IEnumerable<UsuarioResponse>>> Handle(
        ListarUsuariosRequest request, CancellationToken cancellationToken)
    {
        var usuarios = await _identityService.ListarUsuariosComRolesAsync();
        
        // CORRIGIDO: Retornando explicitamente como um resultado de sucesso (ErrorOr)
        return ErrorOr<IEnumerable<UsuarioResponse>>.From((List<Error>)usuarios); 
    }
}