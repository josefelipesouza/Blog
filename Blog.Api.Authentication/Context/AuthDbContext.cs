using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blog.Api.Authentication.Entities;

namespace Blog.Api.Authentication.Context;

    public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Adicionar a restrição de unicidade para o NormalizedEmail
            // Isso garante que o EF Core gere o comando para o índice UNIQUE.
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.NormalizedEmail)
                .IsUnique() // <--- Linha ESSENCIAL
                .HasName("EmailIndex"); // Opcional: mantém o nome original do índice

            // Opcional, mas útil: Força a unicidade do NormalizedUserName também (já está lá por padrão)
            // builder.Entity<ApplicationUser>()
            //     .HasIndex(u => u.NormalizedUserName)
            //     .IsUnique();
        }
    }