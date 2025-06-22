using UnityEngine;

[AddComponentMenu("Camera/Camera Follow Smooth")]
public class CameraFollow : MonoBehaviour
{
    [Header("Target to Follow")]
    [Tooltip("Transform del jugador que la cámara seguirá")]
    public Transform target;

    [Header("Offsets")]
    [Tooltip("Distancia relativa de la cámara respecto al jugador")]
    public Vector3 offset = new Vector3(0f, 5f, -10f);

    [Header("Suavizado")]
    [Range(0.01f, 1f)]
    [Tooltip("Velocidad de interpolación (más bajo = más suave)")]
    public float smoothSpeed = 0.125f;

    [Header("Opciones de Rotación")]
    [Tooltip("¿La cámara debe mirar siempre al jugador?")]
    public bool lookAtTarget = true;

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Posición deseada
        Vector3 desiredPosition = target.position + offset;
        // 2. Interpolación suave
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 3. (Opcional) Mirar al jugador
        if (lookAtTarget)
        {
            Vector3 lookPoint = target.position;
            // Puedes ajustar un ligero balanceo en Y si quieres
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(lookPoint - transform.position),
                smoothSpeed
            );
        }
    }
}
