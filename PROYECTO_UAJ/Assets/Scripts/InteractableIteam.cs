using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableItem : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Etiqueta usada para identificar al jugador")]
    public string playerTag = "Player";

    void Reset()
    {
        // Asegura que el collider sea trigger
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el que entra al trigger tiene la etiqueta del jugador, lo recogemos
        if (other.CompareTag(playerTag))
        {
            Debug.Log("¡Objeto recogido!");  // Mensaje en consola
            Destroy(gameObject);             // Elimina el objeto
        }
    }

    void OnDrawGizmosSelected()
    {
        // Muestra el volumen del collider en el editor
        var col = GetComponent<Collider>();
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);

        if (col is SphereCollider sphere)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawSphere(sphere.center, sphere.radius);
        }
        else if (col is BoxCollider box)
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(box.center, box.size);
        }
    }
}
