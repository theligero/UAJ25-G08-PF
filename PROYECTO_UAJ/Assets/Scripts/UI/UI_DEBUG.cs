using UnityEngine;
using UnityEngine.UI;

public class UI_DEBUG : MonoBehaviour
{
    [SerializeField] private Toggle toggleMostrarFlecha;
    [SerializeField] private Toggle togglePathVis;


    void Start()
    {
        if (toggleMostrarFlecha == null)
        {
            Debug.LogError("Falta asignar Toggle en UI_DEBUG");
            enabled = false;
            return;
        }

        if (togglePathVis == null)
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

        Text labelP = togglePathVis.GetComponentInChildren<Text>();
        if (labelP != null)
        {
            labelP.text = "indicador pathing";
            labelP.fontSize = 24;
            labelP.color = Color.white;
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

        // posicion del toggle pathVisualizer
        RectTransform rtP = togglePathVis.GetComponent<RectTransform>();
        if (rtP != null)
        {
            rtP.sizeDelta = new Vector2(400, 80);
            rtP.anchorMin = new Vector2(0.1f, 0.9f);
            rtP.anchorMax = new Vector2(0.1f, 0.9f);
            rtP.pivot = new Vector2(0, 1);
            rtP.anchoredPosition = new Vector2(0, 20);
        }

        toggleMostrarFlecha.onValueChanged.AddListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.AddListener(OnToggleValueChanged);
    }


    void OnDestroy()
    {
        toggleMostrarFlecha.onValueChanged.RemoveListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.RemoveListener(OnToggleValueChanged);
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
    }

}
