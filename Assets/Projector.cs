using System.Collections.Generic;
using Newtonsoft.Json;

public class Projector {
    // projector.js

    [JsonProperty("name")] public string Name { get; private set; }
    [JsonProperty("author")] public string Author { get; private set; }
    [JsonProperty("clips")] private Dictionary<string, VideoClip> m_clips;
}