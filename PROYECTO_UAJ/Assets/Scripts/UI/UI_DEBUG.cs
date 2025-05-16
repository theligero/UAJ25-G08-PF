using UnityEngine;
using UnityEngine.UI;

public class UI_DEBUG : MonoBehaviour
{
    [SerializeField] private Toggle toggleMostrarFlecha;

    void Start()
    {
        if (toggleMostrarFlecha == null)
        {
            Debug.LogError("Falta asignar Toggle en UI_DEBUG");
            enabled = false;
            return;
        }

        // Cambiar texto y tamaño
        Text label = toggleMostrarFlecha.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.text = "indicador flecha";
            label.fontSize = 24;
            label.color = Color.white;
        }

        // Ajustar tamaño y posición del toggle
        RectTransform rt = toggleMostrarFlecha.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(400, 80);
            rt.anchorMin = new Vector2(0.1f, 0.9f);
            rt.anchorMax = new Vector2(0.1f, 0.9f);
            rt.pivot = new Vector2(0, 1);
            rt.anchoredPosition = Vector2.zero;
        }

        toggleMostrarFlecha.onValueChanged.AddListener(OnToggleValueChanged);
    }


    void OnDestroy()
    {
        toggleMostrarFlecha.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (AccessibilityManager.Instance == null)
        {
            Debug.LogWarning("AccessibilityManager no existe");
            return;
        }

        AccessibilityEvent evt = new AccessibilityEvent(
            isOn ? EventType.Enable : EventType.Disable,
            toggleMostrarFlecha.transform,  // fuente real
            AccessibilityTarget.ArrowIndicator,
            "Toggle UI cambio visibilidad flecha"
        );

        AccessibilityManager.Instance.SendEvent(evt);
    }
}
