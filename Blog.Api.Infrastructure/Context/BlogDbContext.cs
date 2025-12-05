// Em Blog.Api.Infrastructure/Context/BlogDbContext.cs
using Blog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    // Tabela de Postagens
    public DbSet<Postagem> Postagens { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Configurações de mapeamento de entidades podem vir aqui (EntityConfig).
    }
}