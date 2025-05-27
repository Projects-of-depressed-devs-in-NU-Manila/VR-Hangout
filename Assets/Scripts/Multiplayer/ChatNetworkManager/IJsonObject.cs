using System;

public interface IJsonObject<T>
{
    string ConvertToJSON();
    static T FromJSON(string json) => throw new NotImplementedException();
}