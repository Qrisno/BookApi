using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; }
        public string PublicationYear { get; set; }
        public string AuthorName { get; set; }
        public string ViewCount { get; set; }
    }
}