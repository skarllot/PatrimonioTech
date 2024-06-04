using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PatrimonioTech.Domain.Ativos;

[JsonConverter(typeof(B3TickerJsonConverter))]
public sealed record B3Ticker(B3TickerName Name, B3TickerCode Code)
{
    public override string ToString() => $"{Name}{(int)Code}";

    private sealed class B3TickerJsonConverter : JsonConverter<B3Ticker>
    {
        public override B3Ticker? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            Deserialize(reader.GetString());

        public override void Write(Utf8JsonWriter writer, B3Ticker value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());

        private static B3Ticker? Deserialize(string? value)
        {
            if (value is null)
                return null;

            if (value.Length is < 5 or > 6)
                throw new JsonException("Invalid B3 ticker");

            var tickerName = B3TickerName.Create(value[..4]);
            if (!tickerName.TryGet(out var name, out _))
                throw new JsonException("Invalid B3 ticker name");

            var code = int.Parse(value.AsSpan(4), CultureInfo.InvariantCulture);
            return new B3Ticker(name, (B3TickerCode)code);
        }
    }
}
