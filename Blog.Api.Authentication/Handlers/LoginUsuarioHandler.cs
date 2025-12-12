using MediatR;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Authentication.Requests.Login;
using Blog.Api.Authentication.Services;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic; // Para IReadOnlyList

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

        // 🎯 NOVA ADIÇÃO: Buscar as roles do usuário
        var roles = await _userManager.GetRolesAsync(user);

        // -------------------------------------------------------------------------
        // Geração do Token JWT (o método GenerateToken deve usar essas roles para criar as claims)
        // -------------------------------------------------------------------------
        var token = await _jwtService.GenerateToken(user); 

        // 🎯 ATUALIZAÇÃO: Incluir as roles na resposta
        return new LoginUserResponse
        {
            Token = token,
            Username = user.UserName!,
            Email = user.Email!,
            Roles = roles.ToList() // Convertendo a coleção retornada para List<string> ou IReadOnlyList<string>
        };
    }
}