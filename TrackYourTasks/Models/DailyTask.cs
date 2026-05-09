using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TrackYourTasks.Models
{
    public class DailyTask
    {
        // API returns "id" (camelCase). PropertyNameCaseInsensitive=true in serializer will map it to this.
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // UI-only selection flag for bulk actions — ignored by BSON/DB
        [BsonIgnore]
        public bool IsSelected { get; set; }

        // API returns ISO datetime string or null
        public DateTime? RecurrenceTime { get; set; }

        // UI helper (not serialized)
        [BsonIgnore]
        public DateTime? NextOccurrence
        {
            get
            {
                if (!RecurrenceTime.HasValue) return null;
                var tod = RecurrenceTime.Value.TimeOfDay;
                var occurrence = DateTime.Today.Add(tod);
                return occurrence < DateTime.Now ? occurrence.AddDays(1) : occurrence;
            }
        }

        [BsonIgnore]
        public string CreatedAtLabel => CreatedAt == default ? string.Empty : CreatedAt.ToLocalTime().ToString("MMM d, yyyy h:mm tt");

        [BsonIgnore]
        public string RecurrenceLabel => RecurrenceTime.HasValue ? RecurrenceTime.Value.ToLocalTime().ToString("h:mm tt") : string.Empty;
    }
}