using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[AddComponentMenu("Lighting/Flashlight With Multi-Aim")]
public class GreenFlashlight : MonoBehaviour {
    [Header("Beam Settings")]
    public Light flashlight;
    public Vector3 offset = new Vector3(0, 0, 0.5f);
    public Color defaultColor = Color.green;
    public Color aimColor = Color.red;
    [Range(1f, 90f)] public float aimAngleThreshold = 15f;

    [Header("Toggle")]
    public KeyCode toggleKey = KeyCode.Q;
    private bool isOn = true;
    private List<Transform> interestPoints = new List<Transform>();

    void Awake() {
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

    void OnEnable()
    {
        AccessibilityManager.Instance.NotifyContextEvent += HandleEvent;
    }

    void OnDisable()
    {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.NotifyContextEvent -= HandleEvent;
    }

    void LateUpdate() {
        if (!isOn) return;

        // Mantener posición y rotación
        flashlight.transform.position = transform.TransformPoint(offset);
        flashlight.transform.rotation = transform.rotation;

        // 1. Busca el interactable más cercano frente a ti
        Transform best = null;
        float bestScore = float.MinValue;

        foreach (var target in interestPoints) {
            if (target == null) continue;

            Vector3 toTarget = (target.position - flashlight.transform.position).normalized;
            float dot = Vector3.Dot(flashlight.transform.forward, toTarget);
            if (dot > bestScore) {
                bestScore = dot;
                best = target;
            }
        }

        // 2. Si el mejor dot se corresponde a un ángulo bajo, cambiar color
        if (best != null) {
            float angle = Mathf.Acos(Mathf.Clamp(bestScore, -1f, 1f)) * Mathf.Rad2Deg;
            flashlight.color = (angle <= aimAngleThreshold) ? aimColor : defaultColor;
        }
        else {
            flashlight.color = defaultColor;
        }
    }

    public void HandleEvent(AccessibilityEvent evt) {
        if (evt.Target != AccessibilityTarget.ArrowIndicator && evt.Target != AccessibilityTarget.ALL)
            return;

        switch (evt.Type) {
            case EventType.InterestPoint:
                if (!interestPoints.Contains(evt.Source))
                    interestPoints.Add(evt.Source);
                break;
            case EventType.Enable:
                isOn = true;
                break;
            case EventType.Disable:
                isOn = false;
                break;
        }
    }
}
