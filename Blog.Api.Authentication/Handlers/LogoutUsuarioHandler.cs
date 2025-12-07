using MediatR;
using Blog.Api.Authentication.Requests.Logout;

namespace Blog.Api.Authentication.Handlers;

public class LogoutUsuarioHandler : IRequestHandler<LogoutUserRequest, LogoutUserResponse>
{
    public Task<LogoutUserResponse> Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        // Como JWT é stateless, não há logout no servidor.
        // Apenas retornamos sucesso e o frontend deve apagar o token.

        return Task.FromResult(new LogoutUserResponse
        {
            Message = "Logout realizado com sucesso."//O token deve ser removido pelo cliente
        });
    }
}
