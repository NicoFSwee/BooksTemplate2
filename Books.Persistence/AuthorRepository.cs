using Books.Core.Contracts;
using Books.Core.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Books.Core.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using System;

namespace Books.Persistence
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public AuthorRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<AuthorDto>> GetAuthorDtosAsync()
        {
            var authorDtos = await _dbContext.Authors
                                            .Select(a => new AuthorDto()
                                            {
                                                Id = a.Id,
                                                Author = a.Name,
                                                BookCount = a.BookAuthors.Count()
                                            })
                                            .OrderByDescending(a => a.BookCount)
                                            .ThenBy(a => a.Author)
                                            .ToListAsync();

            foreach (var dto in authorDtos)
            {
                dto.Books = await _dbContext.Authors.Include(a => a.BookAuthors)
                                                    .Where(a => a.Id == dto.Id)
                                                    .Select(a => a.BookAuthors)
                                                    .SelectMany(ba => ba)
                                                    .Select(ba => ba.Book)
                                                    .ToArrayAsync();

                string[] publishers = dto.Books.Select(b => b.Publishers)
                                                    .ToArray();

                dto.Publishers = string.Empty;
                for (int i = 0; i < publishers.Length; i++)
                {
                    int sumOfBooksByPublisher = dto.Books.Count(b => b.Publishers == publishers[i]);

                    if (dto.Publishers == string.Empty)
                    {
                        dto.Publishers += $"{publishers[i]} ({sumOfBooksByPublisher})";
                    }
                    else if (dto.Publishers != string.Empty && !dto.Publishers.Contains(publishers[i]))
                    {
                        dto.Publishers += $", {publishers[i]} ({sumOfBooksByPublisher})";
                    }
                    else if (dto.Publishers != string.Empty && dto.Publishers.Contains(publishers[i]))
                    {
                        sumOfBooksByPublisher++;
                    }
                }
            }

            return authorDtos;
        }

        public async Task<AuthorDto> GetAuthorDtoByIdAsync(int? id)
        {
            var authorDto = await _dbContext.Authors
                                .Where(a => a.Id == id)
                                .Select(a => new AuthorDto()
                                {
                                    Id = a.Id,
                                    Author = a.Name,
                                    BookCount = a.BookAuthors.Count()
                                })
                                .OrderByDescending(a => a.BookCount)
                                .ThenBy(a => a.Author)
                                .FirstOrDefaultAsync();



            authorDto.Books = await _dbContext.Authors.Include(a => a.BookAuthors)
                                                .Where(a => a.Id == authorDto.Id)
                                                .Select(a => a.BookAuthors)
                                                .SelectMany(ba => ba)
                                                .Select(ba => ba.Book)
                                                .ToArrayAsync();

            string[] publishers = authorDto.Books.Select(b => b.Publishers)
                                                .ToArray();

            authorDto.Publishers = string.Empty;
            for (int i = 0; i < publishers.Length; i++)
            {
                int sumOfBooksByPublisher = authorDto.Books.Count(b => b.Publishers == publishers[i]);

                if (authorDto.Publishers == string.Empty)
                {
                    authorDto.Publishers += $"{publishers[i]} ({sumOfBooksByPublisher})";
                }
                else if (authorDto.Publishers != string.Empty && !authorDto.Publishers.Contains(publishers[i]))
                {
                    authorDto.Publishers += $", {publishers[i]} ({sumOfBooksByPublisher})";
                }
                else if (authorDto.Publishers != string.Empty && authorDto.Publishers.Contains(publishers[i]))
                {
                    sumOfBooksByPublisher++;
                }
            }


            return authorDto;
        }

        public async Task AddAuthorAsync(Author newAuthor)
            => await _dbContext.Authors.AddAsync(newAuthor);
    }
}