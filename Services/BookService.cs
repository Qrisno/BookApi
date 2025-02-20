using BookApi.Models;
using MongoDB.Driver;
using System.Collections.Generic;


namespace BookApi.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBooksDBSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _books = database.GetCollection<Book>(settings.BooksCollectionName);

        }

        public List<Book> Get() =>
            _books.Find(book => true).ToList();

        public Book Get(string id) =>
            _books.Find<Book>(book => book.Id == id).FirstOrDefault();

        public Book Create(Book book)
        {
            _books.InsertOne(book);
            return book;
        }

        public void Update(string id, Book bookIn) =>
            _books.ReplaceOne(book => book.Id == id, bookIn);

        public void Remove(string id) =>
            _books.DeleteOne(book => book.Id == id);


        private void AddInitialDataToDB()
        {
            var books = new List<Book>
        {
            new Book{Id = "1", Title = "The Great Gatsby", PublicationYear = "1925", AuthorName = "F. Scott Fitzgerald", ViewCount = "100"},
            new Book{Id = "2", Title = "Vefxistyaosani", PublicationYear = "1200", AuthorName = "Shota", ViewCount = "50"},
        };
            foreach (var book in books)

            {
                var bookInDB = _books.Find(b => b.Id == book.Id);
                if (bookInDB == null)
                {
                    _books.InsertOne(book);
                }

            }

        }
    }
}