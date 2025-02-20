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
        };

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            return Ok(books);
        }

        [HttpGet("{title}")]
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
        public ActionResult<Book> AddBook(Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            var bookInList = books.Find(b => b.Title == book.Title);
            if (bookInList != null)
            {
                return BadRequest("Book Already Exists");
            }
            books.Add(book);
            return Ok(books);

        }


        [HttpPost("bulk")]
        public ActionResult<Book[]> AddBooks(Book[] booksForAdding)
        {
            if (books.Count == 0)
            {
                return BadRequest();
            }


            foreach (var book in booksForAdding)
            {
                books.Add(book);
            }
            return Ok(books);
        }

        [HttpPost("title")]
        public ActionResult<Book> updateBook(string title, Book updatedBook)
        {


            if (string.IsNullOrEmpty(title) || updatedBook == null)
            {
                return BadRequest();
            }

            var bookFound = books.Find(books => books.Title == title);
            if (bookFound != null)
            {
                books.Remove(bookFound);
                updatedBook.Title = title;
                books.Add(updatedBook);
                return Ok(books);
            }


            return NotFound();
        }

        [HttpDelete("{title}")]
        public ActionResult<Book> DeleteBook(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest();
            }
            var bookFound = books.Find(books => books.Title == title);
            if (bookFound != null)
            {
                books.Remove(bookFound);
                return Ok(books);
            }

            return NotFound();
        }

        [HttpDelete]
        public ActionResult<IEnumerable<Book>> DeleteBook([FromQuery] string[] titles)
        {
            if (titles == null || titles.Length == 0)
            {
                return BadRequest("No titles provided.");
            }



            foreach (var title in titles)
            {
                var bookFound = books.FirstOrDefault(b => b.Title == title);
                if (bookFound != null)
                {
                    books.Remove(bookFound);

                }

                return NotFound($"No books found with the provided titles: {title}");
            }

            return Ok(books);
        }

        [HttpGet("popular")]
        public ActionResult<Book> GetPopularBookTitles()
        {
            if (books.Count == 0)
            {
                return BadRequest("No Books Exists");
            }
            var orderedTitles = books.OrderBy(b => b.ViewCount).Select(b => b.Title).ToList();
            return Ok(orderedTitles);
        }
    }
}