// Em Blog.Api.Authentication/Context/AuthDbContext.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

// O IdentityDbContext usa a tabela de IdentityUser (padrão)
namespace Blog.Api.Authentication.Context;
public class AuthDbContext : IdentityDbContext<IdentityUser>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }
    
    // Adiciona o mapeamento para usar o esquema 'Identity' no banco (opcional, mas recomendado para separar)
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("Identity"); // Coloca as tabelas Identity sob o schema "Identity"
    }
}