using Blog.Api.Application.Interfaces.Data;
using Blog.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Infrastructure.Context
{
    public class BlogDbContext : DbContext, IUnityOfWork
    {
        public BlogDbContext(DbContextOptions<BlogDbContext> options) 
            : base(options)
        {
        }

        public DbSet<Postagem> Postagens { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }
}
