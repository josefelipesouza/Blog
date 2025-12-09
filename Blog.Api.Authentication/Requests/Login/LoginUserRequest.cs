using MediatR;

namespace Blog.Api.Authentication.Requests.Login;

public class LoginUserRequest : IRequest<LoginUserResponse>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}
