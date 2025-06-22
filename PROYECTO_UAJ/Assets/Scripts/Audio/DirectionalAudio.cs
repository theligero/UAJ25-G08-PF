using UnityEngine;

public class DirectionalAudio : MonoBehaviour {
    [Header("Track")]
    [SerializeField] private AudioClip clip;

    [Header("Audio Settings")]
    [SerializeField] private float maxAudibleAngle = 90f; // Con 0º significa que el player apunta al objetivo. Con maxAudibleAngle el sonido queda a minVolume
    [SerializeField] private float pitchRange = 0.2f; // Tono
    [SerializeField] private float minVolume = 0.1f; // Volumen mínimo

    private AudioSource src; // Fuente del audio
    private Transform currentTarget; // Transform del objetivo al que apuntamos

    void Awake() {
        src = gameObject.AddComponent<AudioSource>();
        src.spatialBlend = 1f; // Seteamos el audio a 3D
        src.loop = true; // Loop del audio
    }

    void Update() {
        if (currentTarget != null) {
            Vector3 toTarget = (currentTarget.position - transform.position).normalized; // Normalizamos el vector player-objetivo
            float angle = Vector3.Angle(transform.forward, toTarget); // Ángulo entre la dirección y el vector objetivo
            float vol = Mathf.InverseLerp(maxAudibleAngle, 0f, angle); // Si angle = maxAudibleAngle -> volumen al mínimo. Si angle = 0 -> volumen máximo

            src.volume = Mathf.Lerp(minVolume, 1f, vol); // El volumen depende de vol
            src.pitch = 1f + pitchRange * (vol - 0.5f); // El pitch también depende de vol
        }
        else { 
            if (src.isPlaying) src.Stop(); // Paramos si no hay objetivo
            return; 
        }
    }

    void OnEnable() {   
        AccessibilityManager.Instance.NotifyContextEvent += HandleEvent;
    }

    void OnDisable() {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.NotifyContextEvent -= HandleEvent;
    }

    void HandleDirectionalAudio(AccessibilityEvent evt) {
        currentTarget = evt.Source; // Guardamos el tranform objetivo
        if (clip) src.clip = clip; // Si en el evento clip != null, cargamos el clip
        if (!src.isPlaying && src.clip) src.Play();
    }

    void HandleEvent(AccessibilityEvent evt) {
        if (evt.Target != AccessibilityTarget.DirectionalAudio && evt.Target != AccessibilityTarget.ALL)
            return;

        switch (evt.Type) {
            case EventType.InterestPoint:
                HandleDirectionalAudio(evt); // Gestionamos el audio
                break;
            case EventType.Enable:
                src.Play();
                break;
            case EventType.Disable:
                src.Stop();
                break;
        }
    }
}
