using Microsoft.Data.SqlClient;

namespace Test1Retake.Infrastructure;

public interface IUnitOfWork : IAsyncDisposable
{
    public ValueTask<SqlConnection> GetConnectionAsync();
    public SqlTransaction? Transaction { get; }
    public Task BeginTransactionAsync();
    public Task CommitTransactionAsync();
    public Task RollbackTransactionAsync();
}