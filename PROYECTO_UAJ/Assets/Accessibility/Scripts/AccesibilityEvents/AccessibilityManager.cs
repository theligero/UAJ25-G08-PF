using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class AccessibilityManager : MonoBehaviour {
    public static AccessibilityManager Instance { get; private set; }

    public event Action<AccessibilityEvent> NotifyContextEvent;
    [Header("Player")]
    [SerializeField] private GameObject player;

    public GameObject Player => player;

    public bool accessibilityEnabled = true;

    [Header("Lista de objetivos accesibles")]
    [Tooltip("Arrastra aquí todos los objetivos posibles desde el editor")]
    [SerializeField] private List<Transform> objetivos = new List<Transform>();

    [Header("Objetivo actual")]
    private Transform objetivoActual;

    public Transform CurrentObjective => objetivoActual;
    public IReadOnlyList<Transform> Objetivos => objetivos;


    void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (player != null) { // Comprobamos si ya tiene el componente
            if (player.GetComponent<ArrowIndicator>() == null) {
                var indicator = player.AddComponent<ArrowIndicator>();
            }
            if (player.GetComponent<DirectionalAudio>() == null) {
                var indicator = player.AddComponent<DirectionalAudio>();
            }
            if (player.GetComponent<AlertFeedback>() == null) {
                var indicator = player.AddComponent<AlertFeedback>();
            }
            if (player.GetComponent<PathVisualizer>() == null) {
                var indicator = player.AddComponent<PathVisualizer>();
            }
            if (player.GetComponent<LineRenderer>() == null) {
                var indicator = player.AddComponent<LineRenderer>();
            }
            if (player.GetComponent<AutoRotateOnIdle>() == null) {
                var indicator = player.AddComponent<AutoRotateOnIdle>();
            }
            if (player.GetComponent<GreenFlashlight>() == null) {
                var indicator = player.AddComponent<GreenFlashlight>();
            }
            if (player.GetComponent<AutoNavigator>() == null) {
                var indicator = player.AddComponent<AutoNavigator>();
            }
        }
    }

    public void SendEvent(AccessibilityEvent evt) {
        if (!accessibilityEnabled) return;

        NotifyContextEvent?.Invoke(evt); 
    }

    private void Start()
    {
        NextObjective();
    }

    // Llama a esto desde tu código cuando quieras cambiar el objetivo activo
    public void NextObjective()
    {
        if (objetivos.Count == 0) return;

        objetivoActual = objetivos.First();
        objetivos.RemoveAt(0);

        // Enviamos un evento para que todos los sistemas reaccionen
        SendEvent(new AccessibilityEvent(
            EventType.InterestPoint,
            objetivoActual,
            AccessibilityTarget.ALL,
            $"Nuevo objetivo: {objetivoActual.name}"
        ));
    }

    public void AddObjective(Transform transform)
    {
        objetivos.Add(transform);
    }
}
