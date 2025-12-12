using MediatR;
using ErrorOr;
using Blog.Api.Application.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Blog.Api.Application.Handlers.User.Listar
{
    public class ListarUsuariosHandler :
        IRequestHandler<ListarUsuariosRequest, ErrorOr<IEnumerable<UsuarioResponse>>>
    {
        private readonly IIdentityService _identityService;

        public ListarUsuariosHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ErrorOr<IEnumerable<UsuarioResponse>>> Handle(
            ListarUsuariosRequest request,
            CancellationToken cancellationToken)
        {
            var usuarios = await _identityService.ListarUsuariosComRolesAsync();

            if (usuarios == null)
                return Error.Failure(description: "Não foi possível listar usuários.");

            // Correção do tipo
            return ErrorOrFactory.From(usuarios);
        }
    }
}
