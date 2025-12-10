using MediatR;

namespace Blog.Api.Authentication.Requests.Register
{
    public class RegisterUserRequest : IRequest<RegisterUserResponse>
    {
        public string Username { get; set; } = string.Empty;   
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
