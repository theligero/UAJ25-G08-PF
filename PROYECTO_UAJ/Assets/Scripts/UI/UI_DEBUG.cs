using UnityEngine;
using UnityEngine.UI;

public class UI_DEBUG : MonoBehaviour
{
    [SerializeField] private Toggle toggleMostrarFlecha;
    [SerializeField] private Toggle togglePathVis;
    [SerializeField] private Toggle toggleAutoRotate; // Nuevo toggle para AutoRotate

    void Start()
    {
        // Validaciones
        if (toggleMostrarFlecha == null || togglePathVis == null || toggleAutoRotate == null)
        {
            Debug.LogError("Falta asignar algún Toggle en UI_DEBUG");
            enabled = false;
            return;
        }

        // Configurar texto y estilo
        SetupToggleLabel(toggleMostrarFlecha, "indicador flecha");
        SetupToggleLabel(togglePathVis, "indicador pathing");
        SetupToggleLabel(toggleAutoRotate, "auto rotate");

        // Posiciones y tamaños
        SetupToggleRect(toggleMostrarFlecha, new Vector2(0f, 0f));
        SetupToggleRect(togglePathVis, new Vector2(0f, 40f));
        SetupToggleRect(toggleAutoRotate, new Vector2(0f, 80f));

        // Añadir listeners
        toggleMostrarFlecha.onValueChanged.AddListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.AddListener(OnToggleValueChanged);
        toggleAutoRotate.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnDestroy()
    {
        toggleMostrarFlecha.onValueChanged.RemoveListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleAutoRotate.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void SetupToggleLabel(Toggle toggle, string text)
    {
        Text label = toggle.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.text = text;
            label.fontSize = 24;
            label.color = Color.white;
        }
    }

    private void SetupToggleRect(Toggle toggle, Vector2 anchoredPos)
    {
        RectTransform rt = toggle.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(400, 80);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0, 0);
            rt.anchoredPosition = anchoredPos;
        }
    }

    private void OnToggleValueChanged(bool isOn)
    {
        if (AccessibilityManager.Instance == null)
        {
            Debug.LogWarning("AccessibilityManager no existe");
            return;
        }

        Toggle toggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>();

        if (toggle == toggleMostrarFlecha)
        {
            AccessibilityEvent evt = new AccessibilityEvent(
                isOn ? EventType.Enable : EventType.Disable,
                toggleMostrarFlecha.transform,
                AccessibilityTarget.ArrowIndicator,
                "Toggle UI cambio visibilidad flecha"
            );
            AccessibilityManager.Instance.SendEvent(evt);
        }
        else if (toggle == togglePathVis)
        {
            AccessibilityEvent evt = new AccessibilityEvent(
                isOn ? EventType.Enable : EventType.Disable,
                togglePathVis.transform,
                AccessibilityTarget.PathVisualizer,
                "Toggle UI cambio visibilidad path"
            );
            AccessibilityManager.Instance.SendEvent(evt);
        }
        else if (toggle == toggleAutoRotate)
        {
            AccessibilityEvent evt = new AccessibilityEvent(
                isOn ? EventType.Enable : EventType.Disable,
                toggleAutoRotate.transform,
                AccessibilityTarget.AutoRotate, // Asegúrate que este enum existe y está definido
                "Toggle UI cambio auto rotate"
            );
            AccessibilityManager.Instance.SendEvent(evt);
        }
    }
}
