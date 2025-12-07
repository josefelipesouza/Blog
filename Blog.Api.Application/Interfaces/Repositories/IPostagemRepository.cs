// Blog.Api.Application/Interfaces/Repositories/IPostagemRepository.cs
using Blog.Api.Domain.Entities;

namespace Blog.Api.Application.Interfaces.Repositories;

public interface IPostagemRepository
{
    IUnityOfWork UnitOfWork { get; }

    Task AdicionarAsync(Postagem postagem, CancellationToken cancellationToken);
    Task<Postagem?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Postagem>> ListarAsync(CancellationToken cancellationToken);
    void Atualizar(Postagem postagem);
    void Remover(Postagem postagem); // Mantido, mas só usaremos Atualizar para Inativação
}