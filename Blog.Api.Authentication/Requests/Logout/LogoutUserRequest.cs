using MediatR;

namespace Blog.Api.Authentication.Requests.Logout;

public class LogoutUserRequest : IRequest<LogoutUserResponse>
{
    public string UserId { get; set; } = string.Empty;
}
