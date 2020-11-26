using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Books.Core.Entities
{
    public class Author : EntityObject
    {
        [Required, MinLength(2, ErrorMessage = "Name muss mindestens 2 Zeichen lang sein"), MaxLength(100)]
        public string Name { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }

        public Author()
        {
            BookAuthors = new List<BookAuthor>();
        }

        public override string ToString()
        {
            return $"{Name} hat {BookAuthors.Count} Bücher";
        }
    }
}
