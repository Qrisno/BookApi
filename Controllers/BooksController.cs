using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using System.Collections.Generic;
using System.Linq;
using BookApi.Data;

namespace BookApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookRepository _bookRepository;

        public BooksController(BookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            var books = _bookRepository.Get();
            return Ok(books);
        }

        [HttpGet("{title}")]
        public ActionResult<Book> GetBook(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest("Title cannot be null or empty.");
            }
            var book = _bookRepository.Get().FirstOrDefault(b => b.Title.Trim().Equals(title.Trim(), StringComparison.OrdinalIgnoreCase));
            if (book == null)
            {
                return NotFound();
            }
            book.ViewCount = (int.Parse(book.ViewCount) + 1).ToString();
            _bookRepository.Update(book.Id, book);
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

            var bookInList = _bookRepository.Get().Find(b => b.Title == book.Title);
            if (bookInList != null)
            {
                return BadRequest("Book Already Exists");
            }
            _bookRepository.Create(book);
            return Ok(book);
        }

        [HttpPost("bulk")]
        public ActionResult<IEnumerable<Book>> AddBooks(Book[] booksForAdding)
        {
            if (booksForAdding == null || booksForAdding.Length == 0)
            {
                return BadRequest();
            }

            foreach (var book in booksForAdding)
            {
                _bookRepository.Create(book);
            }
            return Ok(booksForAdding);
        }

        [HttpPost("title")]
        public ActionResult<Book> UpdateBook(string title, Book updatedBook)
        {
            if (string.IsNullOrEmpty(title) || updatedBook == null)
            {
                return BadRequest();
            }

            var bookFound = _bookRepository.Get().FirstOrDefault(b => b.Title == title);
            if (bookFound != null)
            {
                updatedBook.Id = bookFound.Id;
                _bookRepository.Update(bookFound.Id, updatedBook);
                return Ok(updatedBook);
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
            var bookFound = _bookRepository.Get().FirstOrDefault(b => b.Title == title);
            if (bookFound != null)
            {
                _bookRepository.Remove(bookFound.Id);
                return Ok(bookFound);
            }

            return NotFound();
        }

        [HttpDelete]
        public ActionResult<IEnumerable<Book>> DeleteBooks([FromQuery] string[] titles)
        {
            if (titles == null || titles.Length == 0)
            {
                return BadRequest("No titles provided.");
            }

            var booksRemoved = new List<Book>();

            foreach (var title in titles)
            {
                var bookFound = _bookRepository.Get().FirstOrDefault(b => b.Title == title);
                if (bookFound != null)
                {
                    _bookRepository.Remove(bookFound.Id);
                    booksRemoved.Add(bookFound);
                }
            }

            if (booksRemoved.Count == 0)
            {
                return NotFound("No books found with the provided titles.");
            }

            return Ok(booksRemoved);
        }

        [HttpGet("popular")]
        public ActionResult<IEnumerable<string>> GetPopularBookTitles()
        {
            var books = _bookRepository.Get();
            if (books.Count == 0)
            {
                return BadRequest("No Books Exist");
            }
            var orderedTitles = books.OrderByDescending(b => int.Parse(b.ViewCount)).Select(b => b.Title).ToList();
            return Ok(orderedTitles);
        }
    }
}