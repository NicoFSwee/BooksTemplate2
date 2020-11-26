using System;
using System.Collections.Generic;
using System.Text;

namespace Books.Core.DataTransferObjects
{
    public class BookDto
    {
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Publishers { get; set; }
        public string Authors { get; set; }
    }
}
