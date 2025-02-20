using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using System.Collections.Generic;
using System.Linq;


namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private static List<Book> books = new List<Book>
        {
            new Book{Title = "The Great Gatsby", PublicationYear = "1925", AuthorName = "F. Scott Fitzgerald", ViewCount = "100"},
            new Book{Title = "Vefxistyaosani", PublicationYear = "1200", AuthorName = "Shota", ViewCount = "50"},
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            return Ok(books);
        }

        [HttpGet("{id}")]
        public ActionResult<Book> GetBook(string Title)
        {
            var book = books.FirstOrDefault(b => b.Title == Title);
            if (book == null)
            {
                return NotFound();
            }
            return Ok(book);
        }


        [HttpPost]
        public ActionResult<Book> AddBooks(Book book)
        {

        }


    }
}