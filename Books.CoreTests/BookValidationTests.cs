using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Books.Core.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Books.CoreTests
{
    [TestClass()]
    public class BookValidationTests
    {

        [TestMethod()]
        public void ValidateBook_AllCorrect_ShouldReturnNoValidationError()
        {
            var author = new Author { Name = "Author1" };
            var book = new Book { Title = "Title1", Publishers = "PublisherA", Isbn = "3760778739" };
            var bookAuthor = new BookAuthor { Author = author, Book = book };
            book.BookAuthors.Add(bookAuthor);
            var validationContext = new ValidationContext(book);
            var validationResults = new List<ValidationResult>();
            var ok = Validator.TryValidateObject(book, validationContext, validationResults, true);
            Assert.IsTrue(ok);
        }

        /// <summary>
        /// Test für die IValidatableObject
        /// </summary>
        [TestMethod()]
        public void ValidateBook_NoAuthor_ShouldReturnCorrectErrorMessage()
        {
            var author = new Author { Name = "Author1" };
            var book = new Book { Title = "Title1", Publishers = "PublisherA", Isbn = "3760778739" };
            var bookAuthor = new BookAuthor { Author = author, Book = book };
            //book.BookAuthors.Add(bookAuthor);
            var validationContext = new ValidationContext(book);
            var validationResults = new List<ValidationResult>();
            var ok = Validator.TryValidateObject(book, validationContext, validationResults, true);
            Assert.IsFalse(ok);
            Assert.AreEqual(1, validationResults.Count);
            Assert.AreEqual("Book must have at least one author", validationResults[0].ErrorMessage);
        }

        [TestMethod()]
        public void ValidateBook_AuthorTwiceInBook_ShouldReturnCorrectErrorMessage()
        {
            var author = new Author { Name = "Author1" };
            var book = new Book { Title = "Title1", Publishers = "PublisherA", Isbn = "3760778739" };
            var bookAuthor = new BookAuthor { Author = author, Book = book };
            book.BookAuthors.Add(bookAuthor);
            book.BookAuthors.Add(bookAuthor);
            var validationContext = new ValidationContext(book);
            var validationResults = new List<ValidationResult>();
            var ok = Validator.TryValidateObject(book, validationContext, validationResults, validateAllProperties: true);
            Assert.IsFalse(ok);
            Assert.AreEqual(1, validationResults.Count);
            Assert.AreEqual("Author1 are twice authors of the book", validationResults[0].ErrorMessage);
        }

    }
}