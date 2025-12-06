using MediatR;

namespace Blog.Api.Authentication.Requests.Register;

public class RegisterUserRequest : IRequest<RegisterUserResponse>
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
