using Blog.Api.Authentication.Requests.Register;
using MediatR;
using Blog.Api.Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Authentication.Handlers;

    public class RegistrarUsuarioHandler : IRequestHandler<RegisterUserRequest, RegisterUserResponse>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegistrarUsuarioHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            // Normalize e trim do input para comparação segura
            var emailNormalized = request.Email?.Trim().ToUpperInvariant() ?? string.Empty;
            var userNameNormalized = request.Username?.Trim().ToUpperInvariant() ?? string.Empty;

            // Checagem direta no store (consulta ao IQueryable Users) - é à prova de "bypass"
            var emailExists = await _userManager.Users
                .AsNoTracking()
                .AnyAsync(u => u.NormalizedEmail == emailNormalized, cancellationToken);

            if (emailExists)
            {
                errors.Add("Email já está em uso.");
                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Falha no registro do usuário.",
                    Errors = errors
                };
            }

            var userNameExists = await _userManager.Users
                .AsNoTracking()
                .AnyAsync(u => u.NormalizedUserName == userNameNormalized, cancellationToken);

            if (userNameExists)
            {
                errors.Add("Nome de usuário já está em uso.");
                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Falha no registro do usuário.",
                    Errors = errors
                };
            }

            // Criar novo usuário (UserName = Username ou Email conforme sua regra)
            var user = new ApplicationUser
            {
                UserName = request.Username?.Trim(), // ou request.Email se preferir username = email
                Email = request.Email?.Trim()
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

