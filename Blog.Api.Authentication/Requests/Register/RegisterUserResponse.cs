namespace Blog.Api.Authentication.Requests.Register;

public class RegisterUserResponse
{
    
    public required bool Success { get; set; } 
    
    
    public string? UserId { get; set; } 
    public string? Message { get; set; } 
    
  
    public IEnumerable<string>? Errors { get; set; }
}