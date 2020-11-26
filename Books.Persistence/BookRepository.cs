using Books.Core.Contracts;
using Books.Core.DataTransferObjects;
using Books.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Persistence
{
    public class BookRepository : IBookRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public BookRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(Book book)
            => await _dbContext.Books.AddAsync(book);

        public async Task<string[]> GetAllPublishersAsync()
            => await _dbContext.Books.Select(b => b.Publishers)
                                     .Distinct()
                                     .ToArrayAsync();

        public async Task AddRangeAsync(IEnumerable<Book> books)
            => await _dbContext.AddRangeAsync(books);

        public async Task<Book[]> LookUpBookAsync(string searchString)
            => await _dbContext.Books.Where(b => b.Title.Contains(searchString))
                                .ToArrayAsync();

        public async Task<BookDto[]> GetAllBookDtosAsync()
        {
            var dtos = await _dbContext.Books.OrderBy(b => b.Title)
                                        .Select(b => new BookDto
                                        {
                                            Isbn = b.Isbn,
                                            Title = b.Title,
                                            Publishers = b.Publishers
                                        })
                                        .ToArrayAsync();

            foreach (var dto in dtos)
            {
                string[] authors = await _dbContext.Books.Where(b => b.Isbn == dto.Isbn)
                                                        .SelectMany(b => b.BookAuthors)
                                                        .Select(ba => ba.Author.Name)
                                                        .ToArrayAsync();

                dto.Authors = string.Empty;

                foreach (var author in authors)
                {
                    if (dto.Authors == string.Empty)
                    {
                        dto.Authors += $"{author}";
                    }
                    else
                    {
                        dto.Authors += $"/ {author}";
                    }
                }
            }

            return dtos;
        }
    }

}