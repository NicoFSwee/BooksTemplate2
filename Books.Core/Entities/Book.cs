using Books.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Books.Core.Entities
{
    public class Book : EntityObject, IValidatableObject
    {

        public ICollection<BookAuthor> BookAuthors { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Titel muss definiert sein")]
        [MaxLength(200, ErrorMessage = "Titel darf maximal 200 Zeichen lang sein")]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string Publishers { get; set; }

        [IsbnValidation]
        [Required, MaxLength(13)]
        public string Isbn { get; set; }

        public Book()
        {
            BookAuthors = new List<BookAuthor>();
        }

        /// <summary>
        /// Eine gültige ISBN-Nummer besteht aus den Ziffern 0, ... , 9,
        /// 'x' oder 'X' (nur an der letzten Stelle)
        /// Die Gesamtlänge der ISBN beträgt 10 Zeichen.
        /// Für die Ermittlung der Prüfsumme werden die Ziffern 
        /// von rechts nach links mit 1 - 10 multipliziert und die 
        /// Produkte aufsummiert. Ist das rechte Zeichen ein x oder X
        /// wird als Zahlenwert 10 verwendet.
        /// Die Prüfsumme muss modulo 11 0 ergeben.
        /// </summary>
        /// <returns>Prüfergebnis</returns>
        public static bool CheckIsbn(string isbn)
        {
            int checkSum = 0;

            for (int i = 0; i < isbn.Length; i++)
            {
                if(!char.IsDigit(isbn[i]) && i != 9 || isbn.Length != 10)
                {
                    return false;
                }
                else if(!char.IsDigit(isbn[i]) && i == 9)
                {
                    if(isbn[i] != 'x' && isbn[i] != 'X')
                    {
                        return false;
                    }
                }

                if (char.IsDigit(isbn[i]))
                {
                    int number = int.Parse(isbn[i].ToString());
                    checkSum += number * (i + 1);
                }
                else if(isbn[i] == 'x' || isbn[i] == 'X')
                {
                    checkSum += 10 * (i + 1);
                }
            }

            if(checkSum % 11 == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public override string ToString()
        {
            return $"{Title} {Isbn} mit {BookAuthors.Count()} Autoren";
        }

        /// <inheritdoc />
        /// <summary>
        /// Jedes Buch muss zumindest einen Autor haben.
        /// Weiters darf ein Autor einem Buch nicht mehrfach zugewiesen
        /// werden.
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(this.BookAuthors.Count() == 0)
            {
                yield return new ValidationResult("Book must have at least one author");
            }

            Dictionary<string, Author> authors = new Dictionary<string, Author>();

            foreach (var bookAuthor in this.BookAuthors)
            {
                if(authors.ContainsKey(bookAuthor.Author.Name))
                {
                    yield return new ValidationResult($"{bookAuthor.Author.Name} are twice authors of the book");
                }
                else
                {
                    authors.Add(bookAuthor.Author.Name, bookAuthor.Author);
                }
            }

            if(!CheckIsbn(Isbn))
            {
                yield return new ValidationResult($"Isbn {Isbn} is invalid");
            }

            yield return ValidationResult.Success;
        }
    }
}

