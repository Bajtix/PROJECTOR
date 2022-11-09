using Newtonsoft.Json;

public class VideoClip {
    [JsonProperty("path")] public string Path { get; private set; }
    [JsonProperty("volume")]  public float Volume { get; private set; } = 1f;
    [JsonProperty("trim")]  public TrimString Trim { get; private set; } = TrimString.Full;
}
