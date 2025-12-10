using Blog.Api.Authentication.Requests.Register;
using MediatR;
using Blog.Api.Authentication.Entities;
using Microsoft.AspNetCore.Identity;

namespace Blog.Api.Authentication.Handlers
{
    public class RegistrarUsuarioHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistrarUsuarioHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var user = new ApplicationUser
            {
                UserName = request.Username,  
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Falha no registro do usuário devido à validação.",
                    Errors = result.Errors.Select(e => e.Description)
                };
            }

            return new RegisterUserResponse
            {
                Success = true,
                UserId = user.Id,
                Message = "Usuário registrado com sucesso!"
            };
        }
    }
}
