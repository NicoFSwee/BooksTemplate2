using Books.Core.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Books.Web.ApiControllers
{
    /// <summary>
    /// API to managed the Books application.
    /// </summary>
    [Produces("application/json")]
    [Route("api/books")]
    public class BooksController : Controller
    {
        private readonly IUnitOfWork _uow;

        public BooksController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        /// <summary>
        /// Bind books by title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        // GET: api/books/<title>
        [Route("{title}")]
        [HttpGet]
        public async Task<IActionResult> GetBooks(string title)
        {
            var books = await _uow.Books.LookUpBookAsync(title);

            return Ok(books);
        }
    }
}
