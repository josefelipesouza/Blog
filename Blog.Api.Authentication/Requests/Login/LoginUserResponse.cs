namespace Blog.Api.Authentication.Requests.Login;

public class LoginUserResponse
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    
    // 🎯 ADIÇÃO: Propriedade para retornar as roles do usuário
    public required IReadOnlyList<string> Roles { get; set; }
}