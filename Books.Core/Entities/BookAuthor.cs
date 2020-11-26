using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Core.Entities
{
    public class BookAuthor : EntityObject
    {
        public Author Author { get; set; }
        public int AuthorId { get; set; }

        public Book Book { get; set; }
        public int BookId { get; set; }
    }
}
