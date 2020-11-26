using Books.Core;
using Books.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Books.Core.Entities;

namespace Books.ImportConsole
{
    class Program
    {
        static async Task Main()
        {
            await InitDataAsync();

            Console.WriteLine();
            Console.Write("Beenden mit Eingabetaste ...");
            Console.ReadLine();
        }

        private static async Task InitDataAsync()
        {
            Console.WriteLine("Import der Bücher in die Datenbank");
            await using var unitOfWork = new UnitOfWork();
            Console.WriteLine("Datenbank löschen");
            await unitOfWork.DeleteDatabaseAsync();
            Console.WriteLine("Datenbank migrieren");
            await unitOfWork.MigrateDatabaseAsync();
            Console.WriteLine("Bücher werden von books.csv eingelesen");
            var books = ImportController.ReadBooksFromCsv().ToList();
            if (books.Count == 0)
            {
                Console.WriteLine("!!! Es wurden keine Bücher eingelesen");
                return;
            }
            Console.WriteLine($"  Es wurden {books.Count} Bücher eingelesen!");

            var validBooks = new List<Book>();
            books.ForEach(book =>
            {
                var validationResults = new List<ValidationResult>();
                if (Validator.TryValidateObject(book, new ValidationContext(book), validationResults, true))
                {
                    validBooks.Add(book);
                }
                else
                {
                    validationResults.ForEach(Console.WriteLine);
                }
            });
            Console.WriteLine($"  Nur {validBooks.Count} Bücher sind valide!");
            var authors = validBooks.SelectMany(b => b.BookAuthors).Select(ba => ba.Author).Distinct();
            Console.WriteLine($"  Es wurden {authors.Count()} Autoren eingelesen!");

            Console.WriteLine("Bücher werden in Datenbank gespeichert (in Context übertragen)");
            await unitOfWork.Books.AddRangeAsync(validBooks);
            Console.WriteLine("Bücher werden in Datenbank gespeichert (persistiert)");
            await unitOfWork.SaveChangesAsync();
        }
    }
}
