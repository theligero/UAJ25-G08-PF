using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoRotateOnIdle : MonoBehaviour {
    [Header("Configuración Idle")]
    [Tooltip("Tiempo en segundos que el jugador debe estar quieto para activar la rotación automática")]
    [SerializeField] private float idleTimeThreshold = 3f;

    [Tooltip("Velocidad de rotación al girar hacia el item (grados/segundo)")]
    [SerializeField] private float rotationSpeed = 360f;

    [Header("Dependencias")]
    [Tooltip("El CharacterController asociado al jugador")]
    [SerializeField] private CharacterController characterController;

    [Tooltip("Lista de objetos en la escena a los que se puede rotar")]
    [SerializeField] private List<Transform> interactableItems = new List<Transform>();

    // Internos
    private float idleTimer = 0f;
    private bool isIdle = false;
    private bool active = true;

    // Se ejecuta en editor o al añadir el componente por primera vez
    private void Reset() {
        // Intentamos auto-asignar el CharacterController si existe
        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Awake() {
        // Validación
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            Debug.LogError($"[{nameof(AutoRotateOnIdle)}] Falta asignar CharacterController en {gameObject.name}");
    }

    private void Update() {
        if (active) {
            // 1. Detectar si el jugador está quieto
            Vector3 flatVelocity = characterController.velocity;
            flatVelocity.y = 0f;
            if (flatVelocity.sqrMagnitude < 0.01f) {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleTimeThreshold)
                    isIdle = true;
            }
            else {
                idleTimer = 0f;
                isIdle = false;
            }

            // 2. Si está idle y tenemos items asignados, girar hacia el más cercano
            if (isIdle && interactableItems != null && interactableItems.Count > 0){
                Transform closest = null;
                float minDist = float.MaxValue;

                foreach (var item in interactableItems) {
                    if (item == null) continue;
                    float dist = Vector3.Distance(transform.position, item.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = item;
                    }
                }

                if (closest != null) {
                    // Calculamos la dirección en el plano XZ
                    Vector3 dir = closest.transform.position - transform.position;
                    dir.y = 0f;

                    // Rotación suave hacia el objetivo
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRot,
                        rotationSpeed * Time.deltaTime
                    );
                }
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

    public void HandleEvent(AccessibilityEvent evt) {
        if (evt.Target != AccessibilityTarget.AutoRotate && evt.Target != AccessibilityTarget.ALL)
            return;

        switch (evt.Type) {
            case EventType.InterestPoint:
                interactableItems.Add(evt.Source);
                break;
            case EventType.Enable:
                active = true;
                break;
            case EventType.Disable:
                active = false;
                break;
        }
    }
}
