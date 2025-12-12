namespace Blog.Api.Authentication.Requests.Login;

public class LoginUserResponse
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required IReadOnlyList<string> Roles { get; set; }
}