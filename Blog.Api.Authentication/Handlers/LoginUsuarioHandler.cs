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
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new Exception("Usuário não encontrado.");

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
                throw new Exception("Senha inválida.");

            // -------------------------------------------------------------------------
            // 🎯 CORREÇÃO:
            // 1. Chamando GenerateToken com o argumento correto (o objeto 'user').
            // 2. Usando 'await' porque o método agora é async Task<string>.
            // -------------------------------------------------------------------------
            var token = await _jwtService.GenerateToken(user);

            return new LoginUserResponse
            {
                Token = token,
                Username = user.UserName!,
                Email = user.Email!
            };
        }
    }