using System;
using System.Collections.Generic;
using System.Text;
using Books.Core.Entities;

namespace Books.Core.DataTransferObjects
{
    public class AuthorDto
    {
        public int Id { get; set; }

        public string Author { get; set; }

        public int BookCount { get; set; }

        public IEnumerable<Book> Books { get; set; }

        public string Publishers { get; set; }
    }
}
