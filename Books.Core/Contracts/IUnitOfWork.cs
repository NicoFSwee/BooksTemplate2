using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Books.Core.Contracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IBookRepository Books { get; }
        IAuthorRepository Authors { get; }

        Task<int> SaveChangesAsync();
        Task DeleteDatabaseAsync();
        Task MigrateDatabaseAsync();
        Task CreateDatabaseAsync();
    }
}
