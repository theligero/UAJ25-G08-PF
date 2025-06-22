using UnityEngine;

[RequireComponent(typeof(Transform))]
public class AccessibilityTargetRegister : MonoBehaviour
{
    [Tooltip("Sistemas de accesibilidad a los que registrar este objetivo.")]
    public AccessibilityTarget sistemas = AccessibilityTarget.ALL;

    [Tooltip("Mensaje descriptivo que se enviará al registrar el objetivo.")]
    public string mensaje = "Nuevo objetivo activado";

    private void Start()
    {
        AccessibilityManager.Instance.SendEvent(new AccessibilityEvent(
            EventType.InterestPoint, transform, sistemas, mensaje
        ));
    }
}
