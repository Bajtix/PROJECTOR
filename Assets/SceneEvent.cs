public class SceneEvent {
    public TimeString Time { get; private set; }
    public string Function { get; private set; }
    public bool Marker { get; private set; } = true;
}