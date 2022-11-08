using System;
using Newtonsoft.Json;
using UnityEngine;

public struct TimeString {
    public float time;
    public byte type;

    public TimeString(string from) {
        char last = from[^1];
        string tm = from.Substring(0, from.Length - 1);
        switch (last) {
            case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9' or '.':
                time = float.Parse(tm, System.Globalization.CultureInfo.InvariantCulture);
                type = 0;
                break;
            case '%':
                time = float.Parse(tm, System.Globalization.CultureInfo.InvariantCulture);
                time /= 100f;
                type = 1;
                break;
            default:
                Debug.LogError($"Failed parsing time string '${from}'");
                type = 0;
                time = 0;
                break;
        }
    }

    public override string ToString() {
        string units = " %";
        return (time.ToString(System.Globalization.CultureInfo.InvariantCulture) + units[type]).Trim();
    }

    public static TimeString FromProgress(float p) {
        return new TimeString() { time = p, type = 1 };
    }

    public static TimeString FromTime(float p) {
        return new TimeString() { time = p, type = 0 };
    }
}

public class TimeStringConverter : JsonConverter<TimeString> {
    public override void WriteJson(JsonWriter writer, TimeString value, JsonSerializer serializer) {
        writer.WriteValue(value.ToString());
    }

    public override TimeString ReadJson(JsonReader reader, Type objectType, TimeString existingValue, bool hasExistingValue, JsonSerializer serializer) {
        string s = (string)reader.Value;
        return new TimeString(s);
    }
}