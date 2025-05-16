using UnityEngine;

public class ArrowIndicator : MonoBehaviour {
    private Transform currentTarget; // Objetivo a apuntar
    public Vector3 offset = new Vector3(0, 2, 0); // Altura sobre el jugador

    [SerializeField]
    private GameObject modeloFlecha;      

    private GameObject flecha;

    void Start() {
        if (currentTarget != null && modeloFlecha != null)
            CrearFlecha();
    }

    void Update() {
        if (currentTarget != null) {
            if (flecha == null && modeloFlecha != null) {
                CrearFlecha();
            }

            // Posicionar sobre el jugador
            flecha.transform.position = transform.position + offset;

            // Apuntar hacia el objetivo
            Vector3 direccion = currentTarget.transform.position - flecha.transform.position;
            if (direccion != Vector3.zero) {
                flecha.transform.rotation = Quaternion.LookRotation(direccion);
            }
        }
        else {
            if (flecha != null) {
                Destroy(flecha);
                flecha = null;
            }
        }
    }

    void OnEnable() {
        AccessibilityManager.Instance.NotifyContextEvent += HandleEvent;
    }

    void OnDisable() {
        if (AccessibilityManager.Instance != null)
            AccessibilityManager.Instance.NotifyContextEvent -= HandleEvent;
    }


    void CrearFlecha() {
        flecha = Instantiate(modeloFlecha, transform.position + offset, Quaternion.identity, transform);
    }

    public void HandleEvent(AccessibilityEvent evt) {
        switch (evt.Type) {
            case EventType.InterestPoint:
                    currentTarget = evt.Source;
                break;
            case EventType.Enable:
                if (flecha != null)
                    flecha.SetActive(true);
                break;
            case EventType.Disable:
                if (flecha != null)
                    flecha.SetActive(false);
                break;
        }
    }
}
