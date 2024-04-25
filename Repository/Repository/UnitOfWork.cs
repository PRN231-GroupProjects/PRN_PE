using Repository.Interfaces;
using Repository.Persistence;

namespace Repository.Repository;

public class UnitOfWork : IUnitOfWork
{
    private readonly PRN231_SU23_StudentGroupDBContext _dbContext;

    public UnitOfWork(PRN231_SU23_StudentGroupDBContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public IRepository<TEntity> GetRequiredRepository<TEntity>() where TEntity : class
        => new Repository<TEntity>(_dbContext);

    public async Task<int> CommitAsync()
        => await _dbContext.SaveChangesAsync();
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dbContext.Dispose();
        }
    }
}