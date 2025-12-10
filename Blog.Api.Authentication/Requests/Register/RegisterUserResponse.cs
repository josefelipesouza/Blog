namespace Blog.Api.Authentication.Requests.Register;

public class RegisterUserResponse
{
    // Indica se a operação foi bem-sucedida ou falhou
    public required bool Success { get; set; } 
    
    // Opcional em caso de sucesso
    public string? UserId { get; set; } 
    public string? Message { get; set; } 
    
    // Usado em caso de falha (Success = false)
    public IEnumerable<string>? Errors { get; set; }
}