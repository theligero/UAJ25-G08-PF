using UnityEngine;
using System;

public class AccessibilityManager : MonoBehaviour {
    public static AccessibilityManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject player;


    public bool accessibilityEnabled = true;

    public event Action<AccessibilityEvent> NotifyContextEvent;

    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (player != null) { // Comprobamos si ya tiene el componente
            if (player.GetComponent<ArrowIndicator>() == null) {
                var indicator = player.AddComponent<ArrowIndicator>();
            }
            if (player.GetComponent<DirectionalAudio>() == null) {
                var indicator = player.AddComponent<DirectionalAudio>();
            }
            if (player.GetComponent<AlertFeedback>() == null) {
                var indicator = player.AddComponent<AlertFeedback>();
            }
            if (player.GetComponent<PathVisualizer>() == null) {
                var indicator = player.AddComponent<PathVisualizer>();
            }
            if (player.GetComponent<LineRenderer>() == null) {
                var indicator = player.AddComponent<LineRenderer>();
            }
            if (player.GetComponent<AutoRotateOnIdle>() == null) {
                var indicator = player.AddComponent<AutoRotateOnIdle>();
            }
        }
    }

    public void SendEvent(AccessibilityEvent evt) {
        if (!accessibilityEnabled) return;

        NotifyContextEvent?.Invoke(evt); 
    }
}
