using MediatR;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Authentication.Requests.Login;
using Blog.Api.Authentication.Services;

namespace Blog.Api.Authentication.Handlers;

    public class LoginUsuarioHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenService _jwtService;

        public LoginUsuarioHandler(
            UserManager<ApplicationUser> userManager,
            JwtTokenService jwtService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
        }

        public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
                throw new Exception("Usuário não encontrado.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
                throw new Exception("Senha inválida.");

            var token = _jwtService.GenerateToken(
                user.Id,
                user.UserName ?? throw new Exception("UserName inválido."),
                user.Email ?? throw new Exception("Email inválido.")
            );

            return new LoginUserResponse
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!
            };
        }
    }

