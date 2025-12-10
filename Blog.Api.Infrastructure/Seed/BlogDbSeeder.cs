using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Blog.Api.Authentication.Entities;
using Blog.Api.Infrastructure.Context;
using Blog.Api.Domain.Enums;
using Blog.Api.Domain.Entities;

namespace Blog.Api.Infrastructure.Seed;

    public static class BlogDbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var blogContext = serviceProvider.GetRequiredService<BlogDbContext>();

            // 1. Criar Usuários Comuns (se não existirem)
            await CriarUsuariosComuns(userManager);

            // 2. Criar Postagens de Exemplo (se não existirem)
            await CriarPostagens(userManager, blogContext);
        }

        // ==============================================================================
        // MÉTODOS AUXILIARES
        // ==============================================================================

        private static async Task CriarUsuariosComuns(UserManager<ApplicationUser> userManager)
        {
            var usersToSeed = new (string Username, string Email, string Password)[]
            {
                ("User1", "user1@blog.com", "User122025"),
                ("User2", "user2@blog.com", "User222025")
            };

            foreach (var userData in usersToSeed)
            {
                if (await userManager.FindByEmailAsync(userData.Email) == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = userData.Username,
                        Email = userData.Email,
                        EmailConfirmed = true
                    };

                    var createUser = await userManager.CreateAsync(user, userData.Password);

                    if (createUser.Succeeded)
                    {
                        // Atribui o perfil 'Usuario'
                        await userManager.AddToRoleAsync(user, PerfilDeAcessoBlog.Usuario.ToString());
                    }
                    else
                    {
                        throw new Exception($"Falha ao criar usuário {userData.Username}: {string.Join(", ", createUser.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static async Task CriarPostagens(UserManager<ApplicationUser> userManager, BlogDbContext blogContext)
        {
            if (blogContext.Postagens.Any())
            {
                // Postagens já existem, não faz o seeding
                return;
            }

            // Garante que todos os usuários (Admin e Comuns) estão disponíveis
            var adminUser = await userManager.FindByEmailAsync("Admin@gmail.com.br");
            var user1 = await userManager.FindByEmailAsync("user1@blog.com");
            var user2 = await userManager.FindByEmailAsync("user2@blog.com");
            
            if (adminUser == null || user1 == null || user2 == null) return;

            var postagens = new Postagem[]
            {
                // Postagens do Admin
                new Postagem
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Primeira Postagem do Administrador",
                    Conteudo = "Este é o post inicial. Apenas o Admin tem permissão para publicações importantes.",
                    DataCriacao = DateTime.UtcNow.AddDays(-5), // 5 dias atrás
                    AutorId = adminUser.Id,
                    Inativo = false
                },
                new Postagem
                {
                    Id = Guid.NewGuid(),
                    Titulo = "CORS Finalmente Resolvido!",
                    Conteudo = "Após algumas dores de cabeça, a política CORS está configurada e o frontend e backend se comunicam.",
                    DataCriacao = DateTime.UtcNow, // Hoje
                    AutorId = adminUser.Id,
                    Inativo = false
                },

                // Postagens do User1
                new Postagem
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Dicas de Docker Compose",
                    Conteudo = "Aprender a orquestrar serviços como Postgres, .NET e React é crucial para o desenvolvimento moderno.",
                    DataCriacao = DateTime.UtcNow.AddDays(-2), // 2 dias atrás
                    AutorId = user1.Id,
                    Inativo = false
                },

                // Postagens do User2 (exemplo de post inativo)
                new Postagem
                {
                    Id = Guid.NewGuid(),
                    Titulo = "Revisão do Entity Framework Core",
                    Conteudo = "Revisando as funcionalidades de Migrations e Seeding para garantir que o banco esteja sempre atualizado.",
                    DataCriacao = DateTime.UtcNow.AddDays(-10), // 10 dias atrás
                    AutorId = user2.Id,
                    Inativo = true // Esta postagem estará inativa
                }
            };

            await blogContext.Postagens.AddRangeAsync(postagens);
            await blogContext.SaveChangesAsync();
        }
    }
