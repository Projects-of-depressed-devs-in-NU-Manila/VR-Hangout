using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Vector3Converter : JsonConverter {
    public override bool CanConvert(System.Type objectType) {
        return objectType == typeof(Vector3);
    }

    public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) {
        JObject obj = JObject.Load(reader);
        float x = (float)obj["x"];
        float y = (float)obj["y"];
        float z = (float)obj["z"];
        return new Vector3(x, y, z);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
        Vector3 v = (Vector3)value;
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteValue(v.x);
        writer.WritePropertyName("y");
        writer.WriteValue(v.y);
        writer.WritePropertyName("z");
        writer.WriteValue(v.z);
        writer.WriteEndObject();
    }
}
