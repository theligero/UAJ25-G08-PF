using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class AlertFeedback : MonoBehaviour {
    [Header("Alert Settings")]
    [SerializeField] 
    private float baseVolume = 1f; // Volumen del sonido
    [SerializeField] 
    private float vibrationDuration = 0.25f; // Duración de vibración
    [SerializeField]
    private float distToAlert = 7f; // Distancia a la que se genera la alerta
    [SerializeField]
    private float minIntensity = 0.1f; // Intensidad al límite de distToAlert
    [SerializeField]
    private float maxIntensity = 1.0f; // Intensidad al lado del objetivo
    [SerializeField]
    private float minInterval = 0.3f; // Intervalo de la vibración al lado del objetivo
    [SerializeField]
    private float maxInterval = 2.5f; // // Intervalo de la vibración al límite de distToAlert

    private AudioSource src;
    private bool alertsEnabled; // Flag
    private Transform currentTarget; // Transform del objetivo al que apuntamos
    private float nextAlertTime = 0f; // Contador
    private AudioClip alertClip; 
 

    void Awake() {
        src = GetComponent<AudioSource>();
        src.spatialBlend = 0f; // Audio 2D
        alertsEnabled = true;
    }

    void Update() {
        if (alertsEnabled && currentTarget != null) {
            float dist = Vector3.Distance(transform.position, currentTarget.position);

            if (dist <= distToAlert && Time.time >= nextAlertTime) {
                float t = Mathf.InverseLerp(0f, distToAlert, dist); // 0 si está encima del objeto y 1 si está en distToAlert

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
        if (evt.Clip) alertClip = evt.Clip;                                                                        
    }

    void UpdateContextAlert(float intensity) {
        src.PlayOneShot(alertClip, baseVolume * intensity); // Lanzamos alerta
        if (Gamepad.current != null) StartCoroutine(Vibrate(intensity)); // Y vibración       
    }

    IEnumerator Vibrate(float power) {
        power = Mathf.Clamp01(power);
        Gamepad.current.SetMotorSpeeds(power, power);
        yield return new WaitForSeconds(vibrationDuration);
        Gamepad.current.SetMotorSpeeds(0f, 0f);
    }

    void HandleEvent(AccessibilityEvent evt) {
        switch (evt.Type) {
            case EventType.Alert:
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
