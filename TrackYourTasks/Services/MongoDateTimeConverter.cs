using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TrackYourTasks.Services
{
    // Handles both plain ISO date strings and MongoDB extended JSON { "$date": "..."} (string or epoch ms)
    public class MongoDateTimeConverter : System.Text.Json.Serialization.JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            if (reader.TokenType == JsonTokenType.String)
            {
                var s = reader.GetString();
                if (string.IsNullOrEmpty(s)) return null;
                if (DateTime.TryParse(s, out var dt)) return DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToLocalTime();
                return null;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                if (doc.RootElement.TryGetProperty("$date", out var dateElement))
                {
                    if (dateElement.ValueKind == JsonValueKind.String)
                    {
                        var s = dateElement.GetString();
                        if (DateTime.TryParse(s, out var dt)) return DateTime.SpecifyKind(dt, DateTimeKind.Utc).ToLocalTime();
                    }
                    else if (dateElement.ValueKind == JsonValueKind.Number)
                    {
                        // epoch milliseconds
                        if (dateElement.TryGetInt64(out var ms))
                            return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime.ToLocalTime();
                    }
                }
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                // treat number as epoch ms
                if (reader.TryGetInt64(out var ms))
                    return DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime.ToLocalTime();
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
                writer.WriteStringValue(value.Value.ToUniversalTime().ToString("o"));
            else
                writer.WriteNullValue();
        }
    }
}