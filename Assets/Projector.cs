using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Projector {
    // projector.json

    [JsonProperty("name")] public string Name { get; private set; }
    [JsonProperty("author")] public string Author { get; private set; }
    [JsonProperty("size")] public string Size { get; private set; }
    [JsonProperty("clips")] private Dictionary<string, VideoClip> m_clips;
    [JsonProperty("scenes")] private Dictionary<string, VideoScene> m_scenes;
    [JsonProperty("mouse")] public MouseMode MouseMode { get; private set; } = MouseMode.Mouse;
    [JsonProperty("shortcuts")] private Dictionary<string, string> m_shortcuts = new Dictionary<string, string>();
    [JsonIgnore] public Dictionary<string, string> Shortcuts => m_shortcuts;

    [JsonIgnore] public int ClipCount => m_clips.Count;
    [JsonIgnore] public int SceneCount => m_scenes.Count;

    public VideoClip GetClip(string name) {
        if (!m_clips.ContainsKey(name)) {
            Debug.LogError($"Cannot find clip {name}");
            return null;
        }
        return m_clips[name];
    }

    public VideoScene GetScene(string name) {
        if (!m_scenes.ContainsKey(name)) {
            Debug.LogError($"Cannot find scene {name}");
            return null;
        }
        return m_scenes[name];
    }

    public string GetTransitionForKey(KeyCode key) {
        if (Shortcuts.ContainsKey(key.ToString())) {
            return Shortcuts[key.ToString()];
        }
        return "";
    }
}