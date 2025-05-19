using UnityEngine;
using UnityEngine.UI;

public class UI_DEBUG : MonoBehaviour
{
    [Header("Toggles")]
    [SerializeField] private Toggle toggleMostrarFlecha;
    [SerializeField] private Toggle togglePathVis;
    [SerializeField] private Toggle toggleAutoRotate;
    [SerializeField] private Toggle toggleFlashlight;
    [SerializeField] private Toggle toggleAlert;
    [SerializeField] private Toggle toggleAutoNavegacion;
    [SerializeField] private Toggle toggleAudioDireccional;

    private const float SPACING_Y = 32f;

    void Start()
    {
        if (toggleMostrarFlecha == null || togglePathVis == null || toggleAutoRotate == null ||
            toggleFlashlight == null || toggleAlert == null || toggleAutoNavegacion == null ||
            toggleAudioDireccional == null)
        {
            Debug.LogError("Falta asignar algún Toggle en UI_DEBUG");
            enabled = false;
            return;
        }

        SetupToggleLabel(toggleMostrarFlecha, "Indicador flecha");
        SetupToggleLabel(togglePathVis, "Indicador path");
        SetupToggleLabel(toggleAutoRotate, "Auto rotación");
        SetupToggleLabel(toggleFlashlight, "Linterna");
        SetupToggleLabel(toggleAlert, "Alerta accesibilidad");
        SetupToggleLabel(toggleAutoNavegacion, "Autonavegación");
        SetupToggleLabel(toggleAudioDireccional, "Audio direccional");

        SetupToggleRect(toggleMostrarFlecha, new Vector2(0f, SPACING_Y * 0));
        SetupToggleRect(togglePathVis, new Vector2(0f, SPACING_Y * 1));
        SetupToggleRect(toggleAutoRotate, new Vector2(0f, SPACING_Y * 2));
        SetupToggleRect(toggleFlashlight, new Vector2(0f, SPACING_Y * 3));
        SetupToggleRect(toggleAlert, new Vector2(0f, SPACING_Y * 4));
        SetupToggleRect(toggleAutoNavegacion, new Vector2(0f, SPACING_Y * 5));
        SetupToggleRect(toggleAudioDireccional, new Vector2(0f, SPACING_Y * 6));

        toggleMostrarFlecha.onValueChanged.AddListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.AddListener(OnToggleValueChanged);
        toggleAutoRotate.onValueChanged.AddListener(OnToggleValueChanged);
        toggleFlashlight.onValueChanged.AddListener(OnToggleValueChanged);
        toggleAlert.onValueChanged.AddListener(OnToggleValueChanged);
        toggleAutoNavegacion.onValueChanged.AddListener(OnToggleValueChanged);
        toggleAudioDireccional.onValueChanged.AddListener(OnToggleValueChanged);
    }

    void OnDestroy()
    {
        toggleMostrarFlecha.onValueChanged.RemoveListener(OnToggleValueChanged);
        togglePathVis.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleAutoRotate.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleFlashlight.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleAlert.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleAutoNavegacion.onValueChanged.RemoveListener(OnToggleValueChanged);
        toggleAudioDireccional.onValueChanged.RemoveListener(OnToggleValueChanged);
    }

    private void SetupToggleLabel(Toggle toggle, string text)
    {
        Text label = toggle.GetComponentInChildren<Text>();
        if (label != null)
        {
            label.text = text;
            label.fontSize = 20;
            label.color = Color.white;
        }
    }

    private void SetupToggleRect(Toggle toggle, Vector2 anchoredPos)
    {
        RectTransform rt = toggle.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(400, 60);
            rt.anchorMin = new Vector2(0f, 0f);
            rt.anchorMax = new Vector2(0f, 0f);
            rt.pivot = new Vector2(0f, 0f);
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

        Toggle toggle = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Toggle>();

        if (toggle == toggleMostrarFlecha)
        {
            SendEvent(toggleMostrarFlecha.transform, AccessibilityTarget.ArrowIndicator, isOn, "Cambio visibilidad flecha");
        }
        else if (toggle == togglePathVis)
        {
            SendEvent(togglePathVis.transform, AccessibilityTarget.PathVisualizer, isOn, "Cambio visibilidad path");
        }
        else if (toggle == toggleAutoRotate)
        {
            SendEvent(toggleAutoRotate.transform, AccessibilityTarget.AutoRotate, isOn, "Cambio auto rotación");
        }
        else if (toggle == toggleFlashlight)
        {
            SendEvent(toggleFlashlight.transform, AccessibilityTarget.Flashlight, isOn, "Cambio linterna");
        }
        else if (toggle == toggleAlert)
        {
            SendEvent(toggleAlert.transform, AccessibilityTarget.Alert, isOn, "Cambio alerta accesibilidad");
        }
        else if (toggle == toggleAutoNavegacion)
        {
            SendEvent(toggleAutoNavegacion.transform, AccessibilityTarget.AutoNavigator, isOn, "Cambio autonavegación");
        }
        else if (toggle == toggleAudioDireccional)
        {
            SendEvent(toggleAudioDireccional.transform, AccessibilityTarget.DirectionalAudio, isOn, "Cambio audio direccional");
        }
    }

    private void SendEvent(Transform source, AccessibilityTarget target, bool enabled, string msg)
    {
        AccessibilityEvent evt = new AccessibilityEvent(
            enabled ? EventType.Enable : EventType.Disable,
            source,
            target,
            $"Toggle UI {msg}"
        );
        AccessibilityManager.Instance.SendEvent(evt);
    }
}
