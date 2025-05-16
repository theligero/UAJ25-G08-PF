using UnityEngine;

public enum EventType {
    Enable,
    Disable,
    InterestPoint
}

public enum AccessibilityTarget{
    ArrowIndicator,


    ALL // Broadcast
}


public class AccessibilityEvent {
    public EventType Type { get; private set; }
    public AccessibilityTarget Target { get; private set; }
    public Transform Source { get; private set; }
    public string Description { get; private set; }


    public AccessibilityEvent(EventType type, Transform source, AccessibilityTarget target = AccessibilityTarget.ALL, string description = "") {
        Type = type;
        Target = target;
        Source = source;
        Description = description;
    }
}
