using System.Data;
using Microsoft.Data.SqlClient;

namespace Test1Retake.Infrastructure;


public sealed class UnitOfWork : IUnitOfWork
{
    private readonly SqlConnection _connection;
    public UnitOfWork(IConfiguration cfg)
    {
        var connectionString = cfg.GetConnectionString("Default");
        _connection = new SqlConnection(connectionString);
    }

    public async ValueTask<SqlConnection> GetConnectionAsync()
    {
        if (_connection.State != ConnectionState.Open)
            await _connection.OpenAsync();

        return _connection;
    }

    public SqlTransaction? Transaction { get; private set; }

    public async Task BeginTransactionAsync()
    {
        var con = await GetConnectionAsync();
        Transaction = con.BeginTransaction();
    }

    public async Task CommitTransactionAsync()
    {
        if (Transaction is not null)
            await Transaction.CommitAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        if (Transaction is not null)
            await Transaction.RollbackAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
            await Transaction.DisposeAsync();

        await _connection.DisposeAsync();
    }
}