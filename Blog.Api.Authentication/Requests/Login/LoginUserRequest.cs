using MediatR;

namespace Blog.Api.Authentication.Requests.Login;

public class LoginUserRequest : IRequest<LoginUserResponse>
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}
