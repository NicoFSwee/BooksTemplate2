using Books.Core.Contracts;
using Books.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Books.Web.Pages.Authors
{
    public class CreateModel : PageModel
    {
        private readonly IUnitOfWork _uow;

        [BindProperty]
        [Required, MinLength(2, ErrorMessage = "Name muss mindestens 2 Zeichen lang sein"), MaxLength(100)]
        public string Author { get; set; }

        public CreateModel(IUnitOfWork uow)
        {
            _uow = uow;
        }

        // POST: Authors/Create
        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Author newAuthor = new Author()
            {
                Name = Author,
                BookAuthors = new List<BookAuthor>()
            };

            await _uow.Authors.AddAuthorAsync(newAuthor);

            try
            {
                await _uow.SaveChangesAsync();
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }

            return RedirectToPage("../Index");
        }
    }
}
