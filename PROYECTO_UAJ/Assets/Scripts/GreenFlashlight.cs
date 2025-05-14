using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[AddComponentMenu("Lighting/Flashlight With Multi-Aim")]
public class GreenFlashlight : MonoBehaviour
{
    [Header("Beam Settings")]
    public Light flashlight;
    public Vector3 offset = new Vector3(0, 0, 0.5f);
    public Color defaultColor = Color.green;
    public Color aimColor = Color.red;
    [Range(1f, 90f)] public float aimAngleThreshold = 15f;

    [Header("Toggle")]
    public KeyCode toggleKey = KeyCode.Q;
    private bool isOn = true;

    void Awake()
    {
        // Crea spotlight si no hay
        if (flashlight == null)
        {
            var go = new GameObject("Flashlight");
            go.transform.SetParent(transform);
            go.transform.localPosition = offset;
            flashlight = go.AddComponent<Light>();
            flashlight.type = LightType.Spot;
        }
        flashlight.color = defaultColor;
        flashlight.enabled = isOn;
    }

    void Update() {
        if (Input.GetKeyDown(toggleKey))
            flashlight.enabled = isOn = !isOn;
    }

    void LateUpdate() {
        if (!isOn) return;

        // Mantener posición y rotación
        flashlight.transform.position = transform.TransformPoint(offset);
        flashlight.transform.rotation = transform.rotation;

        // 1. Busca el interactable más cercano frente a ti
        InteractableItem best = null;
        float bestScore = float.MinValue;
        foreach (var item in Object.FindObjectsByType<InteractableItem>(UnityEngine.FindObjectsSortMode.None)) {
            Vector3 toItem = (item.transform.position - flashlight.transform.position).normalized;
            float dot = Vector3.Dot(flashlight.transform.forward, toItem);
            // dot=1 frontal, dot=-1 atrás
            if (dot > bestScore) {
                bestScore = dot;
                best = item;
            }
        }

        // 2. Si el mejor dot se corresponde a un ángulo bajo, cambiar color
        if (best != null) {
            float angle = Mathf.Acos(Mathf.Clamp(bestScore, -1f, 1f)) * Mathf.Rad2Deg;
            flashlight.color = (angle <= aimAngleThreshold)
                ? aimColor
                : defaultColor;
        }
    }
}
