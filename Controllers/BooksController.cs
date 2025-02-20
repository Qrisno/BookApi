using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using System.Collections.Generic;
using System.Linq;
using BookApi.Services;


namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;
        public BooksController(BookService bookService)
        {
            this._bookService = bookService;

        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            var books = _bookService.Get();
            return Ok(books);
        }

        [HttpGet("{title}")]
        public ActionResult<Book> GetBook(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title cannot be null or empty.");
            }
            var book = this._bookService.Get().FirstOrDefault(b => b.Title.Trim().Equals(title.Trim(), StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                return NotFound();
            }
            _bookService.Remove(book.Id);
            book.ViewCount = (int.Parse(book.ViewCount) + 1).ToString();
            _bookService.Create(book);
            return Ok(book);
        }


        [HttpPost]
        public ActionResult<Book> AddBook(Book book)
        {
            if (book == null)
            {
                return BadRequest();
            }

            book.ViewCount = "0";

            var bookInList = _bookService.Get().Find(b => b.Title == book.Title);
            if (bookInList != null)
            {
                return BadRequest("Book Already Exists");
            }
            _bookService.Create(book);
            return Ok(_bookService.Get());

        }


        [HttpPost("bulk")]
        public ActionResult<Book[]> AddBooks(Book[] booksForAdding)
        {
            if (_bookService.Get().Count == 0)
            {
                return BadRequest();
            }


            foreach (var book in booksForAdding)
            {
                _bookService.Create(book);
            }
            return Ok(_bookService.Get());
        }

        [HttpPost("title")]
        public ActionResult<Book> updateBook(string title, Book updatedBook)
        {


            if (string.IsNullOrEmpty(title) || updatedBook == null)
            {
                return BadRequest();
            }

            var bookFound = _bookService.Get().Find(book => book.Title == title);
            if (bookFound != null)
            {
                _bookService.Remove(bookFound.Id);
                updatedBook.Title = title;
                _bookService.Create(updatedBook);
                return Ok(_bookService.Get());
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
            var bookFound = _bookService.Get().Find(books => books.Title == title);
            if (bookFound != null)
            {
                _bookService.Remove(bookFound.Id);
                return Ok(_bookService.Get());
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
                var bookFound = _bookService.Get().FirstOrDefault(b => b.Title == title);
                if (bookFound != null)
                {
                    _bookService.Remove(bookFound.Id);

                }

                return NotFound($"No books found with the provided titles: {title}");
            }

            return Ok(_bookService.Get());
        }

        [HttpGet("popular")]
        public ActionResult<Book> GetPopularBookTitles()
        {
            if (_bookService.Get().Count == 0)
            {
                return BadRequest("No Books Exists");
            }
            var orderedTitles = _bookService.Get().OrderBy(b => b.ViewCount).Select(b => b.Title).ToList();
            return Ok(orderedTitles);
        }
    }
}