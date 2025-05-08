using Newtonsoft.Json;
using UnityEngine;

public static class JsonHelper {
    private static JsonSerializerSettings _settings;

    public static JsonSerializerSettings Settings {
        get {
            if (_settings == null) {
                _settings = new JsonSerializerSettings();
                _settings.Converters.Add(new Vector3Converter());
            }
            return _settings;
        }
    }

    public static T FromJson<T>(string json) {
        return JsonConvert.DeserializeObject<T>(json, Settings);
    }

    public static string ToJson(object obj) {
        return JsonConvert.SerializeObject(obj, Settings);
    }
}
