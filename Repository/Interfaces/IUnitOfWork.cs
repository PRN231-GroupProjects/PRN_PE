namespace Repository.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity> GetRequiredRepository<TEntity>() where TEntity : class;
    Task<int> CommitAsync();
}