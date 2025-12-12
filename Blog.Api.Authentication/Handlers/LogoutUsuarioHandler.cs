using MediatR;
using Blog.Api.Authentication.Requests.Logout;

namespace Blog.Api.Authentication.Handlers;

public class LogoutUsuarioHandler : IRequestHandler<LogoutUserRequest, LogoutUserResponse>
{
    public Task<LogoutUserResponse> Handle(LogoutUserRequest request, CancellationToken cancellationToken)
    {
        
        return Task.FromResult(new LogoutUserResponse
        {
            Message = "Logout realizado com sucesso."
        });
    }
}
