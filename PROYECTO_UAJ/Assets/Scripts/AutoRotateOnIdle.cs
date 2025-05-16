using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoRotateOnIdle : MonoBehaviour {
    [Header("Configuraci�n Idle")]
    [Tooltip("Tiempo en segundos que el jugador debe estar quieto para activar la rotaci�n autom�tica")]
    public float idleTimeThreshold = 3f;

    [Tooltip("Velocidad de rotaci�n al girar hacia el item (grados/segundo)")]
    public float rotationSpeed = 360f;

    private CharacterController characterController;
    private float idleTimer = 0f;
    private bool isIdle = false;

    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    void Update() {
        // 1. Detectar movimiento del jugador
        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;
        if (velocity.sqrMagnitude < 0.01f) {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeThreshold)
                isIdle = true;
        }
        else {
            idleTimer = 0f;
            isIdle = false;
        }

        // 2. Si est� idle, buscar el interactable m�s cercano y rotar en 3D
        if (isIdle) {
            InteractableItem closestItem = null;
            float minDist = float.MaxValue;
            foreach (var item in Object.FindObjectsByType<InteractableItem>(UnityEngine.FindObjectsSortMode.None)) {
                float dist = Vector3.Distance(transform.position, item.transform.position);
                if (dist < minDist) {
                    minDist = dist;
                    closestItem = item;
                }
            }

            if (closestItem != null) {
                // Direccion completa
                Vector3 direction = closestItem.transform.position - transform.position;
                direction.y = 0f; // Eliminar componente vertical

                // Rotaci�n objetivo en 3D
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Girar suavemente hacia la rotaci�n objetivo
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
