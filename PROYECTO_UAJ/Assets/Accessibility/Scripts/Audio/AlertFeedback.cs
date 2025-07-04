using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlertFeedback : MonoBehaviour {
    [Header("Track")]
    [SerializeField] private AudioClip alertClip;

    [Header("Alert Settings")]
    [SerializeField] 
    private float baseVolume = 1f; // Volumen del sonido
    [SerializeField] 
    private float vibrationDuration = 0.25f; // Duraci�n de vibraci�n
    [SerializeField]
    private float distToAlert = 7f; // Distancia a la que se genera la alerta
    [SerializeField]
    private float minIntensity = 0.1f; // Intensidad al l�mite de distToAlert
    [SerializeField]
    private float maxIntensity = 1.0f; // Intensidad al lado del objetivo
    [SerializeField]
    private float minInterval = 0.3f; // Intervalo de la vibraci�n al lado del objetivo
    [SerializeField]
    private float maxInterval = 2.5f; // // Intervalo de la vibraci�n al l�mite de distToAlert

    private AudioSource src;
    private bool alertsEnabled; // Flag
    private Transform currentTarget; // Transform del objetivo al que apuntamos
    private float nextAlertTime = 0f; // Contador
 

    void Awake() {
        src = gameObject.AddComponent<AudioSource>();
        src.spatialBlend = 0f; // Audio 2D
        alertsEnabled = true;
    }

    void Update() {
        if (alertsEnabled && currentTarget != null) {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            if (dist <= distToAlert && Time.time >= nextAlertTime) {
                float t = Mathf.InverseLerp(0f, distToAlert, dist); // 0 si est� encima del objeto y 1 si est� en distToAlert

                float intensity = Mathf.Lerp(maxIntensity, minIntensity, t);
                float interval = Mathf.Lerp(minInterval, maxInterval, t);

                UpdateContextAlert(intensity);

                nextAlertTime = Time.time + interval;
            }
        }
    }

    void OnEnable() {
        AccessibilityManager.Instance.NotifyContextEvent += HandleEvent;
    }

    void OnDisable() {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.NotifyContextEvent -= HandleEvent;
    }

    void HandleContextAlert(AccessibilityEvent evt) {
        currentTarget = evt.Source;
        if (alertClip) src.clip = alertClip;                                                                        
    }

    void UpdateContextAlert(float intensity) {
        src.PlayOneShot(alertClip, baseVolume * intensity); // Lanzamos alerta
        if (Gamepad.current != null) StartCoroutine(Vibrate(intensity)); // Y vibraci�n       
    }

    IEnumerator Vibrate(float power) {
        power = Mathf.Clamp01(power);
        Gamepad.current.SetMotorSpeeds(power, power);
        yield return new WaitForSeconds(vibrationDuration);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    void HandleEvent(AccessibilityEvent evt) {
        if (evt.Target != AccessibilityTarget.Alert && evt.Target != AccessibilityTarget.ALL)
            return;

        switch (evt.Type) {
            case EventType.InterestPoint:
                HandleContextAlert(evt); // Gestionamos la alerta
                break;
            case EventType.Enable:
                alertsEnabled = true;
                break;
            case EventType.Disable:
                alertsEnabled = false;                 
                break;
        }
    }
}
