using Books.Core.Entities;
using System;
using System.Linq;
using System.Collections.Generic;
using Utils;

namespace Books.ImportConsole
{
    public static class ImportController
    {
        const string FILENAME = "books.csv";
        const int AUTHOR_IDX = 0;
        const int BOOKTITLE_IDX = 1;
        const int PUBLISHER_IDX = 2;
        const int ISBN_IDX = 3;
        const string SEPARATOR = "~";
        public static IEnumerable<Book> ReadBooksFromCsv()
        {
            string[][] data = MyFile.ReadStringMatrixFromCsv(FILENAME, false);

            var books = data.Select(d => new Book()
            {
                BookAuthors = new List<BookAuthor>(),
                Title = d[BOOKTITLE_IDX],
                Publishers = d[PUBLISHER_IDX],
                Isbn = d[ISBN_IDX]
            }).ToList();

            var authors = data.Select(d => d[AUTHOR_IDX])
                .ToArray();

            Dictionary<string, Author> authorDictionary = new Dictionary<string, Author>();

            for (int i = 0; i < books.Count(); i++)
            {
                if(authors[i].Contains(SEPARATOR))
                {
                    var authorArray = authors[i].Split(SEPARATOR);

                    foreach (var author in authorArray)
                    {
                        if (!authorDictionary.ContainsKey(author))
                        {
                            Author tmp = new Author()
                            {
                                BookAuthors = new List<BookAuthor>(),
                                Name = author
                            };
                            books[i].BookAuthors.Add(new BookAuthor
                            {
                                Author = tmp,
                                Book = books[i]
                            });
                            authorDictionary.Add(author, tmp);
                        }
                        else
                        {
                            books[i].BookAuthors.Add(new BookAuthor
                            {
                                Author = authorDictionary[author],
                                Book = books[i]
                            });
                        }
                    }
                }
                else
                {
                    if (!authorDictionary.ContainsKey(authors[i]))
                    {
                        Author tmp = new Author()
                        {
                            BookAuthors = new List<BookAuthor>(),
                            Name = authors[i]
                        };
                        books[i].BookAuthors.Add(new BookAuthor
                        {
                            Author = tmp,
                            Book = books[i]
                        });
                        authorDictionary.Add(authors[i], tmp);
                    }
                    else
                    {
                        books[i].BookAuthors.Add(new BookAuthor
                        {
                            Author = authorDictionary[authors[i]],
                            Book = books[i]
                        });
                    }
                }
            }
            return new List<Book>(books);
        }
    }
}
