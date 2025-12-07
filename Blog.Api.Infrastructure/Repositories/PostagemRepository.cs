using Blog.Api.Application.Interfaces.Data;
using Blog.Api.Application.Interfaces.Repositories;
using Blog.Api.Domain.Entities;
using Blog.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Infrastructure.Repositories;

public class PostagemRepository : IPostagemRepository
{
    public IUnityOfWork UnitOfWork => _context;
    private readonly BlogDbContext _context;

    public PostagemRepository(BlogDbContext context)
    {
        _context = context;
    }

    public async Task AdicionarAsync(Postagem postagem, CancellationToken cancellationToken)
    {
        await _context.Postagens.AddAsync(postagem, cancellationToken);
    }

    public async Task<Postagem?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Postagens.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Postagem>> ListarAsync(CancellationToken cancellationToken)
    {
        return await _context.Postagens.AsNoTracking().ToListAsync(cancellationToken);
    }

    public void Atualizar(Postagem postagem)
    {
        _context.Postagens.Update(postagem);
    }

    public void Remover(Postagem postagem)
    {
        _context.Postagens.Remove(postagem);
    }
}
