using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class VideoScene {
    [JsonProperty("clip")] public string Clip { get; private set; }
    [JsonProperty("events")] public SceneEvent[] Events { get; private set; } = new SceneEvent[0];
    [JsonProperty("transitions")]  private Dictionary<string, string> m_transitions = new Dictionary<string, string>();
    [JsonIgnore] public Dictionary<string, string> Transitions => m_transitions;

    public string GetTransitionForKey(KeyCode key) {
        if(Transitions.ContainsKey(key.ToString())) {
            return Transitions[key.ToString()];
        } 
        return "";
    }
}