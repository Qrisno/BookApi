using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookApi.Models
{
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string Id { get; set; }

        public string Title { get; set; }
        public string PublicationYear { get; set; }
        public string AuthorName { get; set; }
        public string ViewCount { get; set; } = "0";
        public bool IsDeleted { get; set; } = false;

        [BsonIgnore]
        public double PopularityScore { get; set; }
    }


    public class BookInput
    {

        public string Title { get; set; }
        public string PublicationYear { get; set; }
        public string AuthorName { get; set; }
    }


}