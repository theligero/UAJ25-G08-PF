using UnityEngine;

public enum EventType {
    InterestPoint
}

public class AccessibilityEvent {
    public EventType Type { get; private set; }
    public Transform Source { get; private set; }
    public string Description { get; private set; }

    public AccessibilityEvent(EventType type, Transform source, string description = "") {
        Type = type;
        Source = source;
        Description = description;
    }
}
