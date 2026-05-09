using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasks.Models
{
    public class DailyTask
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UI-only selection flag for bulk actions — ignored by BSON/DB
        [BsonIgnore]
        public bool IsSelected { get; set; }

        // Persist recurrence time so the client will send/receive it from the API.
        // Store TimeSpan as string in MongoDB JSON (e.g. "01:30:00").
        [BsonRepresentation(BsonType.String)]
        public TimeSpan? RecurrenceTime { get; set; }

        // Calculates the next occurrence of this task based on the stored time
        [BsonIgnore]
        public DateTime? NextOccurrence => RecurrenceTime.HasValue
            ? DateTime.Today.Add(RecurrenceTime.Value)
            : (DateTime?)null;
    }
}