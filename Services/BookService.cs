using BookApi.Models;

namespace BookApi.Services
{
    public class BookService
    {

        public int CalculatePopularityScore(Book book)
        {

            int viewCount = int.Parse(book.ViewCount);
            int publicationYear = int.Parse(book.PublicationYear);
            int currentYear = DateTime.Now.Year;
            return viewCount * 2 + (currentYear - publicationYear);
        }

    }
}