using UnityEngine;

public enum EventType {
    Enable,
    Disable,
    InterestPoint,
    DirectionalAudio, // Audio direccional
    Alert // Alertas
}

public enum AccessibilityTarget {
    ArrowIndicator,
    DirectionalAudioGuide,
    AlertGuide,
    PathVisualizer,


    ALL // Broadcast
}


public class AccessibilityEvent {
    public EventType Type { get; private set; }
    public AccessibilityTarget Target { get; private set; }
    public Transform Source { get; private set; }
    public string Description { get; private set; }
    public AudioClip Clip { get; private set; }

    public AccessibilityEvent(EventType type, Transform source, AccessibilityTarget target = AccessibilityTarget.ALL, string description = "", AudioClip clip = null) {
        Type = type;
        Target = target;
        Source = source;
        Description = description;
        Clip = clip;
    }
}
