using Microsoft.AspNetCore.Mvc;
using BookApi.Models;
using System.Collections.Generic;
using System.Linq;
using BookApi.Data;
using BookApi.Services;
namespace BookApi.Controllers
{
    [Route("BooksApi")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookRepository _bookRepository;
        private readonly BookService _bookService;

        public BooksController(BookRepository bookRepository, BookService bookService)
        {
            _bookRepository = bookRepository;
            _bookService = bookService;
        }

        [HttpGet("GetAllBooks")]
        public ActionResult<IEnumerable<Book>> GetAllBooks()
        {
            var books = _bookRepository.Get();
            return Ok(books);
        }

        [HttpGet("GetPopularBooks")]
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

        [HttpGet("GetBook/{title}")]
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
            var popularityScore = _bookService.CalculatePopularityScore(book);
            book.PopularityScore = popularityScore;

            return Ok(book);
        }

        [HttpPost("AddBook")]
        public ActionResult<Book> AddBook(BookInput book)
        {
            if (book == null)
            {
                return BadRequest();
            }



            var bookInList = _bookRepository.Get().Find(b => b.Title == book.Title);
            if (bookInList != null)
            {
                return BadRequest("Book Already Exists");
            }
            var newBook = new Book { Title = book.Title, PublicationYear = book.PublicationYear, AuthorName = book.AuthorName };
            _bookRepository.Create(newBook);
            return Ok(book);
        }

        [HttpPost("AddBooks")]
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

        [HttpPost("UpdateBook")]
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

        [HttpDelete("DeleteBook/{title}")]
        public ActionResult<Book> DeleteBook(string title)
        {
            if (string.IsNullOrEmpty(title))
            {
                return BadRequest();
            }
            var bookFound = _bookRepository.Get().FirstOrDefault(b => b.Title == title);
            if (bookFound != null)
            {
                bookFound.IsDeleted = true;
                _bookRepository.Update(bookFound.Id, bookFound);
                return Ok(bookFound);
            }

            return NotFound();
        }

        [HttpDelete("DeleteBooks")]
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
                    bookFound.IsDeleted = true;
                    _bookRepository.Update(bookFound.Id, bookFound);
                }
            }

            if (booksRemoved.Count == 0)
            {
                return NotFound("No books found with the provided titles.");
            }

            return Ok(booksRemoved);
        }



    }
}