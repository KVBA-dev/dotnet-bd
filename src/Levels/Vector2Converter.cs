using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Game.Levels;

public class Vector2Converter : JsonConverter<Vector2> {
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        float x = 0, y = 0;
        string propertyName;
        while (reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) {
                break;
            }
            if (reader.TokenType == JsonTokenType.PropertyName) {
                propertyName = reader.GetString();
                reader.Read();

                switch (propertyName) {
                    case "X":
                        x = reader.GetSingle();
                        break;
                    case "Y":
                        y = reader.GetSingle();
                        break;
                }
            }
        }
        return new(x, y);
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("X", value.X);
        writer.WriteNumber("Y", value.Y);
        writer.WriteEndObject();
    }
}