using System.Text.Json;
using Confluent.Kafka;

namespace NewPortfolioMicroservice.Definitions.Kafka.Models;

public class PersonSerializer : ISerializer<EventPersonModel>
{
    public byte[] Serialize(EventPersonModel data, SerializationContext context) =>
        JsonSerializer.SerializeToUtf8Bytes(data);
}