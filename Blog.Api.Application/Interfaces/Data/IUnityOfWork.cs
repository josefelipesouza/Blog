namespace Blog.Api.Application.Interfaces.Data;

public interface IUnityOfWork 
{
    Task CommitAsync(CancellationToken cancellationToken = default);
}