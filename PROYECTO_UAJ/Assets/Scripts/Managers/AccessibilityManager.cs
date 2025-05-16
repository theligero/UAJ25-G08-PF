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
            if (player.GetComponent<DirectionalAudio>() == null)
            {
                var indicator = player.AddComponent<DirectionalAudio>();
            }
        }
    }

    public void SendEvent(AccessibilityEvent evt) {
        if (!accessibilityEnabled) return;

        NotifyContextEvent?.Invoke(evt); // Esto notifica a cualquier suscriptor
                                         // (Mirad el ejemplo de la flecha para hacer los demas igual)
    }
}
