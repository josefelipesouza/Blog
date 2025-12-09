using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Blog.Api.Authentication.Entities;
using Blog.Api.Domain.Enums;

namespace Blog.Api.Infrastructure.Seed;

    public static class AuthDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager  = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager  = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            await CriarRoles(roleManager);
            await CriarUsuarioAdmin(userManager);
        }

        private static async Task CriarRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in Enum.GetNames(typeof(PerfilDeAcessoBlog)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task CriarUsuarioAdmin(UserManager<ApplicationUser> userManager)
        {
            const string emailAdmin = "Admin@gmail.com.br";
            const string usernameAdmin = "Admin";
            const string senha = "Admin122025";

            var usuarioExistente = await userManager.FindByEmailAsync(emailAdmin);
            if (usuarioExistente != null)
            {
                // Garante que está na role Admin mesmo que já exista
                await GarantirRoleAdmin(userManager, usuarioExistente);
                return;
            }

            var user = new ApplicationUser
            {
                UserName = usernameAdmin,
                Email = emailAdmin,
                EmailConfirmed = true
            };

            var createUser = await userManager.CreateAsync(user, senha);
            if (!createUser.Succeeded)
                throw new Exception(string.Join(", ", createUser.Errors.Select(e => e.Description)));

            await GarantirRoleAdmin(userManager, user);
        }

        private static async Task GarantirRoleAdmin(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            var roles = await userManager.GetRolesAsync(user);

            if (!roles.Contains(PerfilDeAcessoBlog.Admin.ToString()))
                await userManager.AddToRoleAsync(user, PerfilDeAcessoBlog.Admin.ToString());
        }
    }


