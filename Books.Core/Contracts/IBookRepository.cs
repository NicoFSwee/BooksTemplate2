using Books.Core.DataTransferObjects;
using Books.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Books.Core.Contracts
{
    public interface IBookRepository
    {
        Task AddRangeAsync(IEnumerable<Book> books);
        Task<Book[]> LookUpBookAsync(string searchString);
        Task<BookDto[]> GetAllBookDtosAsync();
        Task<string[]> GetAllPublishersAsync();
        Task AddAsync(Book book);
        void RemoveBook(Book book);
    }
}