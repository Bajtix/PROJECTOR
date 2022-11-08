using System;
using Newtonsoft.Json;
using UnityEngine;

public struct TrimString {
    public TimeString start;
    public TimeString end;

    public override string ToString() {
        return $"{start};{end}";
    }

    public TrimString(string from) {
        try {
            var frm = from.Split(';')[0];
            var to = from.Split(';')[1];

            start = new TimeString(frm);
            end = new TimeString(to);
        } catch {
            Debug.LogError($"Failed to parse trim string '{from}'");
            start = TimeString.FromProgress(0);
            end = TimeString.FromProgress(1);
        }
    }
}

public class TrimStringConverter : JsonConverter<TrimString> {
    public override void WriteJson(JsonWriter writer, TrimString value, JsonSerializer serializer) {
        writer.WriteValue(value.ToString());
    }

    public override TrimString ReadJson(JsonReader reader, Type objectType, TrimString existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string s = (string)reader.Value;
        return new TrimString(s);
    }
}