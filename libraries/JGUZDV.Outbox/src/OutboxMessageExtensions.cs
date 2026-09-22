using System.Text.Json;

namespace JGUZDV.Outbox;

/// <summary>
/// Provides extension methods for the OutboxMessage class, allowing for convenient creation and manipulation of outbox messages with JSON serialization and deserialization support.
/// </summary>
public static class OutboxMessageExtensions
{
    extension(OutboxMessage message)
    {
        /// <summary>
        /// Creates a new instance of the OutboxMessage class with the specified due date, message type, and message data, serialized as JSON.
        /// </summary>
        public static OutboxMessage Create<TMessageData>(DateTimeOffset dueDate, string messageType, TMessageData messageData)
        {
            var jsonMessageData = JsonSerializer.Serialize(messageData);
            return new OutboxMessage(dueDate, messageType, jsonMessageData);
        }

        /// <summary>
        /// Sets the tags for the outbox message and returns the message instance for method chaining.
        /// </summary>
        public OutboxMessage WithTags(string tags)
        {
            message.Tags = tags;
            return message;
        }

        /// <summary>
        /// Stores the tags for the outbox message as a JSON string and returns the message instance for method chaining.
        /// </summary>
        public OutboxMessage WithTags<TTags>(TTags tags)
        {
            var jsonTags = JsonSerializer.Serialize(tags);
            message.Tags = jsonTags;

            return message;
        }


        /// <summary>
        /// Gets the message data deserialized from JSON to the specified type.
        /// </summary>
        /// <returns>True if the message data was successfully deserialized; otherwise, false.</returns>
        public bool TryGetJsonMessageData<T>(out T messageData)
        {
            try
            {
                messageData = JsonSerializer.Deserialize<T>(message.MessageData)!;
                return true;
            }
            catch
            {
                messageData = default!;
                return false;
            }
        }

        /// <summary>
        /// Gets the message tags deserialized from JSON to the specified type.
        /// </summary>
        /// <returns>True if the message tags were successfully deserialized; otherwise, false.</returns>
        public bool TryGetJsonTags<T>(out T tags)
        {
            try
            {
                if (message.Tags is null)
                {
                    tags = default!;
                    return false;
                }
                tags = JsonSerializer.Deserialize<T>(message.Tags)!;
                return true;
            }
            catch
            {
                tags = default!;
                return false;
            }
        }
    }
}