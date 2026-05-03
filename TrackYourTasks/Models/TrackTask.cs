using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasks.Models
{
    public class TrackTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;  // 🔥 Let Mongo generate this

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
        public bool IsSkipped { get; set; } = false;
        public bool IsPartiallyCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}